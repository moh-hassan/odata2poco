// Copyright (c) Mohamed Hassan & Contributors. All rights reserved. See License.md in the project root for license information.

using System.Net;
using OData2Poco.Extensions;
using OData2Poco.InfraStructure.Logging;

namespace OData2Poco.Http;

internal class CustomHttpClient : IDisposable
{
    private static readonly ILog Logger = PocoLogger.Default;
    internal DelegatingHandler? _delegatingHandler;
    internal readonly OdataConnectionString OdataConnection;
    internal HttpClient Client;
    internal HttpClientHandler HttpHandler;
    public HttpResponseMessage Response = new();
    public Uri ServiceUri { get; set; }

    public CustomHttpClient(OdataConnectionString odataConnectionString)
    {

        OdataConnection = odataConnectionString;
        ServiceUri = new Uri(OdataConnection.ServiceUrl);
        HttpHandler = new HttpClientHandler
        {
            UseDefaultCredentials = true,
            AllowAutoRedirect = true,
            AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
        };
        Client = new HttpClient(HttpHandler);
    }

    public CustomHttpClient(OdataConnectionString odataConnectionString,
        DelegatingHandler dh) : this(odataConnectionString)
    {
        _delegatingHandler = dh;
        _delegatingHandler.InnerHandler = HttpHandler;
        Client = new HttpClient(_delegatingHandler);
    }



    private void SetupHeader()
    {

        if (OdataConnection.HttpHeader == null || !OdataConnection.HttpHeader.Any()) return;
        foreach (var header in OdataConnection.HttpHeader)
        {
            header.TryReplaceToBase64(out var header2);

            var pair = header2.Split(new[] { ':', '=' }, 2);
            if (pair.Length == 2)
            {
                var key = pair[0].Trim().Trim('"');
                var value = pair[1].Trim().Trim('"');
                Client.DefaultRequestHeaders.TryAddWithoutValidation(key, value);
            }
        }
    }

    private async Task SetHttpClient()
    {
        //setup Skip Certification Check.
        //Use in development environment, but is not recommended in production 
        if (OdataConnection.SkipCertificationCheck)
        {
            HttpHandler.ServerCertificateCustomValidationCallback = (_, _, _, _) => true;
            Logger.Warn("Skip Certification Check is set to true.");
        }

        //setup SecurityProtocol
        if (OdataConnection.TlsProtocol > 0)
        {
            ServicePointManager.SecurityProtocol |= OdataConnection.TlsProtocol;
            Logger.Info($"Setting the SSL/TLS protocols to: {OdataConnection.TlsProtocol}");
        }

        if (!string.IsNullOrEmpty(OdataConnection.Proxy))
        {
            Logger.Trace($"Using Proxy: '{OdataConnection.Proxy}'");
            HttpHandler.UseProxy = true;
            HttpHandler.Proxy = new WebProxy(OdataConnection.Proxy);
        }

        Client.DefaultRequestHeaders
            .TryAddWithoutValidation("Content-Type", "application/json; charset=utf-8");
        var agent = "OData2Poco";
        Client.DefaultRequestHeaders.Add("User-Agent", agent);
        SetupHeader();

        if (OdataConnection.Authenticate != AuthenticationType.None)
        {
            Authenticator auth = new(this);
            await auth.Authenticate();
        }
    }
    internal virtual async Task<HttpResponseMessage> GetAsync(string? requestUri)
    {
        await SetHttpClient();
        var response = await Client.GetAsync(requestUri);
        if (response.StatusCode == HttpStatusCode.ServiceUnavailable ||
            response.StatusCode == HttpStatusCode.GatewayTimeout)
        {
            response = await Policy.RetryAsync(() => Client.GetAsync(requestUri), 2);
        }
        return response ?? throw new OData2PocoException("Response is null");
    }

    internal async Task<string> ReadMetaDataAsync()
    {
        string url;
        //check url is remote xml file
        if (ServiceUri.AbsoluteUri.EndsWith(".xml"))
            url = ServiceUri.AbsoluteUri;
        else
            url = ServiceUri.AbsoluteUri.TrimEnd('/') + "/$metadata";
        try
        {
            Response = await GetAsync(url);
            Response.EnsureSuccessStatusCode();
            var content = await Response.Content.ReadAsStringAsync();
            return content;
        }
        catch (Exception)
        {
            if (Response.StatusCode != HttpStatusCode.Unauthorized) throw;
            var wwwAuthenticate = Response.Headers.WwwAuthenticate.ToString();
            throw new OData2PocoException($"HTTP {Response.StatusCode} ({(int)Response.StatusCode}): {wwwAuthenticate}");
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
        _delegatingHandler?.Dispose();
        Response?.Dispose();
        Client.Dispose();
    }
    ~CustomHttpClient()
    {
        Dispose(false);
    }
}
