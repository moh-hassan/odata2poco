using System;
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
        public List<ClassTemplate> ClassList;

       public O2P(PocoSetting setting = null)
        {
            Setting = setting ?? new PocoSetting();
            ClassList = new List<ClassTemplate>();
            MetaData = new MetaDataInfo();
        }

        public O2P(Action <PocoSetting> config)
        {
            Setting= new PocoSetting();
            config(Setting);
        }

        private MetaDataInfo MetaData { get; set; }
       // private Uri ServiceUri { get; set; }

        public string ServiceUrl => MetaData.ServiceUrl;

        //   private string _xmlContent { get; set; }
        /// <summary>
        ///     metadata which is read from url /file or xml string
        /// </summary>
        public string MetaDataAsString => MetaData.MetaDataAsString;

        public string MetaDataVersion => MetaData.MetaDataVersion;

        public string ServiceVersion => MetaData.ServiceVersion;

        public string SchemaNamespace => MetaData.SchemaNamespace;

        public Dictionary<string, string> ServiceHeader => MetaData.ServiceHeader;

        public Media MediaType => MetaData.MediaType;

     
       public Media Source { get; set; }

        public string CodeText { get; set; }
       

        public async Task<MetaDataInfo> LoadMetaDataHttpAsync(Uri uri, string user = "", string password = "")
        {
            if (uri == null)
                throw new ArgumentNullException(nameof(uri));
            MetaData = await MetaDataReader.LoadMetaDataHttpAsync(uri, user, password);
            return MetaData;
        }

        public async Task<string> GenerateAsync(Uri uri, string user = "", string password = "")
        {
            if (uri==null)
                throw new ArgumentNullException(nameof(uri));
            MetaData = await MetaDataReader.LoadMetaDataHttpAsync(uri, user, password);
            var gen = PocoFactory.GeneratePoco(MetaData, Setting);
            ClassList = gen.ClassList;
            CodeText = gen.ToString();
            return CodeText;
        }

        /// <summary>
        /// Generate CSharp code from xml string
        /// </summary>
        /// <param name="xmlContent"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public string Generate(string xmlContent)
        {
            if (string.IsNullOrEmpty(xmlContent))
                throw new ArgumentNullException(nameof(xmlContent));
            MetaData = MetaDataReader.LoadMetaDataFromXml(xmlContent);
            var gen = PocoFactory.GeneratePoco(MetaData, Setting);
            CodeText = gen.ToString();
            ClassList = gen.ClassList;
            return CodeText;
        }
       
    }
}