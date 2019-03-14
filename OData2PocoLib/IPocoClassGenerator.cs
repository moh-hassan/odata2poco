
using System.Collections.Generic;

namespace OData2Poco
{
    public interface IPocoClassGenerator
    {
        PocoSetting PocoSetting { get; set; }
        ClassTemplate this[string index] { get; }
        List<ClassTemplate> ClassList { get; } 
        string GeneratePoco();
        IDictionary<string, ClassTemplate> PocoModel { get;  }
        string PocoModelAsJson { get; }
        //Language LangName { get; set;}
    }
}


