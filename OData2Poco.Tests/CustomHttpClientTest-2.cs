// Copyright (c) Mohamed Hassan & Contributors. All rights reserved. See License.md in the project root for license information.

using NUnit.Framework;
using FluentAssertions;
using OData2Poco.Http;

namespace OData2Poco.Tests;

public class CustomHttpClientTest2
{
    [Test]
    public async Task CustomHttpClient_with_metadata_compressed_should_success()
    {       
        string url = OdataService.Trippin;      
        OdataConnectionString cs = new OdataConnectionString
        {
            ServiceUrl = url,          
        };
        var customClient = new CustomHttpClient(cs);
        var metaData = await customClient.ReadMetaDataAsync();
        metaData.Should().StartWith(@"<?xml version=""1.0"" encoding=""UTF-8""?>");
    }
    [Test]
    public async Task CustomHttpClient_with_metadata_uncompressed_should_success()
    {
        string url = OdataService.Northwind;
        OdataConnectionString cs = new OdataConnectionString
        {
            ServiceUrl = url           
        };
        var customClient = new CustomHttpClient(cs);
        var metaData = await customClient.ReadMetaDataAsync();
        metaData.Should().StartWith(@"<?xml version=""1.0"" encoding=""UTF-8""?>");
    }
}
