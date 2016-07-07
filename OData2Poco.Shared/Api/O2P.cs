
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace OData2Poco
{
    /// <summary>
    /// Wrapper , non Flount Interface 
    /// </summary>
    public class O2P //: IO2P 
    {
        private MetaDataInfo MetaData { get; set; }
        private Uri ServiceUri { get; set; }
        public string ServiceUrl
        {
            get { return MetaData.ServiceUrl; }
        }

     //   private string _xmlContent { get; set; }
        /// <summary>
        /// metadata which is read from url /file or xml string
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
        //private string User { get; set; }
        //private string Password { get; set; }
        //private string Url { get; set; }
        public Media Source { get; set; }
        public List<ClassTemplate> ClassList;

        public string CodeText { get; set; }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="url"> can be : Url , filename or xml data</param>
        /// <param name="setting"></param>
        //public O2P(Uri url, PocoSetting setting = null)
        //{
        //    ServiceUri = url;
        //    Setting = setting ?? new PocoSetting();
        //    Source = Media.Http;
        //    ClassList = new List<ClassTemplate>();
        //    MetaData = new MetaDataInfo();
        //}
        //public O2P(Uri url, string user, string password, PocoSetting setting = null)
        //    : this(url)
        //{
        //    User = user;
        //    Password = password;
        //}

        //public O2P(string xmlContent, PocoSetting setting = null)
        //{
        //    _xmlContent = xmlContent;
        //    Setting = setting ?? new PocoSetting();
        //    Source = Media.Http;
        //    ClassList = new List<ClassTemplate>();
        //    MetaData = new MetaDataInfo();
        //}

        public O2P( PocoSetting setting = null)
        {
           
            Setting = setting ?? new PocoSetting();
         //   Source = Media.Http;
            ClassList = new List<ClassTemplate>();
            MetaData = new MetaDataInfo();
        }
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

        public async Task<string> GenerateAsync(Uri uri,string user="",string password="")
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


