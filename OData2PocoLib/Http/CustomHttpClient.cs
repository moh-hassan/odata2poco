// Copyright (c) Mohamed Hassan & Contributors. All rights reserved. See License.md in the project root for license information.

#pragma warning disable SA1202
namespace OData2Poco.Http;

using System.Net;
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

    public CustomHttpClient(OdataConnectionString odataConnectionString)
    {
        OdataConnection = odataConnectionString;
        ServiceUri = new Uri(OdataConnection.ServiceUrl);
        _httpHandler = new HttpClientHandler
        {
            UseDefaultCredentials = true,
            AllowAutoRedirect = true,
            AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
        };
        _client = new HttpClient(_httpHandler);
    }

    public CustomHttpClient(OdataConnectionString odataConnectionString,
        DelegatingHandler dh) : this(odataConnectionString)
    {
        _delegatingHandler = dh;
        _delegatingHandler.InnerHandler = _httpHandler;
        _client = new HttpClient(_delegatingHandler);
    }

    public Uri ServiceUri { get; set; }

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
        //check url is remote xml file
        try
        {
            _response = await GetAsync(url).ConfigureAwait(false);
            _response.EnsureSuccessStatusCode();
            var content = await _response.Content.ReadAsStringAsync().ConfigureAwait(false);
            return content;
        }
        catch (Exception)
        {
            if (_response?.StatusCode != HttpStatusCode.Unauthorized)
            {
                throw;
            }

            var wwwAuthenticate = _response.Headers.WwwAuthenticate.ToString();
            throw new OData2PocoException(
                $"HTTP {_response.StatusCode} ({(int)_response.StatusCode}): {wwwAuthenticate}");
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
            Logger.Warn("Skip Certification Check is set to true.");
        }

        //setup SecurityProtocol
        if (OdataConnection.TlsProtocol > 0)
        {
            ServicePointManager.SecurityProtocol |= OdataConnection.TlsProtocol;
            Logger.Info($"Setting the SSL/TLS protocols to: {OdataConnection.TlsProtocol}");
        }

        SetupProxy();

        _client.DefaultRequestHeaders
            .TryAddWithoutValidation("Content-Type", "application/json; charset=utf-8");
        const string Agent = "OData2Poco";
        _client.DefaultRequestHeaders.Add("User-Agent", Agent);
        SetupHeader();

        if (OdataConnection.Authenticate != AuthenticationType.None)
        {
            Authenticator auth = new(this);
            return auth.Authenticate();
        }

        return Task.CompletedTask;
    }

    private void SetupProxy()
    {
        if (!string.IsNullOrEmpty(OdataConnection.Proxy))
        {
            Logger.Trace($"Using Proxy: '{OdataConnection.Proxy}'");
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
                    Logger.Warn("ProxyUser is not in the correct format. Expected format is 'username:password'.");
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
        OdataConnection.Password.Dispose();
        _httpHandler.Dispose();
        _delegatingHandler?.Dispose();
        _response?.Dispose();
        _client.Dispose();
    }
}
