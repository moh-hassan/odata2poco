using System.Collections.Generic;

namespace OData2Poco
{
    public class MetaDataInfo
    {
        public string ServiceUrl { get; set; } = null!;
        public string MetaDataAsString { get; set; } = null!;
        public string MetaDataVersion { get; set; } = null!;
        public string ServiceVersion { get; set; } = null!;//for http media
        public string? SchemaNamespace { get; set; } = null!;
        public Dictionary<string, string> ServiceHeader { get; set; }
        public Media MediaType { get; set; }

        public MetaDataInfo()
        {
            ServiceHeader = new Dictionary<string, string>();
        }

    }
}