// Copyright (c) Mohamed Hassan & Contributors. All rights reserved. See License.md in the project root for license information.

namespace OData2Poco;

using Http;

internal static class MetaDataReader
{
    public static async Task<MetaDataInfo> LoadMetaDataHttpAsync(OdataConnectionString odataConnString)
    {
        using var client = await CustomHttpClient.CreateAsync(odataConnString).ConfigureAwait(false);
        var response = await client.ReadMetaDataAsync().ConfigureAwait(false);
        if (response == null)
            return new MetaDataInfo();

        var content = await (response.Content.ReadAsStringAsync()).ConfigureAwait(false);

        var metaData = new MetaDataInfo
        {
            MetaDataAsString = content,
            MetaDataVersion = Helper.GetMetadataVersion(content),
            ServiceUrl = client.ServiceUri.OriginalString,
            SchemaNamespace = Helper.GetNameSpace(content),
            MediaType = Media.Http
        };

        foreach (var entry in response.Headers)
        {
            var value = entry.Value.FirstOrDefault();
            if (value == null) continue;
            var key = entry.Key;
            metaData.ServiceHeader.Add(key, value);
        }

        metaData.ServiceVersion = Helper.GetServiceVersion(metaData.ServiceHeader);
        return metaData;
    }

    /// <summary>
    ///     Load Metadata from xml string
    /// </summary>
    /// <param name="xmlContent">xml string </param>
    /// <returns></returns>
    public static MetaDataInfo LoadMetaDataFromXml(string xmlContent)
    {
        var metaData = new MetaDataInfo
        {
            MetaDataAsString = xmlContent,
            MetaDataVersion = Helper.GetMetadataVersion(xmlContent),
            ServiceUrl = string.Empty,
            SchemaNamespace = Helper.GetNameSpace(xmlContent),
            MediaType = Media.Xml
        };
        return metaData;
    }

    public static async Task<MetaDataInfo> LoadMetadataAsync(OdataConnectionString odataConnString)
    {
        MetaDataInfo metaData;
        if (!odataConnString.ServiceUrl.StartsWith("http"))
        {
            using StreamReader reader = new(odataConnString.ServiceUrl);
            var xml = await reader.ReadToEndAsync().ConfigureAwait(false);
            metaData = LoadMetaDataFromXml(xml);
            metaData.ServiceUrl = odataConnString.ServiceUrl;
            return metaData;
        }

        metaData = await LoadMetaDataHttpAsync(odataConnString).ConfigureAwait(false);
        return metaData;
    }
}
