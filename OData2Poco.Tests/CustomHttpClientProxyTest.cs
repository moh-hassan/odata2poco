// Copyright (c) Mohamed Hassan & Contributors. All rights reserved. See License.md in the project root for license information.

namespace OData2Poco.Tests;

using Fake.Common;
using Http;

[Category("proxy")]
public class CustomHttpClientProxyTest
{
    private readonly string _machineName = Environment.MachineName;
    private readonly string _proxy = "http://localhost:8888";
    private bool _isLive;

    [OneTimeSetUp]
    public void Setup()
    {
        _isLive = !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("LiveTest", EnvironmentVariableTarget.User));
    }

    [Test]
    public async Task Proxy_with_valid_credentials_should_succeed()
    {
        //test is run only on local machine with proxy running
        if (!_isLive)
        {
            Assert.Ignore("Ignore proxy test in CI");
            return;
        }

        var url = OdataService.Northwind;
        url = url.Replace("localhost", _machineName);
        var cs = new OdataConnectionString
        {
            ServiceUrl = url,
            Proxy = _proxy,
            ProxyUser = "user:password"
        };
        using var customClient = new CustomHttpClient(cs);
        var metaData = await customClient.ReadMetaDataAsync().ConfigureAwait(false);
        Assert.That(metaData, Does.StartWith("""<?xml version="1.0" encoding="UTF-8"?>"""));
    }

    [Test]
    public void Proxy_with_Invalid_credentials_should_fail()
    {
        //test is run only on local machine with proxy running
        if (!_isLive)
        {
            Assert.Ignore("Ignore proxy test in CI");
            return;
        }

        var url = OdataService.Northwind;
        url = url.Replace("localhost", _machineName);
        var cs = new OdataConnectionString
        {
            ServiceUrl = url,
            Proxy = _proxy,
            ProxyUser = "user:invalid_password"
        };
        using var customClient = new CustomHttpClient(cs);
        const string Msg = "Response status code does not indicate success: 407";
        Assert.That(
            customClient.ReadMetaDataAsync,
            Throws.Exception.TypeOf<HttpRequestException>()
            //  .With.Message.EqualTo(Msg)
            .With.Message.Contain(Msg)
            );
    }
}
