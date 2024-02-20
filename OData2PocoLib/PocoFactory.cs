// Copyright (c) Mohamed Hassan & Contributors. All rights reserved. See License.md in the project root for license information.

namespace OData2Poco;

using V4;

// factory class
internal static class PocoFactory
{
    /// <summary>
    ///     Generate Poco Model List<ClassTemplate/>
    /// </summary>
    /// <param name="metadata"></param>
    /// <param name="setting"></param>
    /// <returns></returns>
    internal static IPocoGenerator Create(MetaDataInfo metadata, PocoSetting setting)
    {
        if (string.IsNullOrEmpty(metadata.MetaDataAsString))
        {
            throw new InvalidOperationException("No Metadata available");
        }

        var metaDataVersion = metadata.MetaDataVersion;
        return metaDataVersion switch
        {
            ODataVersion.V4 => new Poco(metadata, setting),
            ODataVersion.V1 => new V3.Poco(metadata, setting),
            ODataVersion.V2 => new V3.Poco(metadata, setting),
            ODataVersion.V3 => new V3.Poco(metadata, setting),
            _ => throw new NotSupportedException($"OData Version '{metaDataVersion}' is not supported")
        };
    }

    internal static async Task<IPocoGenerator> GenerateModel(OdataConnectionString connectionString,
        PocoSetting setting)
    {
        var metaData = await MetaDataReader.LoadMetadataAsync(connectionString).ConfigureAwait(false);
        var generator = Create(metaData, setting);
        return generator;
    }

    internal static async Task<IPocoGenerator> GenerateModelAsync(string xmlContents,
        PocoSetting setting)
    {
        var metaData = await Task.Run(() => MetaDataReader.LoadMetaDataFromXml(xmlContents)).ConfigureAwait(false);
        var generator = Create(metaData, setting);
        return generator;
    }
}
