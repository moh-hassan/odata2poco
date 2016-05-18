using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;


namespace OData2Poco
{
    internal class MetaDataReader  //: IMetaDataReader
    {
        public MetaDataInfo MetaData { get; set; }
        public string ServiceUrl { get; set; }
        private string User { get; set; }
        private string Token { get; set; }
        public MetaDataReader(string url)
        {
            ServiceUrl = url;
        }

        public MetaDataReader(string url, string user, string pw)
            : this(url)
        {
            Token = Convert.ToBase64String(Encoding.ASCII.GetBytes(user + ":" + pw));
            User = user;
        }

        public PocoClassGenerator Generate()
        {
            PocoSetting setting = new PocoSetting();
            return Generate(setting);
        }

        public PocoClassGenerator Generate(PocoSetting setting)
        {
            if (MetaData == null) MetaData = LoadMetaData();
            IPocoGenerator pocoFactory = PocoFactory.Create(MetaData);
            var generator = new PocoClassGenerator(pocoFactory, setting);
            return generator;
        }

      public MetaDataInfo LoadMetaData()
        {
            string metaLocation = ServiceUrl;
            if (metaLocation.StartsWith("http"))
            {
                return LoadMetaDataHttp();
            }
            return LoadMetaDataFile(); //file source
        }

        internal async Task<MetaDataInfo> LoadMetaDataAsync()
        {
            string metaLocation = ServiceUrl;
            string content;
         
            if (metaLocation.StartsWith("http"))
            {
                return await LoadMetaDataHttpAsync();
            }
            return await LoadMetaDataFileAsync();
        
        }

        private MetaDataInfo LoadMetaDataFile()
        {
            if (!File.Exists(ServiceUrl))
                throw new FileNotFoundException("File not found: " + ServiceUrl);


            using (var reader = File.OpenText(ServiceUrl))
            {
                var text = reader.ReadToEnd();
                var metaData = new MetaDataInfo
                {
                    MetaDataAsString = text,
                    MetaDataVersion = Helper.GetMetadataVersion(text),
                     ServiceHeader = new Dictionary<string, string>( ), //http only
                    ServiceVersion = "NA", //http only
                    ServiceUrl = ServiceUrl,
                    SchemaNamespace = "",
                     MediaType = Media.File
                };
                return metaData;
            }
        }

        private async Task<MetaDataInfo> LoadMetaDataFileAsync()
        {
            if (!File.Exists(ServiceUrl))
                throw new FileNotFoundException("File not found: " + ServiceUrl);
            using (var reader = File.OpenText(ServiceUrl))
            {
                var text = await reader.ReadToEndAsync();
                var metaData = new MetaDataInfo
                {
                    MetaDataAsString = text,
                    MetaDataVersion = Helper.GetMetadataVersion(text),
                    //ServiceHeader = null, //http only
                    //ServiceVersion = null, //http only
                    ServiceUrl = ServiceUrl,
                    SchemaNamespace = "",
                    MediaType = Media.File
                };
                return metaData;
            }
        }
       

        private MetaDataInfo LoadMetaDataHttp()
        {
            string url = ServiceUrl.TrimEnd('/') + "/$metadata";
            using (var client = new WebClient())
            {
                //credintial
                if (!string.IsNullOrEmpty(User))
                {
                  //  string credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes(User + ":" + Password));
                    client.Headers[HttpRequestHeader.Authorization] = string.Format("Basic {0}", Token);
                }

                var content = client.DownloadString(url);

                if (!string.IsNullOrEmpty(content))
                {
                    content = Helper.PrettyXml(content);
                    GetServiceHttpHeader(client);
                    var metaData = new MetaDataInfo
                    {
                        MetaDataAsString = content,
                        MetaDataVersion = Helper.GetMetadataVersion(content),
                        ServiceHeader = GetServiceHttpHeader(client),
                        //ServiceVersion = null,
                        ServiceUrl = ServiceUrl,
                        SchemaNamespace = "",
                        MediaType = Media.Http
                    };
                    metaData.ServiceVersion = GetServiceVersion(metaData.ServiceHeader);
                    return metaData;
                }
                return new MetaDataInfo();
            }
        }

        private async Task<MetaDataInfo> LoadMetaDataHttpAsync()
        {
            string url = ServiceUrl.TrimEnd('/') + "/$metadata";
            using (var client = new WebClient())
            {
                //credintial
                if (!string.IsNullOrEmpty(User))
                {
                   
                    //_token = Convert.ToBase64String(Encoding.ASCII.GetBytes(User + ":" + Password));
                    client.Headers[HttpRequestHeader.Authorization] = string.Format("Basic {0}", Token);
                }

                var content = await client.DownloadStringTaskAsync(url);

                if (!string.IsNullOrEmpty(content))
                {
                    content = Helper.PrettyXml(content);
                    GetServiceHttpHeader(client);
                    var metaData = new MetaDataInfo
                    {
                        MetaDataAsString = content,
                        MetaDataVersion = Helper.GetMetadataVersion(content),
                        ServiceHeader = GetServiceHttpHeader(client),
                        //ServiceVersion = null,
                        ServiceUrl = ServiceUrl,
                        SchemaNamespace = "",
                        MediaType = Media.Http
                    };
                    metaData.ServiceVersion = GetServiceVersion(metaData.ServiceHeader);
                    return metaData;
                }
                return new MetaDataInfo();
            }
        }
        
        internal Dictionary<string, string> GetServiceHttpHeader(WebClient client)
        {
            Dictionary<string, string> header = new Dictionary<string, string>();
          
            WebHeaderCollection whc = client.ResponseHeaders;
            foreach (string key in whc)
                header.Add(key, whc[key]);

            //for (int i = 0; i < whc.Count; i++)
            //{
            //    var key = whc.GetKey(i);
            //    var value = whc.Get(i);
            //    header.Add(key, value);
            //}
            return header;
        }
        internal string GetServiceVersion(Dictionary<string, string> header)
        {

            foreach (var entry in header)
            {
                if (entry.Key.Contains("OData-Version") || entry.Key.Contains("DataServiceVersion"))
                    return  entry.Value;
            }
            return "";
        }

        #region CodeGeneration
        /// <summary>
        /// Generate cs code for all POCO classes as a one unit 
        /// </summary>
        /// <returns></returns>
        public string GeneratePoco()
        {
            //  if (string.IsNullOrEmpty(MetaDataAsString)) return String.Empty;
            MetaData = LoadMetaData();
            //generator property have all information
            //TODO: generate code for multi files (one file per class)
            var code = Generate().ToString(); // Generator.ToString(); //one file for all classes
            //populate ClassList
            //var classList = generator.ClassDictionary.Select(kvp => kvp.Value).ToList();
            return code;
        }

        #endregion
        public void SaveMetadata(string fname)
        {
            File.WriteAllText(fname, MetaData.MetaDataAsString);
        }
    }
}
