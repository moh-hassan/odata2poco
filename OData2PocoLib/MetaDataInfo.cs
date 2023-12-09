// Copyright (c) Mohamed Hassan & Contributors. All rights reserved. See License.md in the project root for license information.

namespace OData2Poco;

public class MetaDataInfo
{
    public MetaDataInfo()
    {
        ServiceHeader = [];
    }

    public string ServiceUrl { get; set; } = null!;
    public string MetaDataAsString { get; set; } = null!;
    public string MetaDataVersion { get; set; } = null!;
    public string ServiceVersion { get; set; } = null!; //for http media
    public string? SchemaNamespace { get; set; } = null!;
    public Dictionary<string, string> ServiceHeader { get; set; }
    public Media MediaType { get; set; }
}