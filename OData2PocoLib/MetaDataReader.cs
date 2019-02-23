using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using OData2Poco.OAuth2;

namespace OData2Poco
{
    internal class MetaDataReader
    {
        public static async Task<MetaDataInfo> LoadMetaDataHttpAsync(OdataConnectionString odataConnString)
        {
            // to avoid the Error Message://An error occurred while sending the request.-->
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12
                                                   | SecurityProtocolType.Tls11
                                                   | SecurityProtocolType.Tls;


            var serviceUri = new Uri(odataConnString.ServiceUrl);
            using (var client = new HttpClient())
            {
                await new Authenticator(client).Authenticate(odataConnString);
                client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json; charset=utf-8");
                client.DefaultRequestHeaders.Add("User-Agent", "OData2Poco");
                string url = serviceUri.AbsoluteUri.TrimEnd('/') + "/$metadata";

                using (HttpResponseMessage response = await client.GetAsync(url))
                {
                    if (!response.IsSuccessStatusCode)
                        throw new HttpRequestException(
                            $"Http Error {(int) response.StatusCode}: {response.ReasonPhrase}");
                    var content = await response.Content.ReadAsStringAsync();
                    if (string.IsNullOrEmpty(content))
                        throw new HttpRequestException(
                            $"Http Error {(int) response.StatusCode}: {response.ReasonPhrase}");
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
            return metaData;
        }
        public static async Task<MetaDataInfo> LoadMetadataAsync(OdataConnectionString odataConnString)
        {
            MetaDataInfo metaData;
            if (!odataConnString.ServiceUrl.StartsWith("http"))
            {
                using (StreamReader reader = new StreamReader(odataConnString.ServiceUrl))
                {
                    var xml = await reader.ReadToEndAsync();
                    metaData = LoadMetaDataFromXml(xml);
                    return metaData;
                }
            }

            metaData = await LoadMetaDataHttpAsync(odataConnString);
            return metaData;
        }
    }
}
