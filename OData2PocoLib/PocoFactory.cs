using System;
using System.Threading.Tasks;
using OData2Poco.V4;

namespace OData2Poco
{
    // factory class
    internal class PocoFactory
    {
        /// <summary>
        /// Generate Poco Modelas List<ClassTemplate> 
        /// </summary>
        /// <param name="metadata"></param>
        /// <param name="setting"></param>
        /// <returns></returns>
        internal static IPocoGenerator Create(MetaDataInfo metadata, PocoSetting setting)
        {
            if (string.IsNullOrEmpty(metadata.MetaDataAsString)) throw new InvalidOperationException("No Metadata available");

            var metaDataVersion = metadata.MetaDataVersion;
            switch (metaDataVersion)
            {
                case ODataVersion.V4:
                    return new Poco(metadata, setting);

                case ODataVersion.V1:
                case ODataVersion.V2:
                case ODataVersion.V3:
                    return new V3.Poco(metadata, setting);
                //throw new NotImplementedException();

                default:
                    throw new NotSupportedException($"OData Version '{metaDataVersion}' is not supported");

            }
        }
        //todo
        internal static async Task<IPocoGenerator> GenerateModel(OdataConnectionString connectionString,
            PocoSetting setting)
        {
            var metaData = await MetaDataReader.LoadMetadataAsync(connectionString);
            IPocoGenerator generator = Create(metaData, setting);
            return generator;
        }

        internal static async Task<IPocoGenerator> GenerateModel(string xmlContents,
          PocoSetting setting)
        {
            var metaData = await Task.Run (()=> MetaDataReader.LoadMetaDataFromXml(xmlContents));
            IPocoGenerator generator = Create(metaData, setting);
            return generator;
        }

        //internal static async Task<IPocoClassGenerator> GeneratePoco(OdataConnectionString connectionString,
        //    PocoSetting setting)
        //{
        //  var gen = await  GenerateModel( connectionString, setting);
        //  switch (setting.Lang)
        //  {  
        //      case Language.CS:
        //      case Language.VB:
        //          return new PocoClassGeneratorCs(gen, setting);
        //      //todo
        //      //case Language.TS:
        //      //    return new PocoClassGeneratorTs(gen, setting);
        //      default:
        //          throw new ArgumentOutOfRangeException();
        //  }
        //}

        //----------
        //var generatorCs = new PocoClassGeneratorCs(gen, Setting);

        //public async Task<string> GenerateAsync(OdataConnectionString odataConnString)
        //{
        //    var gen = await GenerateModel(odataConnString);
        //    var generatorCs = new PocoClassGeneratorCs(gen, Setting);
        //    ClassList = generatorCs.ClassList;
        //    CodeText = generatorCs.ToString();
        //    return CodeText;
        //}

        //public static IPocoGenerator GenerateModelInternal(MetaDataInfo metadata, PocoSetting setting = null)
        //{
        //    if (setting == null) setting = new PocoSetting();
        //    if (string.IsNullOrEmpty(metadata.MetaDataAsString))
        //        throw new XmlException("Metaddata is empty");

        //    IPocoGenerator generator = Create(metadata, setting);
        //    //save to cache for latter use
        //    ServiceCache.Default.AddGenerator(generator);
        //    return generator;
        //}
    }
}

