// Copyright (c) Mohamed Hassan & Contributors. All rights reserved. See License.md in the project root for license information.

using NUnit.Framework;
using FluentAssertions;
using OData2Poco.Http;

namespace OData2Poco.Tests;

public partial class CustomHttpClientTest
{
    [Test]
    public async Task CustomHttpClient_auto_decompress_gzip_data()
    {
        string url = new GzipMockServer();
        OdataConnectionString cs = new OdataConnectionString
        {
            ServiceUrl = url,
            Authenticate = AuthenticationType.None,
        };
        var customClient = new CustomHttpClient(cs);
        var metaData = await customClient.ReadMetaDataAsync();
        metaData.Should().StartWith(@"<?xml version=""1.0"" encoding=""UTF-8""?>");
    }
}
