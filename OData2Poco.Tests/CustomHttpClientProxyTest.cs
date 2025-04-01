// Copyright (c) Mohamed Hassan & Contributors. All rights reserved. See License.md in the project root for license information.

namespace OData2Poco.Tests;

using System.Net;
using Http;

[Category("proxy")]
public class CustomHttpClientProxyTest
{
    private readonly string _machineName = Environment.MachineName;
    private readonly string _proxy = "http://localhost:8888";
    private PocoSetting ps = new PocoSetting();

    [OneTimeSetUp]
    public async Task Setup()
    {
        if (!await IsProxyAvailable().ConfigureAwait(false))
            Assert.Ignore("Ignore proxy test in CI");
    }

    [Test]
    public async Task Proxy_with_valid_credentials_should_succeed()
    {
        var url = OdataService.Northwind;
        url = url.Replace("localhost", _machineName);
        var cs = new OdataConnectionString
        {
            ServiceUrl = url,
            Proxy = _proxy,
            ProxyUser = "user:password"
        };
        using var customClient = await CustomHttpClient
            .CreateAsync(cs, ps)
            .ConfigureAwait(false);
        var metaData = await customClient.ReadMetaDataAsStringAsync().ConfigureAwait(false);
        Assert.That(metaData, Does.StartWith("""<?xml version="1.0" encoding="UTF-8"?>"""));
    }

    [Test]
    public void Proxy_with_Invalid_credentials_should_fail()
    {
        var url = OdataService.Northwind;
        url = url.Replace("localhost", _machineName);
        var cs = new OdataConnectionString
        {
            ServiceUrl = url,
            Proxy = _proxy,
            ProxyUser = "user:invalid_password"
        };
        using var customClient = CustomHttpClient.CreateAsync(cs, ps).GetAwaiter().GetResult();
        const string Msg = "Response status code does not indicate success: 407";
        Assert.That(
            customClient.ReadMetaDataAsync,
            Throws.Exception.TypeOf<HttpRequestException>()
                .With.Message.Contain(Msg)
        );
    }

    private async Task<bool> IsProxyAvailable()
    {
        var handler = new HttpClientHandler
        {
            Proxy = new WebProxy("localhost", 8888), // Fiddler's default settings
            UseProxy = true
        };
        handler.Proxy.Credentials = new NetworkCredential("user", "password");
        using var httpClient = new HttpClient(handler);
        var url = "http://www.example.com";

        try
        {
            // Send an HTTP HEAD request
            var request = new HttpRequestMessage(HttpMethod.Head, url);
            var response = await httpClient.SendAsync(request).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();
            return true;
        }
        catch
        {
            return false;
        }
    }
}
