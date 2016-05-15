using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;


namespace OData2Poco
{
    internal class MetaDataReader
    {
        public string ServiceUrl { get; set; }
        private string User { get; set; }
        private string Password { get; set; }
        private string _metaDataAsString;
        public string MetaDataAsString
        {
            get
            {
                return _metaDataAsString ?? (_metaDataAsString = LoadMetaData());
            }
        }

        private string _metaDataVersion;
        public string MetaDataVersion
        {
            get
            {
                if (!string.IsNullOrEmpty(MetaDataAsString))
                return _metaDataVersion ?? (_metaDataVersion = Helper.GetMetadataVersion(MetaDataAsString));
                return ODataVersion.VNa;
            }
        }
        public string ServiceVersion { get; set; }
        public string SchemaNamespace { get; set; }
        public Dictionary<string, string> ServiceHeader { get; set; }
        //public HttpStatusCode StatusCode { get; set; }
        public Media MediaType { get; set; }

        private PocoClassGenerator _generator;
        public PocoClassGenerator Generator
        {
            get { return _generator ?? (_generator = Execute()); }
        }

        public MetaDataReader(string url)
        {
            ServiceUrl = url;
            ServiceHeader = new Dictionary<string, string>();
        }

        public MetaDataReader(string url, string user, string pw)
            : this(url)
        {
            //   ServiceUrl = url;
            User = user;
            Password = pw;

        }

        public PocoClassGenerator Execute()
        {
            //avoid double execution
            if (_generator == null)
            {
                IPocoGenerator pocoFactory = PocoFactory.Create(MetaDataAsString, ServiceUrl);
                _generator = new PocoClassGenerator(pocoFactory);
            }
            return _generator;
        }
        //public async Task<PocoClassGenerator> ExecuteAsync()
        //{
        //    if (_generator == null)
        //    {
        //        IPocoGenerator pocoFactory = PocoFactory.Create(MetaDataAsString, ServiceUrl);
        //        _generator = new PocoClassGenerator(pocoFactory);
        //    }
        //    return _generator;
        //}

        private string LoadMetaData()
        {
            Console.WriteLine("LoadMetaData");
            string metaLocation = ServiceUrl;
            string content;
            //logger.Info("Connecting to: " +metaLocation);
            if (metaLocation.StartsWith("http"))
            {
                MediaType = Media.Http;
                content = LoadMetaDataHttp();
            }
            else
            {
                MediaType = Media.File;
                content = LoadMetaDataFile();
            }

            return content;
        }

        internal async Task<string> LoadMetaDataAsync()
        {
            string metaLocation = ServiceUrl;
            string content;
            //logger.Info("Connecting to: " +metaLocation);
            if (metaLocation.StartsWith("http"))
            {
                MediaType = Media.Http;
                content = await LoadMetaDataHttpAsync();
            }
            else
            {
                MediaType = Media.File;
                content = await LoadMetaDataFileAsync();
            }

            return content;
        }

        internal string LoadMetaDataFile()
        {
            if (!File.Exists(ServiceUrl))
                throw new FileNotFoundException("File not found: " + ServiceUrl);
            using (var reader = File.OpenText(ServiceUrl))
            {
                var text = reader.ReadToEnd();
                return text;
            }
        }

        internal async Task<string> LoadMetaDataFileAsync()
        {
            if (!File.Exists(ServiceUrl))
                throw new FileNotFoundException("File not found: " + ServiceUrl);
            using (var reader = File.OpenText(ServiceUrl))
            {
                var text = await reader.ReadToEndAsync();
                return text;
            }
        }

        //httpclient
        //private async Task<string> LoadMetaDataHttp2Async()
        //{
        //    string url = ServiceUrl.TrimEnd('/') + "/$metadata";
        //    using (var httpClient = new HttpClient())
        //    {
        //        //credintial
        //        if (!string.IsNullOrEmpty(User))
        //        {
        //            var byteArray = Encoding.ASCII.GetBytes(String.Format("{0}:{1}", User, Password)); // "USER:PASS"
        //            httpClient.DefaultRequestHeaders.Authorization =
        //                new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
        //        }

        //        var response = await httpClient.GetAsync(new Uri(url));
        //        response.EnsureSuccessStatusCode();    // Throw if not a success code.

        //        StatusCode = response.StatusCode;
        //        GetServiceHttpHeader(response);
        //        var content = await response.Content.ReadAsStringAsync();
        //        content = Helper.PrettyXml(content);
        //        //Debug.WriteLine(content);
        //        return content;
        //    }
        //}
        internal async Task<string> LoadMetaDataHttpAsync()
        {
            string url = ServiceUrl.TrimEnd('/') + "/$metadata";
            using (var httpClient = new WebClient())
            {

                //credintial
                if (!string.IsNullOrEmpty(User))
                {
                    string credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes(User + ":" + Password));
                    httpClient.Headers[HttpRequestHeader.Authorization] = string.Format("Basic {0}", credentials);
                }

                var content = await httpClient.DownloadStringTaskAsync(url);
                //   response.EnsureSuccessStatusCode();    // Throw if not a success code.

                if (!string.IsNullOrEmpty(content))
                {
                    content = Helper.PrettyXml(content);
                    //Debug.WriteLine(content);
                    GetServiceHttpHeader(httpClient);
                }
                return content;
            }
        }

        internal string LoadMetaDataHttp()
        {
            string url = ServiceUrl.TrimEnd('/') + "/$metadata";
            using (var client = new WebClient())
            {
                //credintial
                if (!string.IsNullOrEmpty(User))
                {
                    string credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes(User + ":" + Password));
                    client.Headers[HttpRequestHeader.Authorization] = string.Format("Basic {0}", credentials);
                }

                var content = client.DownloadString(url);

                if (!string.IsNullOrEmpty(content))
                {
                    content = Helper.PrettyXml(content);
                    //Debug.WriteLine(content);
                    GetServiceHttpHeader(client);
                }
                return content;
            }
        }


        internal void GetServiceHttpHeader(WebClient client)
        {
            WebHeaderCollection whc = client.ResponseHeaders;
            //Console.WriteLine("header count = {0}", whc.Count);
            for (int i = 0; i < whc.Count; i++)
            {
                var key = whc.GetKey(i);
                var value = whc.Get(i);
                //Console.WriteLine(key + " = " + value);
                if (key.Contains("OData-Version") || key.Contains("DataServiceVersion"))
                {
                    ServiceVersion = value;
                }
                ServiceHeader.Add(key, value);
            }
        }

        //private void GetServiceHttpHeader(HttpResponseMessage response)
        //{
        //    foreach (var header in response.Headers)
        //    {
        //        if (header.Key.Contains("OData-Version") || header.Key.Contains("DataServiceVersion"))
        //        {
        //            ServiceVersion = header.Value.FirstOrDefault();
        //        }
        //        ServiceHeader.Add(header.Key, header.Value.FirstOrDefault());
        //        //Debug.WriteLine("{0} {1} ", header.Key, header.Value.FirstOrDefault());
        //    }
        //}

        #region CodeGeneration
        /// <summary>
        /// Generate cs code for all POCO classes as a one unit 
        /// </summary>
        /// <returns></returns>
        public string GeneratePoco()
        {
            if (string.IsNullOrEmpty(MetaDataAsString)) return String.Empty;
          
               //generator property have all information
                //TODO: generate code for multi files (one file per class)
                var code = Generator.ToString(); //one file for all classes
                //populate ClassList
                //var classList = generator.ClassDictionary.Select(kvp => kvp.Value).ToList();
                return code;
        }
        //call PocoFactory to get poco that match v3/v4 generation
        //public async Task<string> GeneratePocoAsync()
        //{

        //    //MetaDataAsString = await LoadMetaDataAsync();
        //    //Console.WriteLine("metalength {0}", MetaDataAsString);
        //    if (!string.IsNullOrEmpty(MetaDataAsString))
        //    {
        //        //IPocoGenerator pocoFactory = PocoFactory.Create(MetaDataAsString, ServiceUrl);
        //        //var generator = new PocoClassGenerator(pocoFactory);

        //        var code = Generator.ToString(); //generate all classes
        //        foreach (var item in _generator.ClassDictionary)
        //        {
        //            Console.WriteLine("dict {0} {1}", item.Key, item.Value.EntitySetName);
        //        }
        //        //ClassList = generator.ClassList.ToList();
        //        //ClassList = generator.ClassDictionary.Select(kvp => kvp.Value).ToList();
        //        //Console.WriteLine("classlist {0}", ClassList.Count);
        //        return code;
        //    }
        //    return "";
        //}
        #endregion
        public void SaveMetadata(string fname)
        {
            File.WriteAllText(fname, MetaDataAsString);
        }
    }
}
