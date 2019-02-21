using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using OData2Poco.Extensions;
using OData2Poco.OAuth2;

namespace OData2Poco
{
    internal class MetaDataReader
    {
        public static async Task<MetaDataInfo> LoadMetaDataHttpAsync(OdataConnectionString odataConnString)
        {
            // to avoid the Error Message:
            //An error occurred while sending the request.-->
            //The underlying connection was closed: An unexpected error occurred on a send. --
            //->Unable to read data from the transport connection: An existing connection was forcibly closed by the remote host. --->An existing connection was forcibly closed by the remote host--->

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 
                                                   | SecurityProtocolType.Tls11 
                                                   | SecurityProtocolType.Tls;


            Uri serviceUri = new Uri(odataConnString.ServiceUrl);
            using (var client = new HttpClient())
            {
                await new Authenticator(client).Authenticate(odataConnString);


                client.DefaultRequestHeaders.Add("User-Agent", "OData2Poco");
                string url = serviceUri.AbsoluteUri.TrimEnd('/') + "/$metadata";

                using (HttpResponseMessage response = await client.GetAsync(url))
                {

                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        //Debug.WriteLine(content);
                        if (!string.IsNullOrEmpty(content))
                        {

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
                                metaData.ServiceHeader.Add(key, value);
                            }
                            metaData.ServiceVersion = Helper.GetServiceVersion(metaData.ServiceHeader);
                            return metaData;
                        }

                    }
                    //Debug.WriteLine(response.ReasonPhrase);
                    throw new HttpRequestException("Http Error " + (int)response.StatusCode + ": " +
                                                   response.ReasonPhrase);


                }
            }
        }

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


        public static async Task<MetaDataInfo> LoadMetadataAsync(OdataConnectionString odataConnString)
        {
            MetaDataInfo MetaData= new MetaDataInfo();
            if (!odataConnString.ServiceUrl.StartsWith("http"))
            {
                //return await GenerateFromFileAsync(odataConnString.ServiceUrl);
                string xml;
                using (StreamReader reader = new StreamReader(odataConnString.ServiceUrl))
                {
                    xml = await reader.ReadToEndAsync();
                     MetaData = LoadMetaDataFromXml(xml);
                    return MetaData;
                }
            }

            MetaData = await LoadMetaDataHttpAsync(odataConnString);
            return MetaData;
        }


        //public MetaDataInfo MetaData { get; set; }
    }
}
