using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
//using System.Net.Http;
using System.Net.Http.Headers;

namespace OData2Poco
{
    internal class MetaDataReader  
    {
        public static  async Task<MetaDataInfo> LoadMetaDataHttpAsync(Uri serviceUri,string user ,string password)
        {
            string url = serviceUri.AbsoluteUri.TrimEnd('/') + "/$metadata";
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("User-Agent", "OData2Poco in Codeplex");
                //credintial
                if (!string.IsNullOrEmpty(user))
                {
                    var token = Convert.ToBase64String(Encoding.UTF8.GetBytes(user + ":" + password));
                    Debug.WriteLine(token);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", token);
                }

                using (HttpResponseMessage response = await client.GetAsync(url))
                {
                    //   Debug.WriteLine(await response.Content.ReadAsStringAsync());
                    //response.EnsureSuccessStatusCode();
                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        //Debug.WriteLine(content);
                        if (!string.IsNullOrEmpty(content))
                        {
                           // content = Helper.PrettyXml(content);
                            var metaData = new MetaDataInfo
                            {
                                MetaDataAsString = content,
                                MetaDataVersion = Helper.GetMetadataVersion(content),
                                ServiceUrl = serviceUri.OriginalString,
                                SchemaNamespace = Helper.GetNameSpace(content),
                                MediaType = Media.Http,
                                ServiceHeader = new Dictionary<string, string>()
                            };
                            foreach (var entry in response.Headers)
                            {
                                string value = entry.Value.FirstOrDefault();
                                string key = entry.Key;
                                //Debug.WriteLine(key +":" +value);
                                metaData.ServiceHeader.Add(key, value);
                            }
                            metaData.ServiceVersion = Helper.GetServiceVersion(metaData.ServiceHeader);
                            //Debug.WriteLine(metaData.MetaDataAsString);
                            return metaData;
                        }

                    }
                    Debug.WriteLine(response.ReasonPhrase);
                    // throw new WebException("Http Error " + (int)response.StatusCode + ": " + response.ReasonPhrase);
                    throw new HttpRequestException("Http Error " + (int) response.StatusCode + ": " +
                                                   response.ReasonPhrase);


                }
            }
        }
#if fileSupport
        internal static async Task<MetaDataInfo> LoadMetaDataFileAsync(string fname)
        {
            IFile dataFile = await FileSystem.Current.GetFileFromPathAsync(fname);
            if (dataFile == null)
                throw new FileNotFoundException("File not found: " + fname);

            var content = await dataFile.ReadAllTextAsync();
            Debug.WriteLine(content);
            if (!string.IsNullOrEmpty(content))
            {
              var metaData = new MetaDataInfo
                {
                    MetaDataAsString = content,
                    MetaDataVersion = Helper.GetMetadataVersion(content),
                    ServiceUrl = fname,
                    SchemaNamespace = Helper.GetNameSpace(content),
                    MediaType = Media.Xml
                };
                Debug.WriteLine(content);
                return metaData;
            }
            return new MetaDataInfo();
        }
#endif

        /// <summary>
        /// Load Metadata from xml string
        /// </summary>
        /// <param name="xmlContent">xml string </param>
        /// <returns></returns>
        public static MetaDataInfo LoadMetaDataFromXml(string xmlContent)
        {
            var metaData = new MetaDataInfo
            {
                MetaDataAsString = xmlContent,
                MetaDataVersion = Helper.GetMetadataVersion(xmlContent),
                ServiceUrl = "",
                SchemaNamespace = Helper.GetNameSpace(xmlContent),
                MediaType = Media.Xml
            };
            //Debug.WriteLine(xmlContent);
            return metaData;
        }

        public MetaDataInfo MetaData { get; set; }
    }
}
