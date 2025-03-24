namespace OData2Poco.Tests;

using NUnit.Framework;
using OData2Poco.Http;
using System;
using System.Net;
using System.Net.Http.Headers;
using static OData2Poco.Fake.TestCaseSources;

[Category("OdataService")]
public class OdataServiceTest
{
    private HttpClient CreateClient()
    {
        var handler = new HttpClientHandler()
        {
            AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
        };
        return new HttpClient(handler);
    }

    [Test]
    public void Insure_valid_service_url()
    {
        OdataService.Instance.ShowWireMockServerInfo();
        Assert.Multiple(() =>
        {
            Assert.That(OdataService.Trippin, Is.Not.Empty);
            Assert.That(OdataService.TrippinBasic, Is.Not.Empty);
            Assert.That(OdataService.TrippinBearer, Is.Not.Empty);
            Assert.That(OdataService.Northwind, Is.Not.Empty);
            Assert.That(OdataService.Northwind2, Is.Not.Empty);
            Assert.That(OdataService.Northwind3, Is.Not.Empty);
            Assert.That(OdataService.Northwind4, Is.Not.Empty);
        });
    }

    [Test]
    public async Task TrippinBasic_ShouldReturn_ok_WithBasicAuthAsync()
    {
        var url = new Uri($@"{OdataService.TrippinBasic}\$metadata");
        using var client = CreateClient();
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Basic", "user:secret".ToBase64());
        var response = await client.GetAsync(url).ConfigureAwait(false);
        var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
        Assert.Multiple(() =>
        {
            Assert.That(response.IsSuccessStatusCode, Is.True);
            Assert.That(content, Does.StartWith("""<?xml version="1.0" encoding="UTF-8"?>""").IgnoreCase);
        });
    }

    [Test]
    public async Task TrippinBearer_ShouldReturn_ok_with_BearerAuthAsync()
    {
        using var client = CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "secret_token");
        var url = new Uri($@"{OdataService.TrippinBearer}\$metadata");
        var response = await client.GetAsync(url).ConfigureAwait(false);
        var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
        Assert.Multiple(() =>
        {
            Assert.That(response.IsSuccessStatusCode, Is.True);
            Assert.That(content, Does.StartWith("""<?xml version="1.0" encoding="UTF-8"?>""").IgnoreCase);
        });
    }

    [Test]
    public async Task Trippin_ShouldReturn_ok_WithNo_AuthAsync()
    {
        using var client = CreateClient();

        var url = new Uri($@"{OdataService.Trippin}\$metadata");
        var response = await client.GetAsync(url).ConfigureAwait(false);
        var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
        Assert.Multiple(() =>
        {
            Assert.That(response.IsSuccessStatusCode, Is.True);
            Assert.That(content, Does.StartWith("""<?xml version="1.0" encoding="UTF-8"?>""").IgnoreCase);
        });
    }

    [Test]
    [TestCaseSource(typeof(TestCaseSources), nameof(TestMockCases))]
    public async Task No_auth_service_should_work(string path, string version)
    {
        using var client = CreateClient();

        var url = new Uri($@"{path}\$metadata");
        var response = await client.GetAsync(url).ConfigureAwait(false);
        var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
        Assert.Multiple(() =>
        {
            Assert.That(response.IsSuccessStatusCode, Is.True);
            Assert.That(content, Does.Contain("EntityType").IgnoreCase);
        });
    }

    [Test]
    [TestCaseSource(typeof(TestCaseSources), nameof(TestMockCases))]
    public async Task OdataService_mock_test(string url, string version)
    {
        var connString = new OdataConnectionString
        {
            ServiceUrl = url,
            Authenticate = AuthenticationType.None
        };

        var client = await CustomHttpClient.CreateAsync(connString)
            .ConfigureAwait(false);
        var response = await client.ReadMetaDataAsync().ConfigureAwait(false);
        var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
        Assert.Multiple(() =>
        {
            Assert.That(response.IsSuccessStatusCode, Is.True);
            Assert.That(content, Does.Contain("EntityType").IgnoreCase);
        });
    }

    [Test]
    public async Task OdataService_mock_test2()
    {
        var url = OdataService.Northwind4;
        var connString = new OdataConnectionString
        {
            ServiceUrl = url,
            Authenticate = AuthenticationType.None
        };

        var client = await CustomHttpClient.CreateAsync(connString)
            .ConfigureAwait(false);
        var response = await client.ReadMetaDataAsync().ConfigureAwait(false);
        Assert.That(response.IsSuccessStatusCode, Is.True);
    }
}
