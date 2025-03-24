// Copyright (c) Mohamed Hassan & Contributors. All rights reserved. See License.md in the project root for license information.

namespace OData2Poco.Tests;

using Http;

public class CustomHttpClientTest2
{
    [Test]
    public async Task CustomHttpClient_with_metadata_compressed_should_success()
    {
        var url = OdataService.Trippin;
        var cs = new OdataConnectionString
        {
            ServiceUrl = url
        };
        using var customClient = await CustomHttpClient.CreateAsync(cs).ConfigureAwait(false);
        var response = await customClient.ReadMetaDataAsync().ConfigureAwait(false);
        var metaData = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
        metaData.Should().StartWith(@"<?xml version=""1.0"" encoding=""UTF-8""?>");
    }

    [Test]
    public async Task CustomHttpClient_with_metadata_uncompressed_should_success()
    {
        var url = OdataService.Northwind;
        var cs = new OdataConnectionString
        {
            ServiceUrl = url
        };
        using var customClient = await CustomHttpClient.CreateAsync(cs).ConfigureAwait(false);
        var response = await customClient.ReadMetaDataAsync().ConfigureAwait(false);
        var metaData = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
        metaData.Should().StartWith(@"<?xml version=""1.0"" encoding=""UTF-8""?>");
    }
}
