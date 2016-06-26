using System;
using OData2Poco.V4;

namespace OData2Poco
{
    // factory class
    internal class PocoFactory
    {
        public static IPocoGenerator Create(MetaDataInfo metadata)
        {
            if (string.IsNullOrEmpty(metadata.MetaDataAsString)) throw new InvalidOperationException("No Metadata available");

            var metaDataVersion = metadata.MetaDataVersion;  
            switch (metaDataVersion)
            {
                case ODataVersion.V4:
                    return new Poco(metadata);

                case ODataVersion.V1:
                case ODataVersion.V2:
                case ODataVersion.V3:
                    return new V3.Poco(metadata);
                //throw new NotImplementedException();

                default:
                    throw new NotSupportedException(string.Format("OData Version '{0}' is not supported", metaDataVersion));
                  
            }
        }
    }
}
