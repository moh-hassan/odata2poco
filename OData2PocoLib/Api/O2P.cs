using System.Collections.Generic;
using System.Threading.Tasks;

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
        public string MetaDataAsString => MetaData.MetaDataAsString;
        public string MetaDataVersion => MetaData.MetaDataVersion;
       
        public Dictionary<string, string> ServiceHeader => MetaData.ServiceHeader;
       
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

        public string GenerateProject()
        {
           var proj = new ProjectGenerator(Setting.Attributes);
           return proj.GetProjectCode();
        }
      


    }
}