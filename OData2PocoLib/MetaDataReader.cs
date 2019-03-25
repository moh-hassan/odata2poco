using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using OData2Poco.InfraStructure.Logging;
using OData2Poco.OAuth2;

namespace OData2Poco
{
    internal class MetaDataReader
    {
        public static ILog Logger = PocoLogger.Default;
        public static async Task<MetaDataInfo> LoadMetaDataHttpAsync(OdataConnectionString odataConnString)
        {
            // to avoid the Error Message://An error occurred while sending the request.-->
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12
                                                   | SecurityProtocolType.Tls11
                                                   | SecurityProtocolType.Tls;


            var serviceUri = new Uri(odataConnString.ServiceUrl);
            CredentialCache credentials = new CredentialCache();
            switch (odataConnString.Authenticate)
            {
                case AuthenticationType.Ntlm:
                    Logger.Trace("Authenticating with NTLM");
                    credentials.Add(serviceUri, "NTLM",
                        new NetworkCredential(odataConnString.UserName, odataConnString.Password, odataConnString.Domain));
                    break;
                case AuthenticationType.Digest:
                    Logger.Trace("Authenticating with Digest");
                    credentials.Add(serviceUri, "Digest",
                        new NetworkCredential(odataConnString.UserName, odataConnString.Password, odataConnString.Domain));
                    break;
            }
            //UseDefaultCredentials for NTLM support in windows
            var handler = new HttpClientHandler()
            {
                UseDefaultCredentials = true,
                Credentials = credentials,
            };

            if (!string.IsNullOrEmpty(odataConnString.Proxy))
            {
                Logger.Trace($"Using Proxy: '{odataConnString.Proxy}'");
                handler.UseProxy=true;
                handler.Proxy=   new WebProxy(odataConnString.Proxy);
            }

            using (var client = new HttpClient(handler))
            {
                var auth = new Authenticator(client);
                //authenticate
                await auth.Authenticate(odataConnString);
                client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json; charset=utf-8");
                client.DefaultRequestHeaders.Add("User-Agent", "OData2Poco");
                string url = serviceUri.AbsoluteUri.TrimEnd('/') + "/$metadata";

                using (HttpResponseMessage response = await client.GetAsync(url))
                {
                    if (!response.IsSuccessStatusCode)
                        throw new HttpRequestException(
                            $"Http Error {(int)response.StatusCode}: {response.ReasonPhrase}");
                    var content = await response.Content.ReadAsStringAsync();
                    if (string.IsNullOrEmpty(content))
                        throw new HttpRequestException(
                            $"Http Error {(int)response.StatusCode}: {response.ReasonPhrase}");
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
