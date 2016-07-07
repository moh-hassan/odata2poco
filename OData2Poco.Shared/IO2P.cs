using System.Collections.Generic;
using System.Threading.Tasks;

namespace OData2Poco
{
    public interface IO2P 
    {
        string ServiceUrl { get; }
        string MetaDataAsString { get; }
        string MetaDataVersion { get; }
        string ServiceVersion { get; }
        string SchemaNamespace { get; }
        Dictionary<string, string> ServiceHeader { get; }
        Media MediaType { get; }
        PocoSetting Setting { get; set; }
        string CodeText { get; set; }
        void AddKeyAttribute();
        void AddRequiredAttribute();
        void AddNavigation();
        void AddTableAttribute();
        //Task<string> SaveMetaDataTo(string fname = "meta.xml");
        Task<string> Generate(string fname = "poco.cs");
    }
}