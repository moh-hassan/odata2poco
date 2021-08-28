using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using OData2Poco.Http;
using OData2Poco.InfraStructure.Logging;

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




            var client = new CustomeHttpClient(odataConnString);
            var content = await client.ReadMetaDataAsync();

            var metaData = new MetaDataInfo
            {
                MetaDataAsString = content,
                MetaDataVersion = Helper.GetMetadataVersion(content),
                ServiceUrl = client.ServiceUri.OriginalString,
                SchemaNamespace = Helper.GetNameSpace(content),
                MediaType = Media.Http,
            };
            if (client.Response != null)
                foreach (var entry in client.Response.Headers)
                {
                    var value = entry.Value.FirstOrDefault();
                    if (value == null) continue;
                    string key = entry.Key;
                    metaData.ServiceHeader.Add(key, value);
                }

            metaData.ServiceVersion = Helper.GetServiceVersion(metaData.ServiceHeader);
            return metaData;
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
                using StreamReader reader = new StreamReader(odataConnString.ServiceUrl);
                var xml = await reader.ReadToEndAsync();
                metaData = LoadMetaDataFromXml(xml);
                metaData.ServiceUrl = odataConnString.ServiceUrl;
                return metaData;
            }

            metaData = await LoadMetaDataHttpAsync(odataConnString);
            return metaData;
        }
    }
}
