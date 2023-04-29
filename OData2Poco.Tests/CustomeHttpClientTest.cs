// Copyright (c) Mohamed Hassan & Contributors. All rights reserved. See License.md in the project root for license information.

using FluentAssertions;
using NUnit.Framework;
using OData2Poco.Fake;
using OData2Poco.Http;

namespace OData2Poco.Tests;

internal class CustomeHttpClientTest
{
    [Test]
    public async Task No_auth_ReadMetaDataTest()
    {
        string url = TestSample.UrlTripPinService;
        var connection = new OdataConnectionString
        {
            ServiceUrl = url,
            Authenticate = AuthenticationType.None,
        };
        var cc = new CustomHttpClient(connection);
        var metadata = await cc.ReadMetaDataAsync();
        Assert.That(metadata.Length, Is.GreaterThan(0));
    }

    [Test]
    public async Task Token_Auth_CheckHttpRequestMessage_HttpGet()
    {
        var connection = new OdataConnectionString
        {

            ServiceUrl = "http://localhost/odata2/api/northwind",
            Password = "accessToken",
            Authenticate = AuthenticationType.Token


        };
        var client = new CustomHttpClient(connection, new CustomeHandler(r =>
        {
            Assert.AreEqual("http://localhost/odata2/api/northwind/$metadata",
                r.RequestUri?.ToString());

            Assert.IsNotNull(r.Headers.UserAgent);
            Assert.AreEqual("OData2Poco", r.Headers.UserAgent.ToString());

            Assert.IsNotNull(r.Headers.Authorization);
            Assert.AreEqual("Bearer accessToken", r.Headers.Authorization.ToString());
        }));
        await client.ReadMetaDataAsync();
    }

    [Test]
    public async Task Basic_Auth_CheckHttpRequestMessage_HttpGet()
    {
        var connection = new OdataConnectionString
        {
            ServiceUrl = "http://localhost/odata2/api/northwind",
            UserName = "user1",
            Password = "secret",
            Authenticate = AuthenticationType.Basic
        };
        var client = new CustomHttpClient(connection, new CustomeHandler(r =>
        {
            Assert.AreEqual("http://localhost/odata2/api/northwind/$metadata",
                r.RequestUri?.ToString());
            Assert.IsNotNull(r.Headers.UserAgent);
            Assert.AreEqual("OData2Poco", r.Headers.UserAgent.ToString());
            Assert.IsNotNull(r.Headers.Authorization);
            Assert.AreEqual("Basic dXNlcjE6c2VjcmV0", r.Headers.Authorization.ToString());
        }));
        await client.ReadMetaDataAsync();
    }
    [Test]
    public async Task Custom_header_test()
    {
        var list = new List<string>()
         {
             "ky1=123",
             "Authorization=Bearer abc.123"
        };

        var connection = new OdataConnectionString
        {
            ServiceUrl = "https://localhost/odata/v1",
            HttpHeader = list,
        };
        var client = new CustomHttpClient(connection, new CustomeHandler(r =>
        {
            Console.WriteLine(r.Headers.Count());
            r.Headers.Count().Should().Be(3);
            Assert.IsNotNull(r.Headers.Authorization);
            Assert.AreEqual("Bearer abc.123", r.Headers.Authorization.ToString());
        }));
        await client.ReadMetaDataAsync();

    }
}