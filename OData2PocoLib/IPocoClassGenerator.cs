
using OData2Poco.TypeScript;
using System.Collections.Generic;

namespace OData2Poco
{
    public interface IPocoClassGeneratorMultiFiles
    {         
        PocoStore GeneratePoco();
    }
    public interface IPocoClassGenerator
    {
        PocoSetting PocoSetting { get; set; }
        List<ClassTemplate> ClassList { get; set; }
        string GeneratePoco();
    }
     
}


