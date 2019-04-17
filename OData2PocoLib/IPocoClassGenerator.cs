
using System.Collections.Generic;

namespace OData2Poco
{
    public interface IPocoClassGenerator
    { 
        //string LangName { get; set;}
        PocoSetting PocoSetting { get; set; }
        List<ClassTemplate> ClassList { get; set; }
        string GeneratePoco();
       
    }

    
}


