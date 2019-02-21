using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using OData2Poco.OAuth2;

namespace OData2Poco.Api
{
    /// <summary>
    ///     Wrapper class
    /// </summary>
    public class O2P
    {
        public PocoSetting Setting { get; set; }
        public List<ClassTemplate> ClassList { get; set; }
        private MetaDataInfo MetaData { get; set; }
        //public string ServiceUrl => MetaData.ServiceUrl;
        public string MetaDataAsString => MetaData.MetaDataAsString;
        public string MetaDataVersion => MetaData.MetaDataVersion;
        //public string ServiceVersion => MetaData.ServiceVersion;
        //public string SchemaNamespace => MetaData.SchemaNamespace;
        public Dictionary<string, string> ServiceHeader => MetaData.ServiceHeader;
        //public Media MediaType => MetaData.MediaType;
        //public Media Source { get; set; }
        public string CodeText { get; set; }
        public O2P(PocoSetting setting = null)
        {
            Setting = setting ?? new PocoSetting();
            ClassList = new List<ClassTemplate>();
            MetaData = new MetaDataInfo();
        }

        public async Task<string> GenerateAsync(OdataConnectionString odataConnString)
        {
            MetaData = await MetaDataReader.LoadMetadataAsync(odataConnString);
            var gen = GenerateModel(MetaData);
            CodeText = gen.ToString();
            return CodeText;
        }
        private IPocoClassGenerator GenerateModel(MetaDataInfo metaData)
        {
            var gen = PocoFactory.GeneratePoco(metaData, Setting);
            ClassList = gen.ClassList;
            return gen;
        }

        //private string GenerateCodeAsync(MetaDataInfo metaData)
        //{
        //    var gen = PocoFactory.GeneratePoco(metaData, Setting);
        //    ClassList = gen.ClassList;

        //    CodeText = gen.ToString();
        //    return CodeText;
        //}
        //public async Task<string> GenerateAsync(OdataConnectionString odataConnString)
        //{
        //    MetaData = await MetaDataReader.LoadMetadataAsync(odataConnString);
        //    var code = GenerateCodeAsync(MetaData);
        //    return code;
        //}


    }
}