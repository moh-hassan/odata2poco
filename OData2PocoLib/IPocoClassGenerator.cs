#if x
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OData2Poco
{
    interface IPocoClassGenerator : IEnumerable<string>
    {
        List<ClassTemplate> ClassList { get;  }
        // Indexer declaration:
        ClassTemplate this[string index]
        {
            get;
            //set;
        }
        PocoSetting PocoSetting { get; set; }
        //string GeneratePoco();
        //string ClassToString(ClassTemplate ent, bool includeNamespace = false);
        //List<ClassTemplate> GetClassTemplateList();


    }
}

#endif
