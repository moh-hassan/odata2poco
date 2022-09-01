using System.Collections.Generic;

namespace OData2Poco
{
    public interface IPocoGenerator
    {
        MetaDataInfo MetaData { get; set; }
        List<ClassTemplate> GeneratePocoList();
        string GenerateOpenApi(int openApiVersion = 3);
    }
}
