#if fluent
using System;
using System.Collections.Generic;
using System.IO;

/*
 * todo: build fluent based on  Api.O2P
 
 * */
namespace OData2Poco.Fluent
{
    /// <summary>
    /// Flount interface is other version of Api.O2P , but fluent oriented
    /// it should be implemented from its Api.O2P
    /// TODO: public after implementation
    /// </summary>
    internal class O2P
    {
        private Api.O2P _fluent;
      static  public PocoSetting Setting { get; set; }
        private static MetaDataReader _metaDataReader;
        private static MetaDataInfo MetaData
        {
            get { return _metaDataReader.MetaData; }

        }

        private string User { get; set; }
        private string Password { get; set; }
        private string Url { get; set; }

        public List<ClassTemplate> ClassList;

        private static string CodeText { get; set; }
        public O2P(string url)
        {
            Url = url;
            Setting = new PocoSetting();
        }


        public O2P(string url, string user, string password)
        {
            Url = url;
            User = user;
            Password = password;
            Setting = new PocoSetting();
        }
        public O2P()
        {
           // _fluent = new Api.O2P( )
            Setting = new PocoSetting();
        }
        public O2P SetUrl(string url)
        {
            Url = url;
            return this;
        }
        public O2P Authenticate(string user, string password)
        {
            User = user;
            Password = password;
            return this;
        }

        public O2P AddKeyAttribute()
        {
            Setting.AddKeyAttribute = true;
            //Console.WriteLine("key :{0}", Setting.AddKeyAttribute);
            return this;
        }
        public O2P AddRequiredAttribute()
        {
          
            Setting.AddRequiredAttribute = true;
          
            return this;
        }
        public O2P AddNavigation()
        {
            Setting.AddNavigation = true;
            return this;
        }
        public O2P AddTableAttribute()
        {
            Setting.AddTableAttribute = true;
            return this;
        }

        //public O2P SaveCodeTo(string fname = "poco.cs")
        //{
        //    Generate(Setting);
        //    File.WriteAllText(fname, CodeText);
        //    return this;
        //}
        public O2P SaveMetaDataTo(string fname = "meta.xml")
        {
            File.WriteAllText(fname, MetaData.MetaDataAsString);
            return this;
        }

        //public O2P Generate(string fname = "poco.cs")
        //{
        //    Generate(Setting);
        //    File.WriteAllText(fname, CodeText);
        //    return this;
        //}

        public O2P Generate(string fname = "meta.xml") //PocoSetting pocoSetting)
        {
           
            if (Url == null)
                throw new NullReferenceException("Url is empty");

            _metaDataReader = string.IsNullOrEmpty(User)
            ? new MetaDataReader(Url)
            : new MetaDataReader(Url, User, Password);
            //Console.WriteLine("o2p generate key: {0}", Setting.AddKeyAttribute);
            var gen = _metaDataReader.Generate(Setting);
            
            CodeText = gen.ToString();  
            //CodeText = gen.GeneratePoco();  
            File.WriteAllText(fname, CodeText);
            //ClassList = gen.ClassDictionary.Select(kvp => kvp.Value).ToList();
            ClassList = gen.ClassList;
            return this;
        }


        /// <summary>
        /// Implicit Convertion to string and return generated c# code
        /// </summary>
        /// <param name="o2P"></param>
        /// <returns></returns>
        public static implicit operator string(O2P o2P)
        {
            return CodeText;
        }
        /// <summary>
        /// Implict conversion
        /// </summary>
        /// <param name="o2P"></param>
        /// <returns></returns>
        public static implicit operator MetaDataInfo(O2P o2P)
        {
            return MetaData;
        }
         
    }
}

#endif