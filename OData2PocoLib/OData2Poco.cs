using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace OData2Poco
{
    public class O2P
    {

        public string User { get; set; }
        public string Password { get; set; }
        public string Url { get; set; }
        private MetaDataReader _metaDataReader;
        public List<ClassTemplate> ClassList;
        public string ServiceVersion { get; set; }
        public string MetaDataVersion { get; set; }
        public string MetaDataAsString { get; private set; }
        public Dictionary<string, string> ServiceHeader { get; set; }
        public O2P(string url)
        {
            Url = url;
        }

        public O2P(string url, string user, string password)
            : this(url)
        {
            User = user;
            Password = password;
            //Url = _url;
        }

        public string Generate(Language lang = Language.CS)
        {
            if (lang == Language.CS) return CSGenerate();
            else return VBGenerate();
        }

        private string VBGenerate()
        {
            throw new NotImplementedException();
        }

        private string CSGenerate()
        {
            _metaDataReader = string.IsNullOrEmpty(User)
            ? new MetaDataReader(Url)
            : new MetaDataReader(Url, User, Password);

            var code = _metaDataReader.GeneratePoco();
            ServiceVersion = _metaDataReader.ServiceVersion;
            MetaDataVersion = _metaDataReader.MetaDataVersion;
            ClassList = _metaDataReader.ClassList;
            ServiceHeader = _metaDataReader.ServiceHeader;
            MetaDataAsString = _metaDataReader.MetaDataAsString;
         //   File.WriteAllText(filename, code);
            // return this;
            return code;
        }
        public void SaveMetadata(string fname="meta.xml")
        {
            File.WriteAllText(fname, MetaDataAsString);
        }
    }
}
