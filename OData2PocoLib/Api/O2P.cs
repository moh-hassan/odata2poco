using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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

        public O2P(Action <PocoSetting> config)
        {
            Setting= new PocoSetting();
            config(Setting);
        }

        private MetaDataInfo MetaData { get; set; }
        private Uri ServiceUri { get; set; }

        public string ServiceUrl
        {
            get { return MetaData.ServiceUrl; }
        }

        //   private string _xmlContent { get; set; }
        /// <summary>
        ///     metadata which is read from url /file or xml string
        /// </summary>
        public string MetaDataAsString
        {
            get { return MetaData.MetaDataAsString; }
        }

        public string MetaDataVersion
        {
            get { return MetaData.MetaDataVersion; }
        }

        public string ServiceVersion
        {
            get { return MetaData.ServiceVersion; }
        } //for http media

        public string SchemaNamespace
        {
            get { return MetaData.SchemaNamespace; }
        }

        public Dictionary<string, string> ServiceHeader
        {
            get { return MetaData.ServiceHeader; }
        }

        public Media MediaType
        {
            get { return MetaData.MediaType; }
        }

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

        public async Task<MetaDataInfo> LoadMetaDataHttpAsync(Uri uri, string user = "", string password = "")
        {
            MetaData = await MetaDataReader.LoadMetaDataHttpAsync(uri, user, password);
            return MetaData;
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
            //Debug.WriteLine(CodeText);
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