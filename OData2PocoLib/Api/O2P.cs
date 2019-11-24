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
        internal MetaDataInfo MetaData { get; set; }
        public string MetaDataAsString => MetaData?.MetaDataAsString;
        public string MetaDataVersion => MetaData?.MetaDataVersion;
        public string SchemaNamespace => MetaData?.SchemaNamespace;
        public Dictionary<string, string> ServiceHeader => MetaData?.ServiceHeader;

        public string CodeText { get; set; }
        //warning due to renaming of reserved keywords
        public List<string> ModelWarning => ModelManager.ModelWarning;
        public O2P(PocoSetting setting = null)
        {
            Setting = setting ?? new PocoSetting();
            ClassList = new List<ClassTemplate>();
            MetaData = new MetaDataInfo();
        }


        internal async Task<IPocoGenerator> GenerateModel(OdataConnectionString odataConnString)
        {
            IPocoGenerator gen = await PocoFactory.GenerateModel(odataConnString, Setting);
            MetaData=gen.MetaData;
            return gen;
        }
       
        public async Task<string> GenerateAsync(OdataConnectionString odataConnString)
        {
            var gen = await GenerateModel(odataConnString);
            var generatorCs = PocoClassGeneratorCs.GenerateCsPocoClass(gen, Setting);
            ClassList = generatorCs.ClassList;
            CodeText = generatorCs.ToString();
            return CodeText;
        }
      

        public string GenerateProject()
        {
            var proj = new ProjectGenerator(Setting.Attributes);
            return proj.GetProjectCode();
        }
    }
}