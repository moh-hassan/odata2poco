// Copyright (c) Mohamed Hassan & Contributors. All rights reserved. See License.md in the project root for license information.

#pragma warning disable S4830

using System.Net;
using System.Net.Http;
using OData2Poco.InfraStructure.Logging;

namespace OData2Poco.Http;

internal class CustomHttpClient : IDisposable
{
    private static readonly ILog Logger = PocoLogger.Default;
    private readonly DelegatingHandler? _delegatingHandler;
    internal readonly OdataConnectionString _odataConnectionString;
    internal HttpClient _client;
    internal HttpClientHandler handler;
    public HttpResponseMessage? Response;
    public Uri ServiceUri { get; set; }

    public CustomHttpClient(OdataConnectionString odataConnectionString)
    {
        _odataConnectionString = odataConnectionString;
        ServiceUri = new Uri(_odataConnectionString.ServiceUrl);
        handler = new HttpClientHandler
        {
            UseDefaultCredentials = true,
            AllowAutoRedirect = true,
        };
        _client = new HttpClient(handler);
    }

    public CustomHttpClient(OdataConnectionString odataConnectionString,
        DelegatingHandler dh) : this(odataConnectionString)
    {
        _delegatingHandler = dh;
        _delegatingHandler.InnerHandler = handler;
        _client = new HttpClient(_delegatingHandler);
    }

    private void SetupHeader()
    {
        if (_odataConnectionString.HttpHeader == null || !_odataConnectionString.HttpHeader.Any()) return;
        foreach (var header in _odataConnectionString.HttpHeader)
        {
            var pair = header.Split('=');
            if (pair.Length == 2)
            {
                var key = pair[0].Trim().Trim('"');
                var value = pair[1].Trim().Trim('"');
                _client.DefaultRequestHeaders.Add(key, value);
            }
        }
    }

    private async Task SetHttpClient()
    {
        //setup SkipCertificationCheck.
        //Use in development environment, but is not recommended in production 
        if (_odataConnectionString.SkipCertificationCheck)
        {
            handler.ServerCertificateCustomValidationCallback = (_, _, _, _) => true;
            Logger.Warn("Skip Certification Check is set to true. This is not recommended in production environment");
        }

        //setup SecurityProtocol
        if (_odataConnectionString.TlsProtocol > 0)
        {
            ServicePointManager.SecurityProtocol |= _odataConnectionString.TlsProtocol;
            Logger.Info($"Setting the SSL/TLS protocols to: {_odataConnectionString.TlsProtocol}");
        }

        if (!string.IsNullOrEmpty(_odataConnectionString.Proxy))
        {
            Logger.Trace($"Using Proxy: '{_odataConnectionString.Proxy}'");
            handler.UseProxy = true;
            handler.Proxy = new WebProxy(_odataConnectionString.Proxy);
        }

        _client.DefaultRequestHeaders
            .TryAddWithoutValidation("Content-Type", "application/json; charset=utf-8");
        var agent = "OData2Poco";
        _client.DefaultRequestHeaders.Add("User-Agent", agent);
        SetupHeader();

        if (_odataConnectionString.Authenticate != AuthenticationType.None)
        {
            Authenticator auth = new(this);
            await auth.Authenticate();
        }
    }
    internal virtual async Task<HttpResponseMessage> GetAsync(string? requestUri)
    {
        await SetHttpClient();
        return await _client.GetAsync(requestUri);
    }

    internal async Task<string> ReadMetaDataAsync()
    {
        string url;
        //check url is remote xml file
        if (ServiceUri.AbsoluteUri.EndsWith(".xml"))
            url = ServiceUri.AbsoluteUri;
        else
            url = ServiceUri.AbsoluteUri.TrimEnd('/') + "/$metadata";

        Response = await GetAsync(url);
        Response.EnsureSuccessStatusCode();

        if (Response is { IsSuccessStatusCode: false })
            throw new HttpRequestException(
                $"Http Error {(int)Response.StatusCode}: {Response.ReasonPhrase}");

        var content = await Response.Content.ReadAsStringAsync();
        if (string.IsNullOrEmpty(content))
            throw new HttpRequestException(
                $"Http Error {(int)Response.StatusCode}: {Response.ReasonPhrase}");
        return content;
    }
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
    protected virtual void Dispose(bool disposing)
    {
        _odataConnectionString.Password.Dispose();
        _delegatingHandler?.Dispose();
        Response?.Dispose();
        _client.Dispose();
    }
    ~CustomHttpClient()
    {
        Dispose(false);
    }
}
