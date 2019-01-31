using System;
using System.Xml;
using OData2Poco.V4;

namespace OData2Poco
{
    // factory class
    internal class PocoFactory
    {
        private static IPocoGenerator Create(MetaDataInfo metadata, PocoSetting setting)
        {
            if (string.IsNullOrEmpty(metadata.MetaDataAsString)) throw new InvalidOperationException("No Metadata available");

            var metaDataVersion = metadata.MetaDataVersion;  
            switch (metaDataVersion)
            {
                case ODataVersion.V4:
                    return new Poco(metadata, setting);

                case ODataVersion.V1:
                case ODataVersion.V2:
                case ODataVersion.V3:
                    return new V3.Poco(metadata, setting);
                //throw new NotImplementedException();

                default:
                    throw new NotSupportedException($"OData Version '{metaDataVersion}' is not supported");
                  
            }
        }

        public static IPocoClassGenerator GeneratePoco(MetaDataInfo metadata, PocoSetting setting = null)
        {
            if (setting == null) setting = new PocoSetting();
            if (string.IsNullOrEmpty(metadata.MetaDataAsString))
                throw new XmlException("Metaddata is empty");
            var generator = Create(metadata, setting);
            return new PocoClassGeneratorCs(generator, setting);
        }
    }
}

