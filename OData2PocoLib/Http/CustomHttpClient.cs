﻿// Copyright (c) Mohamed Hassan & Contributors. All rights reserved. See License.md in the project root for license information.

namespace OData2Poco.Http;

using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Security.Authentication;
using System.Text.RegularExpressions;
using Extensions;
using InfraStructure.Logging;

internal class CustomHttpClient : IDisposable
{
    internal static readonly ILog Logger = PocoLogger.Default;
    internal readonly OdataConnectionString OdataConnection;
    internal HttpClient _client;
    internal HttpClientHandler _httpHandler;
    public Uri ServiceUri { get; }
    internal string UserAgent { get; } = GetHttpAgent();
    private CustomHttpClient(OdataConnectionString odataConnectionString)
    {
        _ = odataConnectionString ?? throw new ArgumentNullException(nameof(odataConnectionString));
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
    }

    public static async Task<CustomHttpClient> CreateAsync(
       OdataConnectionString odataConnectionString, PocoSetting pocoSetting)
    {
        _ = pocoSetting ?? throw new ArgumentNullException(nameof(pocoSetting));
        var cc = new CustomHttpClient(odataConnectionString);
        cc.SetHttpClient();
        cc._client.DefaultRequestHeaders.IfModifiedSince = pocoSetting.GetLastUpdate();
        await cc.Authenicate().ConfigureAwait(false);
        await cc.LoginAsync().ConfigureAwait(false);
        return cc;
    }

    public async Task<HttpResponseMessage?> GetAsync(string url)
    {
        HttpResponseMessage? response = null;
        try
        {
            response = await _client.GetAsync(new Uri(url)).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();
            return response;
        }
        catch (Exception ex)
        {
            HandleHttpException(response, ex);
        }
        return response;
    }
    internal async Task<HttpResponseMessage?> ReadMetaDataAsync()
    {
        var url = ServiceUri.AbsoluteUri.EndsWith(".xml")
            ? ServiceUri.AbsoluteUri
            : ServiceUri.AbsoluteUri.TrimEnd('/') + "/$metadata";
        var response = await GetAsync(url).ConfigureAwait(false);
        return response;
    }
    internal async Task<string> ReadMetaDataAsStringAsync()
    {
        var response = await ReadMetaDataAsync().ConfigureAwait(false);
        if (response == null) return string.Empty;
        var metadata = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
        return metadata;
    }
    private static string GetHttpAgent()
    {
        var assembly = Assembly.GetCallingAssembly();
        var version = assembly.GetName().Version;
        if (version == null)
            return "OData2Poco";

        var strVersion = $"OData2Poco/{version} (https://github.com/moh-hassan/odata2poco)";
        return strVersion;
    }
    private async Task<bool> LoginAsync()
    {
        var loginUrl = OdataConnection.LogInUrl;
        if (string.IsNullOrEmpty(loginUrl))
            return false;
        Logger.Info($"Logging in using the login URL: {loginUrl}");
        HttpResponseMessage? response = null;
        try
        {
            response = await _client.GetAsync(new Uri(loginUrl)).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();
            Logger.Info("Login successful and cookies have been stored.");
        }
        catch (Exception ex)
        {
            HandleHttpException(response, ex);
        }
        return true;
    }

    private void SetupHeader()
    {
        //set if-modified-since header
        //if (OdataConnection.LastUpdated.HasValue)
        //    _client.DefaultRequestHeaders.IfModifiedSince = OdataConnection.LastUpdated;

        if (OdataConnection.HttpHeader == null || !OdataConnection.HttpHeader.Any())
        {
            return;
        }

        foreach (var header in OdataConnection.HttpHeader)
        {
            var header2 = header.ReplaceToBase64();
            var match = Regex.Match(header2, @"^(?<key>[^:=]+)[:=](?<value>.+)$");
            if (match.Success)
            {
                var key = match.Groups["key"].Value.Trim().Trim('"');
                var value = match.Groups["value"].Value.Trim().Trim('"');
                _client.DefaultRequestHeaders.Add(key, value);
            }
        }
    }

    private void SetHttpClient()
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
    }

    private async Task Authenicate()
    {
        if (OdataConnection.Authenticate == AuthenticationType.None) return;
        Authenticator auth = new(this);
        await auth.Authenticate().ConfigureAwait(false);
    }

    private void SetupSsl(HttpClientHandler handler)
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

    private void HandleHttpException(HttpResponseMessage? response, Exception ex)
    {
        if (response != null)
        {
            switch (response.StatusCode)
            {
                case HttpStatusCode.NotModified:
                    // Handle 304 Not Modified
                    var lastModified = response.Content.Headers.LastModified;
                    Logger.Warn($"Not Modified Exception: OData Metadata has not been modified.\nLast Modified on: '{lastModified}'.");
                    var metadataException = new MetaDataNotUpdatedException("The metadata has not been modified and No code generation is done.", ex);
                    metadataException.Data.Add("StatusCode", response.StatusCode);
                    metadataException.Data.Add("RequestUri", response.RequestMessage?.RequestUri);
                    metadataException.Data.Add("LastModified", lastModified);
                    throw metadataException;
            }
        }

        if (ex is HttpRequestException)
        {
            var errorMessage = ex.Message;
            var oData2PocoException = new OData2PocoException(errorMessage, ex
            );
            oData2PocoException.Data.Add("StatusCode", response?.StatusCode);
            oData2PocoException.Data.Add("RequestUri", response?.RequestMessage?.RequestUri);
            throw oData2PocoException;
        }
        else
        {
            throw ex;
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
            _httpHandler.Dispose();
            OdataConnection.Password.Dispose();
        }
    }
}
