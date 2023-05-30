// Copyright (c) Mohamed Hassan & Contributors. All rights reserved. See License.md in the project root for license information.

using System.Net;
using FluentAssertions;
using NUnit.Framework;
using OData2Poco.Extensions;
using OData2Poco.Fake;
using OData2Poco.Http;

namespace OData2Poco.Tests;

internal class CustomHttpClientTest : BaseTest
{
    private bool _isLive;
    private string _token;
    private string _url ;

    [OneTimeSetUp]
    public void Setup()
    {
        _isLive = !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("LiveTest", EnvironmentVariableTarget.User));
        _token = Environment.GetEnvironmentVariable("Token", EnvironmentVariableTarget.User);
        _url = "https://localhost/odata/odata";
    }

    [Test]
    public async Task No_auth_ReadMetaDataTest()
    {
        //Arrange
        string url = TestSample.UrlTripPinService;
        var connection = new OdataConnectionString
        {
            ServiceUrl = url,
            Authenticate = AuthenticationType.None,
        };
        //Act
        var cc = new CustomHttpClient(connection);
        var metadata = await cc.ReadMetaDataAsync();
        //Assert
        Assert.That(metadata.Length, Is.GreaterThan(0));
    }

    [Test]
    public async Task Token_Auth_CheckHttpRequestMessage_HttpGet()
    {
        //Arrange
        var connection = new OdataConnectionString
        {
            ServiceUrl = "http://localhost/odata/odata",
            Password = "accessToken",
            Authenticate = AuthenticationType.Token

        };
        //Act
        var ah = new AuthHandler();
        var client = new CustomHttpClient(connection, ah);
        await client.GetAsync(client.ServiceUri.ToString());
        //Assert
        ah.AuthHeader.Should().NotBeNull();
        ah.Scheme?.Should().Be("Bearer");
        ah.Parameter?.Should().Be("accessToken");
        ah.AuthHeader?.ToString().Should().Be("Bearer accessToken");
    }

    [Test]
    public async Task Basic_Auth_CheckHttpRequestMessage_HttpGet()
    {
        //Arrange
        var connection = new OdataConnectionString
        {
            ServiceUrl = "http://localhost/odata2/api/northwind",
            UserName = "user1",
            Password = "secret",
            Authenticate = AuthenticationType.Basic,
            Domain = "localhost",
        };
        //Act
        //Assert
        var client = new CustomHttpClient(connection, new CustomeHandler(r =>
        {
            r.RequestUri?.ToString().Should().Be("http://localhost/odata2/api/northwind/$metadata");
            r.Headers.UserAgent.Should().NotBeNull();
            r.Headers.UserAgent.ToString().Should().Be("OData2Poco");
            r.Headers.Authorization.Should().NotBeNull();
            r.Headers?.Authorization?.ToString().Should().Be("Basic dXNlcjE6c2VjcmV0");
        }));
        await client.ReadMetaDataAsync();
    }
    [Test]
    public async Task Custom_header_test()
    {
        //Arrange
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
        //Act
        //Assert
        var client = new CustomHttpClient(connection, new CustomeHandler(r =>
        {
            r.Headers.Count().Should().Be(3);
            Assert.IsNotNull(r.Headers.Authorization);
            Assert.AreEqual("Bearer abc.123", r.Headers.Authorization.ToString());
        }));
        await client.ReadMetaDataAsync();
    }

    [Test]
    public async Task Custom_client_http_header_with_basic_authorization_test()
    {
        //Arrange
        var ocs = new OdataConnectionString
        {
            ServiceUrl = "https://localhost/weatherforecast",
            HttpHeader = new List<string>
            {
                "Authorization=Basic YWRtaW46YWRtaW4="
            }
        };
        var ah = new AuthHandler();
        var sut = ocs.ToCustomHttpClient(ah);
        //Act
        await sut.GetAsync(ocs.ServiceUrl);
        //Assert
        ah.AuthHeader.Should().NotBeNull();
        ah.AuthHeader?.ToString().Should().Be("Basic YWRtaW46YWRtaW4=");
        ah.Scheme.Should().Be("Basic");
        ah.Parameter.Should().Be("YWRtaW46YWRtaW4=");
    }

    #region Header Test

    [Test]
    public async Task Custom_client_http_header_with_bearer_authorization_test()
    {
        //Arrange
        var ocs = new OdataConnectionString
        {
            ServiceUrl = "https://localhost/weatherforecast",
            HttpHeader = new List<string>
            {
                "Authorization=Bearer secret$token"
            }
        };
        var ah = new AuthHandler();
        var sut = ocs.ToCustomHttpClient(ah);
        //Act
        await sut.GetAsync(ocs.ServiceUrl);
        //Assert
        ah.AuthHeader.Should().NotBeNull();
        ah.AuthHeader?.ToString().Should().Be("Bearer secret$token");
        ah.Scheme.Should().Be("Bearer");
        ah.Parameter.Should().Be("secret$token");
    }

    [Test]
    public async Task Custom_client_http_header_to_base64_calcualtion_test()
    {
        //Arrange
        var ocs = new OdataConnectionString
        {
            ServiceUrl = "https://localhost/weatherforecast",
            HttpHeader = new List<string>
            {
                "Authorization:Basic {user1:password1}"
            }
        };
        var ah = new AuthHandler();
        var sut = ocs.ToCustomHttpClient(ah);
        //Act
        await sut.GetAsync(ocs.ServiceUrl);
        //Assert
        ah.AuthHeader.Should().NotBeNull();
        ah.AuthHeader?.ToString().Should().Be("Basic dXNlcjE6cGFzc3dvcmQx");
        ah.Scheme.Should().Be("Basic");
        ah.Parameter.Should().Be("dXNlcjE6cGFzc3dvcmQx");
    }
    [Test]
    public async Task Custom_client_http_multiple_header_test()
    {
        //Arrange
        var ocs = new OdataConnectionString
        {
            ServiceUrl = "https://localhost/weatherforecast",
            HttpHeader = new List<string>
            {
                "key1:123",
                "key2:abc",
                "Authorization:Bearer secret$token"
            }
        };

        var ah = new AuthHandler();
        var sut = ocs.ToCustomHttpClient(ah);
        //Act
        await sut.GetAsync(ocs.ServiceUrl);
        //Assert
        ah.AuthHeader.Should().NotBeNull();
        ah.AuthHeader?.ToString().Should().Be("Bearer secret$token");
        ah.Request?.Headers.Should().Contain(x => x.Key == "key1" && x.Value.Contains("123"));
        ah.Request?.Headers.Should().Contain(x => x.Key == "key2" && x.Value.Contains("abc"));
        ah.Request?.Headers.Should().Contain(x => x.Key == "Authorization" && x.Value.Contains("Bearer secret$token"));
    }

    [Test]
    public async Task Custom_client_http_invalid_header_items_test()
    {
        //Arrange
        var ocs = new OdataConnectionString
        {
            ServiceUrl = "https://localhost/weatherforecast",
            HttpHeader = new List<string>
            {
                "key1:123",
                "key2:abc",
                "key3:",
                "xyz",
                "Authorization:Bearer secret+token"
            }
        };

        var ah = new AuthHandler();
        var sut = ocs.ToCustomHttpClient(ah);
        //Act
        await sut.GetAsync(ocs.ServiceUrl);
        //Assert
        ah.AuthHeader.Should().NotBeNull();
        ah.AuthHeader?.ToString().Should().Be("Bearer secret+token");
        ah.Request?.Headers.Should().Contain(x => x.Key == "key1" && x.Value.Contains("123"));
        ah.Request?.Headers.Should().Contain(x => x.Key == "key2" && x.Value.Contains("abc"));
        ah.Request?.Headers.Should().Contain(x => x.Key == "Authorization" && x.Value.Contains("Bearer secret+token"));
        ah.Request?.Headers.Should().Contain(x => x.Key == "key3" && x.Value.Contains(""));

    }
    #endregion

    #region Auth live Test

    [Test]
    [Category("live")]
    public async Task Bearer_Auth_live_with_valid_token_test()
    {
        if (!_isLive)
            Assert.Ignore("live test");

        //Arrange
        var ocs = new OdataConnectionString
        {
            ServiceUrl = _url,
            Password = _token,
            Authenticate = AuthenticationType.Token,
        };
        var ah = new AuthHandler(true);
        var client = ocs.ToCustomHttpClient(ah);
        //Act
        await client.GetAsync(client.ServiceUri.ToString());
        //Assert
        var expectedToken = ocs.Password.GetToken();
        ah.AuthHeader.Should().NotBeNull();
        // ah.AuthHeader?.ToString().Should().Be("Bearer xyz");
        ah.Scheme?.Should().Be("Bearer");
        ah.Parameter?.Should().Be(expectedToken);
        ah.Response?.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Test]
    [Category("live")]
    public async Task Bearer_Auth_live_with_inValid_token_test()
    {
        if (!_isLive)
            Assert.Ignore("live test");
        //Arrange
        var ocs = new OdataConnectionString
        {
            ServiceUrl = _url,
            Password = "xyz",
            Authenticate = AuthenticationType.Token,
        };
        var ah = new AuthHandler(true);
        var client = ocs.ToCustomHttpClient(ah);

        //Act
        var act = async () => await client.ReadMetaDataAsync();
        //Assert
        await act.Should().ThrowAsync<OData2PocoException>()
            .WithMessage(@"HTTP Unauthorized (401): Bearer error=""invalid_token""");
    }

    [Test]
    [Category("live")]
    public async Task Bearer_auth_via_header_live_test()
    {
        if (!_isLive)
            Assert.Ignore("live test");

        //Arrange
        var ocs = new OdataConnectionString
        {
            ServiceUrl = _url,
            HttpHeader = new List<string> { $"Authorization : Bearer {_token}" },
        };
        var ah = new AuthHandler(true);
        var client = ocs.ToCustomHttpClient(ah);
        //Act
        await client.ReadMetaDataAsync();
        //Assert
        ah.AuthHeader.Should().NotBeNull();
        ah.AuthHeader?.ToString().Should().Be($"Bearer {_token}");
        ah.Scheme?.Should().Be("Bearer");
        ah.Parameter?.Should().Be(_token);
        ah.Response?.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    #endregion

}
