// Copyright (c) Mohamed Hassan & Contributors. All rights reserved. See License.md in the project root for license information.

namespace OData2Poco.Tests;

using Http;

[Category("proxy")]
public class CustomHttpClientProxyTest
{
    private int _port = 8888;
    private string _proxy => $"http://localhost:{_port}";
    private string _machineName = Environment.MachineName;

    private PocoSetting ps = new PocoSetting();

    [OneTimeSetUp]
    public void Setup()
    {
        if (PortChecker.IsPortInUse(_port, out var name) && name == "mitmdump")
            return;
        Assert.Ignore("Ignore proxy test. mitmdump is not running");
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
        url = url.Replace("localhost", _machineName); //proxy bypass localhost in net472
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
            Throws.Exception.TypeOf<OData2PocoException>()
                .With.Message.Contain(Msg)
        );
    }
}
