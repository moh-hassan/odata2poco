using System.Collections.Generic;

namespace OData2Poco
{
    public interface IPocoGenerator
    {
        
          string MetaDataAsString { get; set; }
          string MetaDataVersion { get; set; }
          string ServiceUrl { get; set; }
        List<ClassTemplate> GeneratePocoList();
    }
}
