using OData2Poco.TypeScript;
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
        public string MetaDataAsString => MetaData.MetaDataAsString;
        public string MetaDataVersion => MetaData.MetaDataVersion;
        public string? SchemaNamespace => MetaData.SchemaNamespace;
        public Dictionary<string, string> ServiceHeader => MetaData.ServiceHeader;

        public string CodeText { get; set; }
        //warning due to renaming of reserved keywords
        public List<string> ModelWarning => ModelManager.ModelWarning;
        public O2P(PocoSetting? setting = null)
        {
            Setting = setting ?? new PocoSetting();
            ClassList = new List<ClassTemplate>();
            MetaData = new MetaDataInfo();
            CodeText = "";
        }
        public async Task<IPocoGenerator> GenerateModel(OdataConnectionString odataConnString)
        {
            IPocoGenerator gen = await PocoFactory.GenerateModel(odataConnString, Setting);
            MetaData = gen.MetaData;
            ClassList = gen.GeneratePocoList();
            return gen;
        }
        public async Task<IPocoGenerator> GenerateModel(string xmlContent)
        {
            IPocoGenerator gen = await PocoFactory.GenerateModel(xmlContent, Setting);
            MetaData = gen.MetaData;
            ClassList = gen.GeneratePocoList();
            return gen;
        }
        //Generate c# code
        public async Task<string> GenerateAsync(OdataConnectionString odataConnString)
        {
            var gen = await GenerateModel(odataConnString);
            var generatorCs = PocoClassGeneratorCs.GenerateCsPocoClass(gen, Setting);
            CodeText = generatorCs.ToString();
            return CodeText;
        }

        //feature request #41
        public async Task<string> GenerateAsync(string xmlContent)
        {
            var gen = await GenerateModel(xmlContent);
            var generatorCs = PocoClassGeneratorCs.GenerateCsPocoClass(gen, Setting);
            CodeText = generatorCs.ToString();
            return CodeText;
        }
        public string GenerateProject()
        {
            var proj = new ProjectGenerator(Setting.Attributes);
            return proj.GetProjectCode();
        }
#if OPENAPI
        public async Task<string> GenerateOpenApiAsync(OdataConnectionString odataConnString,
          int openApiVersion = 3)
        {
            IPocoGenerator gen = await GenerateModel(odataConnString);
            var apiText = gen.GenerateOpenApi(openApiVersion);
            return apiText;
        }
#endif         
        public async Task<PocoStore> GenerateTsAsync(OdataConnectionString odataConnString)
        {
            IPocoGenerator gen = await GenerateModel(odataConnString);
            var ts = new TsPocoGenerator(gen, Setting);
            var pocoStore = ts.GeneratePoco();
            return pocoStore;
        }

    }
}