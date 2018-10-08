using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;


namespace OData2Poco.Api
{
    /// <summary>
    ///     Wrapper , non Flount Interface
    /// </summary>
    public class O2P //: IO2P 
    {
        public List<ClassTemplate> ClassList;

       public O2P(PocoSetting setting = null)
        {
            Setting = setting ?? new PocoSetting();
            //   Source = Media.Http;
            ClassList = new List<ClassTemplate>();
            MetaData = new MetaDataInfo();
        }

        private MetaDataInfo MetaData { get; set; }
        private Uri ServiceUri { get; set; }

        public string ServiceUrl => MetaData.ServiceUrl;

        //   private string _xmlContent { get; set; }
        /// <summary>
        ///     metadata which is read from url /file or xml string
        /// </summary>
        public string MetaDataAsString => MetaData.MetaDataAsString;

        public string MetaDataVersion => MetaData.MetaDataVersion;

        public string ServiceVersion
            //for http media
            => MetaData.ServiceVersion;

        public string SchemaNamespace => MetaData.SchemaNamespace;

        public Dictionary<string, string> ServiceHeader => MetaData.ServiceHeader;

        public Media MediaType => MetaData.MediaType;

        public PocoSetting Setting { get; set; }
       public Media Source { get; set; }

        public string CodeText { get; set; }

        public void AddKeyAttribute()
        {
            Setting.AddKeyAttribute = true;
        }

        public void AddRequiredAttribute()
        {
            Setting.AddRequiredAttribute = true;
        }

        public void AddNavigation()
        {
            Setting.AddNavigation = true;
        }

        public void AddTableAttribute()
        {
            Setting.AddTableAttribute = true;
        }

        /// <summary>
        /// Set properties of class public (without virtual modifier)
        /// </summary>
        public void AddEager()
        {
            Setting.AddEager = true;
        }

        public async Task<string> GenerateAsync(Uri uri, string user = "", string password = "")
        {
            //if (string.IsNullOrEmpty(uri.AbsoluteUri))
            //    throw new ArgumentNullException();
            //  MetaData = await MetaDataReader.LoadMetaDataHttpAsync(ServiceUri, User, Password);
            MetaData = await MetaDataReader.LoadMetaDataHttpAsync(uri, user, password);
            var gen = PocoFactory.GeneratePoco(MetaData, Setting);
            ClassList = gen.ClassList;
            CodeText = gen.ToString();
            Debug.WriteLine(CodeText);
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
                throw new ArgumentNullException();
            MetaData = MetaDataReader.LoadMetaDataFromXml(xmlContent);
            var gen = PocoFactory.GeneratePoco(MetaData, Setting);
            CodeText = gen.ToString();
            ClassList = gen.ClassList;
            Debug.WriteLine(CodeText);
            return CodeText;
        }
    }
}