// Copyright (c) Mohamed Hassan & Contributors. All rights reserved. See License.md in the project root for license information.

using OData2Poco.V4;

namespace OData2Poco;

// factory class
internal static class PocoFactory
{
    /// <summary>
    ///     Generate Poco Modelas List<ClassTemplate>
    /// </summary>
    /// <param name="metadata"></param>
    /// <param name="setting"></param>
    /// <returns></returns>
    internal static IPocoGenerator Create(MetaDataInfo metadata, PocoSetting setting)
    {
        if (string.IsNullOrEmpty(metadata.MetaDataAsString))
            throw new InvalidOperationException("No Metadata available");

        var metaDataVersion = metadata.MetaDataVersion;
        switch (metaDataVersion)
        {
            case ODataVersion.V4:
                return new Poco(metadata, setting);

            case ODataVersion.V1:
            case ODataVersion.V2:
            case ODataVersion.V3:
                return new V3.Poco(metadata, setting);

            default:
                throw new NotSupportedException($"OData Version '{metaDataVersion}' is not supported");
        }
    }

    internal static async Task<IPocoGenerator> GenerateModel(OdataConnectionString connectionString,
        PocoSetting setting)
    {
        var metaData = await MetaDataReader.LoadMetadataAsync(connectionString);
        var generator = Create(metaData, setting);
        return generator;
    }

    internal static async Task<IPocoGenerator> GenerateModel(string xmlContents,
        PocoSetting setting)
    {
        var metaData = await Task.Run(() => MetaDataReader.LoadMetaDataFromXml(xmlContents));
        var generator = Create(metaData, setting);
        return generator;
    }
}