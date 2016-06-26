using System.Collections.Generic;

namespace OData2Poco
{
    public class MetaDataInfo
    {
        public string ServiceUrl { get; set; }
        public string MetaDataAsString { get; set; }
        public string MetaDataVersion { get; set; }
        public string ServiceVersion { get; set; } //for http media
        public string SchemaNamespace { get; set; }
        public Dictionary<string, string> ServiceHeader { get; set; }
        public Media MediaType { get; set; }

    }
}