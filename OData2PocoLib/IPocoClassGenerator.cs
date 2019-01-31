
using System.Collections.Generic;

namespace OData2Poco
{
    public interface IPocoClassGenerator
    {
        PocoSetting PocoSetting { get; set; }
        ClassTemplate this[string index] { get; }
        List<ClassTemplate> ClassList { get; } //
        string GeneratePoco();
        //v2.2 to support exporting model as json
        IDictionary<string, ClassTemplate> PocoModel { get;  }
        string PocoModelAsJson { get; }
    }
}


