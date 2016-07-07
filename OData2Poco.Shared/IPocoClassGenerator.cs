
using System.Collections.Generic;

namespace OData2Poco
{
    interface IPocoClassGenerator
    {
        PocoSetting PocoSetting { get; set; }
        ClassTemplate this[string index] { get; }
        List<ClassTemplate> ClassList { get; } //
        string GeneratePoco();
        
    }
}


