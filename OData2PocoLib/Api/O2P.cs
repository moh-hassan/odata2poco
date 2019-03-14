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
        public string SchemaNamespace =>MetaData?.SchemaNamespace;
        public Dictionary<string, string> ServiceHeader => MetaData?.ServiceHeader;

        public string CodeText { get; set; }
        public O2P(PocoSetting setting = null)
        {
            Setting = setting ?? new PocoSetting();
            ClassList = new List<ClassTemplate>();
            MetaData = new MetaDataInfo();
        }

       
        //internal async Task<IPocoGenerator> GenerateModel(OdataConnectionString odataConnString)
        //{
        //    //MetaData = await MetaDataReader.LoadMetadataAsync(odataConnString);
        //    IPocoGenerator gen = await PocoFactory.GenerateModel(odataConnString, Setting);
        //    return gen;
        //}
        public async Task<string> GenerateAsync(OdataConnectionString odataConnString)
        {
            
            var generator = await PocoFactory.GeneratePoco(odataConnString, Setting);
            ClassList = generator.ClassList;
            CodeText = generator.ToString();
            return CodeText;
        }
        //public async Task<string> GenerateAsync(OdataConnectionString odataConnString)
        //{
        //    var gen = await GenerateModel(odataConnString);
        //    var generatorCs = new PocoClassGeneratorCs(gen, Setting);
        //    ClassList = generatorCs.ClassList;
        //    CodeText = generatorCs.ToString();
        //    return CodeText;
        //}
        //public async Task<string> GenerateTsAsync(OdataConnectionString odataConnString)
        //{
        //    var gen = await GenerateModel(odataConnString);
        //    var ts = new PocoClassGeneratorTs(gen, Setting);
        //    ClassList = ts.ClassList;
        //    CodeText = ts.ToString();
        //    return CodeText;
        //}

        public string GenerateProject()
        {
            var proj = new ProjectGenerator(Setting.Attributes);
            return proj.GetProjectCode();
        }
    }
}