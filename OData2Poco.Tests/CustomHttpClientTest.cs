// Copyright (c) Mohamed Hassan & Contributors. All rights reserved. See License.md in the project root for license information.

namespace OData2Poco.Tests;

using System.Net;
using Http;

public class CustomHttpClientTest : BaseTest
{
    private bool _isLive;
    private string _token;
    private string _url;

    [OneTimeSetUp]
    public void Setup()
    {
        _isLive = !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("LiveTest", EnvironmentVariableTarget.User));
        //token is updated periodically per month
        _token = Environment.GetEnvironmentVariable("Token", EnvironmentVariableTarget.User);
        _url = "https://localhost/odata/odata";
    }

    [Test]
    public async Task No_auth_ReadMetaDataTest()
    {
        //Arrange
        var url = TestSample.UrlTripPinService;
        var connection = new OdataConnectionString
        {
            ServiceUrl = url,
            Authenticate = AuthenticationType.None
        };
        //Act
        using var cc = new CustomHttpClient(connection);
        var metadata = await cc.ReadMetaDataAsync().ConfigureAwait(false);
        //Assert
        Assert.That(metadata, Is.Not.Empty);
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
        using var ah = new AuthHandler();
        using var client = new CustomHttpClient(connection, ah);
        await client.GetAsync(client.ServiceUri.ToString()).ConfigureAwait(false);
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
            Domain = "localhost"
        };
        //Act
        //Assert
        using var delegatingHandler = new CustomeHandler(r =>
        {
            r.RequestUri?.ToString().Should().Be("http://localhost/odata2/api/northwind/$metadata");
            r.Headers.UserAgent.Should().NotBeNull();
            r.Headers.UserAgent.ToString().Should().Contain("OData2Poco");
            r.Headers.Authorization.Should().NotBeNull();
            r.Headers.Authorization?.ToString().Should().Be("Basic dXNlcjE6c2VjcmV0");
        });
        using var client = new CustomHttpClient(connection, delegatingHandler);
        await client.ReadMetaDataAsync().ConfigureAwait(false);
    }

    [Test]
    public async Task Custom_header_test()
    {
        //Arrange
        var list = new List<string>()
        {
            "ky1=123", "Authorization=Bearer abc.123"
        };

        var connection = new OdataConnectionString
        {
            ServiceUrl = "https://localhost/odata/v1",
            HttpHeader = list
        };
        //Act
        //Assert
        using var delegatingHandler = new CustomeHandler(r =>
        {
            r.Headers.Count().Should().Be(3);
            Assert.That(r.Headers.Authorization, Is.Not.Null);
            Assert.That(r.Headers.Authorization.ToString(), Is.EqualTo("Bearer abc.123"));
        });
        using var client = new CustomHttpClient(connection, delegatingHandler);
        await client.ReadMetaDataAsync().ConfigureAwait(false);
    }

    [Test]
    public async Task Custom_client_http_header_with_basic_authorization_test()
    {
        //Arrange
        var ocs = new OdataConnectionString
        {
            ServiceUrl = "https://localhost/weatherforecast",
            HttpHeader =
            [
                "Authorization=Basic YWRtaW46YWRtaW4="
            ]
        };
        using var ah = new AuthHandler();
        var sut = ocs.ToCustomHttpClient(ah);
        //Act
        await sut.GetAsync(ocs.ServiceUrl).ConfigureAwait(false);
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
            HttpHeader =
            [
                "Authorization=Bearer secret$token"
            ]
        };
        using var ah = new AuthHandler();
        using var sut = ocs.ToCustomHttpClient(ah);
        //Act
        await sut.GetAsync(ocs.ServiceUrl).ConfigureAwait(false);
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
            HttpHeader =
            [
                "Authorization:Basic {user1:password1}"
            ]
        };
        using var ah = new AuthHandler();
        using var sut = ocs.ToCustomHttpClient(ah);
        //Act
        await sut.GetAsync(ocs.ServiceUrl).ConfigureAwait(false);
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
            HttpHeader =
            [
                "key1:123",
                "key2:abc",
                "Authorization:Bearer secret$token"
            ]
        };

        using var ah = new AuthHandler();
        using var sut = ocs.ToCustomHttpClient(ah);
        //Act
        await sut.GetAsync(ocs.ServiceUrl).ConfigureAwait(false);
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
            HttpHeader =
            [
                "key1:123",
                "key2:abc",
                "key3:",
                "xyz",
                "Authorization:Bearer secret+token"
            ]
        };

        using var ah = new AuthHandler();
        using var sut = ocs.ToCustomHttpClient(ah);
        //Act
        await sut.GetAsync(ocs.ServiceUrl).ConfigureAwait(false);
        //Assert
        ah.AuthHeader.Should().NotBeNull();
        ah.AuthHeader?.ToString().Should().Be("Bearer secret+token");
        ah.Request?.Headers.Should().Contain(x => x.Key == "key1" && x.Value.Contains("123"));
        ah.Request?.Headers.Should().Contain(x => x.Key == "key2" && x.Value.Contains("abc"));
        ah.Request?.Headers.Should().Contain(x => x.Key == "Authorization" && x.Value.Contains("Bearer secret+token"));
        ah.Request?.Headers.Should().Contain(x => x.Key == "key3" && x.Value.Contains(string.Empty));
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
            Authenticate = AuthenticationType.Token
        };
        using var ah = new AuthHandler(true);
        using var client = ocs.ToCustomHttpClient(ah);
        //Act
        await client.GetAsync(client.ServiceUri.ToString()).ConfigureAwait(false);
        //Assert
        var expectedToken = ocs.Password.GetToken();
        ah.AuthHeader.Should().NotBeNull();
        ah.Scheme?.Should().Be("Bearer");
        ah.Parameter?.Should().Be(expectedToken);
        ah.Response?.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Test]
    [Category("live")]
    public async Task Bearer_Auth_live_with_inValid_token_test()
    {
        if (!_isLive) Assert.Ignore("live test");
        //Arrange
        var ocs = new OdataConnectionString
        {
            ServiceUrl = _url,
            Password = "xyz",
            Authenticate = AuthenticationType.Token
        };
        using var ah = new AuthHandler(true);
        using var client = ocs.ToCustomHttpClient(ah);

        //Act
        var act = client.ReadMetaDataAsync;
        //Assert
        await act.Should().ThrowAsync<OData2PocoException>()
            .WithMessage("""
            Request failed with status code (401) Unauthorized.
            www-authenticate: Bearer error="invalid_token"
            """.Trim())
            .ConfigureAwait(false);
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
            HttpHeader =
            [
                $"Authorization : Bearer {_token}"
            ]
        };
        using var ah = new AuthHandler(true);
        using var client = ocs.ToCustomHttpClient(ah);
        //Act
        await client.ReadMetaDataAsync().ConfigureAwait(false);
        //Assert
        ah.AuthHeader.Should().NotBeNull();
        ah.AuthHeader?.ToString().Should().Be($"Bearer {_token}");
        ah.Scheme?.Should().Be("Bearer");
        ah.Parameter?.Should().Be(_token);
        ah.Response?.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    #endregion
}
