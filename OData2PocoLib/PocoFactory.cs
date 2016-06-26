using System;
using OData2Poco.V4;

namespace OData2Poco
{
    // factory class
    internal class PocoFactory
    {
        //private string MetaDataAsString { get; set; }
        //public string ServiceUrl { get; set; }
        //public PocoFactory( string metaDataAsString,string url)
        //{
        //    MetaDataAsString = metaDataAsString;
        //    ServiceUrl = url;
        //}

 

     //   public IPocoGenerator SelectPocoGenerator(string metaDataAsString)
        public static IPocoGenerator Create(string metaDataAsString, string serviceUrl)
        {
            if (string.IsNullOrEmpty(metaDataAsString)) throw new InvalidOperationException("No Metadata available"); 

            var metaDataVersion = Helper.GetMetadataVersion(metaDataAsString);
            switch (metaDataVersion)
            {
                case ODataVersion.V4:
                    return new Poco(metaDataAsString, serviceUrl);
                   
                case ODataVersion.V1:
                case ODataVersion.V2:
                case ODataVersion.V3:
                    return new V3.Poco(metaDataAsString, serviceUrl);
                   
                //throw new NotImplementedException();

                default:
                    throw new NotSupportedException(string.Format("OData Version '{0}' is not supported", metaDataVersion));
                  
            }
        }
    }
}
