// Copyright (c) Mohamed Hassan & Contributors. All rights reserved. See License.md in the project root for license information.

namespace OData2Poco.Tests;
using Http;
using OData2Poco.Fake.Common;

[NonParallelizable]
public class CustomHttpClientTest : BaseTest
{
    private string _token = "secret_token";
    private string _base64 = "user:secret".ToBase64();

    private async Task AssertConnectionAsync(OdataConnectionString connection)
    {
        using var client = await CustomHttpClient.CreateAsync(connection).ConfigureAwait(false);
        var metadata = await client.ReadMetaDataAsStringAsync().ConfigureAwait(false);
        Assert.That(metadata, Is.Not.Empty);
        Assert.That(metadata, Does.StartWith("""<?xml version="1.0" encoding="UTF-8"?>""").IgnoreCase);
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
        //Assert
        await AssertConnectionAsync(connection).ConfigureAwait(false);
    }

    [Test]
    public async Task Token_Auth_CheckHttpRequestMessage_HttpGet()
    {
        //Arrange
        var connection = new OdataConnectionString
        {
            ServiceUrl = OdataService.TrippinBearer,
            Password = _token,
            Authenticate = AuthenticationType.Token
        };
        //Act
        //Assert
        await AssertConnectionAsync(connection).ConfigureAwait(false);
    }

    [Test]
    public async Task Basic_Auth_CheckHttpRequestMessage_HttpGet()
    {
        //Arrange
        var connection = new OdataConnectionString
        {
            ServiceUrl = OdataService.TrippinBasic,
            UserName = "user",
            Password = "secret",
            Authenticate = AuthenticationType.Basic,
            Domain = "localhost"
        };

        //Act       
        //Assert
        await AssertConnectionAsync(connection).ConfigureAwait(false);
    }

    [Test]
    public async Task Custom_header_test()
    {
        //Arrange
        var list = new List<string>()
        {
            // "key1:123",
             $"Authorization:Bearer {_token}"
        };

        var connection = new OdataConnectionString
        {
            ServiceUrl = OdataService.TrippinBearer,
            HttpHeader = list,
            Authenticate = AuthenticationType.None
        };

        //Act       
        //Assert
        await AssertConnectionAsync(connection).ConfigureAwait(false);
    }

    [Test]
    public async Task Custom_client_http_header_with_basic_authorization_test()
    {
        //Arrange

        var connection = new OdataConnectionString
        {
            ServiceUrl = OdataService.TrippinBasic,
            HttpHeader =
            [
                $"Authorization:Basic {_base64}",
                "key=value"
            ]
        };

        await AssertConnectionAsync(connection).ConfigureAwait(false);
    }

    [Test]
    public async Task Custom_client_http_header_with_bearer_authorization_test()
    {
        //Arrange
        var connection = new OdataConnectionString
        {
            ServiceUrl = OdataService.TrippinBearer, //  "https://localhost/weatherforecast",
            HttpHeader =
            [
                $"Authorization=Bearer {_token}"
            ]
        };

        await AssertConnectionAsync(connection).ConfigureAwait(false);
    }

    [Test]
    public async Task Custom_client_http_header_to_base64_calcualtion_test()
    {
        //Arrange
        var connection = new OdataConnectionString
        {
            ServiceUrl = OdataService.TrippinBasic,
            HttpHeader =
            [
                "Authorization:Basic {user:secret}"
            ]
        };

        await AssertConnectionAsync(connection).ConfigureAwait(false);
    }

    [Test]
    public async Task Custom_client_http_multiple_header_test()
    {
        //Arrange
        var connection = new OdataConnectionString
        {
            ServiceUrl = OdataService.TrippinBearer,
            HttpHeader =
            [
                "key1:123",
                "key2:abc",
                $"Authorization:Bearer {_token}"
            ]
        };

        await AssertConnectionAsync(connection).ConfigureAwait(false);
    }

    [Test]
    public async Task Custom_client_http_invalid_header_items_test()
    {
        //Arrange
        var connection = new OdataConnectionString
        {
            ServiceUrl = OdataService.TrippinBearer, // "https://localhost/weatherforecast",
            HttpHeader =
            [
                "key1:123",
                "key2:abc",
                "key3:",
                "xyz",
                $"Authorization:Bearer {_token}"
            ]
        };

        await AssertConnectionAsync(connection).ConfigureAwait(false);
    }

    #region Auth live Test

    [Test]
    public async Task Bearer_Auth_live_with_valid_token_test()
    {
        //Arrange
        var connection = new OdataConnectionString
        {
            ServiceUrl = OdataService.TrippinBearer,
            Password = _token,
            Authenticate = AuthenticationType.Token
        };

        await AssertConnectionAsync(connection).ConfigureAwait(false);
    }

    [Test]
    public async Task Bearer_auth_via_header_live_test()
    {
        //Arrange
        var connection = new OdataConnectionString
        {
            ServiceUrl = OdataService.TrippinBearer,
            HttpHeader =
            [
                $"Authorization : Bearer {_token}"
            ]
        };

        await AssertConnectionAsync(connection).ConfigureAwait(false);
    }

    #endregion
}
