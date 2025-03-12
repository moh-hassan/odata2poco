// Copyright (c) Mohamed Hassan & Contributors. All rights reserved. See License.md in the project root for license information.

#pragma warning disable SA1202
namespace OData2Poco.Http;

using System.Net;
using System.Reflection;
using System.Security.Authentication;
using Extensions;
using InfraStructure.Logging;

internal class CustomHttpClient : IDisposable
{
    public HttpResponseMessage? _response = new();
    internal static readonly ILog Logger = PocoLogger.Default;
    internal readonly OdataConnectionString OdataConnection;
    internal HttpClient _client;
    internal DelegatingHandler? _delegatingHandler;
    internal HttpClientHandler _httpHandler;
    internal static string UserAgent { get; } = GetHttpAgent();

    public CustomHttpClient(OdataConnectionString odataConnectionString)
    {
        OdataConnection = odataConnectionString;
        ServiceUri = new Uri(OdataConnection.ServiceUrl);
        _httpHandler = new HttpClientHandler
        {
            UseDefaultCredentials = true,
            AllowAutoRedirect = true,
            AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate,
            UseCookies = true,
            CookieContainer = new CookieContainer(),
        };
        _client = new HttpClient(_httpHandler);
        //set if-modified-since header
        if (OdataConnection.LastUpdated.HasValue)
            _client.DefaultRequestHeaders.IfModifiedSince = OdataConnection.LastUpdated;
    }

    public CustomHttpClient(OdataConnectionString odataConnectionString,
        DelegatingHandler dh) : this(odataConnectionString)
    {
        _delegatingHandler = dh;
        _delegatingHandler.InnerHandler = _httpHandler;
        _client = new HttpClient(_delegatingHandler);
    }

    public Uri ServiceUri { get; set; }

    private static string GetHttpAgent()
    {
        var assembly = Assembly.GetCallingAssembly();
        var version = assembly.GetName().Version;
        if (version == null)
            return "OData2Poco";

        var strVersion = $"OData2Poco/{version.ToString()} (https://github.com/moh-hassan/odata2poco)";
        return strVersion;
    }

    internal virtual async Task<HttpResponseMessage> GetAsync(string? requestUri)
    {
        _ = requestUri ?? throw new ArgumentNullException(nameof(requestUri));
        await SetHttpClient().ConfigureAwait(false);
        var response = await _client.GetAsync(new Uri(requestUri)).ConfigureAwait(false);
        if (response.StatusCode is HttpStatusCode.ServiceUnavailable or
            HttpStatusCode.GatewayTimeout)
        {
            response = await Policy
                .RetryAsync(() => _client.GetAsync(new Uri(requestUri)), 2).ConfigureAwait(false);
        }

        return response ?? throw new OData2PocoException("Response is null");
    }

    internal async Task<string> ReadMetaDataAsync()
    {
        var url = ServiceUri.AbsoluteUri.EndsWith(".xml") ? ServiceUri.AbsoluteUri : ServiceUri.AbsoluteUri.TrimEnd('/') + "/$metadata";
        try
        {
            _response = await GetAsync(url).ConfigureAwait(false);
            _response.EnsureSuccessStatusCode();
            var content = await _response.Content.ReadAsStringAsync().ConfigureAwait(false);
            return content;
        }
        catch (HttpRequestException ex) when (_response?.StatusCode == HttpStatusCode.NotModified)
        {
            var lastModified = _response.Content.Headers.LastModified;
            Logger.Info($"OData Metadata was Last modified on '{lastModified}'.");
            throw new MetaDataNotUpdatedException("The metadata has not been modified and No code generation is done.", ex);
        }
        catch (HttpRequestException ex) when (_response?.StatusCode == HttpStatusCode.Unauthorized)
        {
            var wwwAuthenticate = _response.Headers.WwwAuthenticate.ToString();
            throw new OData2PocoException(
                $"Request failed with status code ({(int)_response.StatusCode}) {_response.StatusCode}.\nWWW-Authenticate: {wwwAuthenticate}", ex);
        }
        catch (Exception)
        {
            throw;
        }
    }

    private void SetupHeader()
    {
        if (OdataConnection.HttpHeader == null || !OdataConnection.HttpHeader.Any())
        {
            return;
        }

        foreach (var header in OdataConnection.HttpHeader)
        {
            header.TryReplaceToBase64(out var header2);
            var pair = header2.Split([':', '='], 2);
            if (pair.Length == 2)
            {
                var key = pair[0].Trim().Trim('"');
                var value = pair[1].Trim().Trim('"');
                _client.DefaultRequestHeaders.TryAddWithoutValidation(key, value);
            }
        }
    }

    private Task SetHttpClient()
    {
        //setup Skip Certification Check.
        //Use in development environment, but is not recommended in production
        if (OdataConnection.SkipCertificationCheck)
        {
            _httpHandler.ServerCertificateCustomValidationCallback = (_, _, _, _) => true;
            Logger.Warn("Skip certification check is set to true.");
        }

        if (OdataConnection.TlsProtocol > 0)
            SetupSsl(_httpHandler);
        SetupProxy();

        _client.DefaultRequestHeaders
            .TryAddWithoutValidation("Content-Type", "application/json; charset=utf-8");
        var agent = GetHttpAgent();
        _client.DefaultRequestHeaders.Add("User-Agent", agent);
        SetupHeader();

        if (OdataConnection.Authenticate != AuthenticationType.None)
        {
            Authenticator auth = new(this);
            return auth.Authenticate();
        }

        return Task.CompletedTask;
    }

    public void SetupSsl(HttpClientHandler handler)
    {
#if NET8_0_OR_GREATER
        handler.SslProtocols = (SslProtocols)OdataConnection.TlsProtocol;
        handler.ServerCertificateCustomValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true;
#else
        ServicePointManager.SecurityProtocol |= OdataConnection.TlsProtocol;
#endif
        Logger.Info($"Setting the SSL/TLS protocols to: {OdataConnection.TlsProtocol}.");
    }

    private void SetupProxy()
    {
        if (!string.IsNullOrEmpty(OdataConnection.Proxy))
        {
            Logger.Trace($"Using proxy: '{OdataConnection.Proxy}'");
            _httpHandler.UseProxy = true;
            _httpHandler.Proxy = new WebProxy(OdataConnection.Proxy);
            if (!string.IsNullOrEmpty(OdataConnection.ProxyUser))
            {
                var credentials = OdataConnection.ProxyUser.Split(':');
                if (credentials.Length == 2)
                {
                    var proxyUserName = credentials[0];
                    var proxyPassword = credentials[1];
                    _httpHandler.Proxy.Credentials = new NetworkCredential(proxyUserName, proxyPassword);
                    //disable default credentials
                    _httpHandler.UseDefaultCredentials = false;
                }
                else
                {
                    Logger.Warn("ProxyUser is not in the correct format. The expected format is 'username:password'.");
                }
            }
            else
            {
                //enable default credentials
                _httpHandler.UseDefaultCredentials = true;
            }
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            _client.Dispose();
            _response?.Dispose();
            _delegatingHandler?.Dispose();
            _httpHandler.Dispose();
            OdataConnection.Password.Dispose();
        }
    }
}
