#if x
//not planned yet
using System;
using System.Collections;
using System.Collections.Generic;

//not implemented , not planned yet
namespace OData2Poco
{
    internal class PocoClassGeneratorVb : IPocoClassGenerator, IEnumerable<string>
    {
        public PocoClassGeneratorVb(IPocoGenerator pocoFactory, PocoSetting setting)
        {
            throw new NotImplementedException();
        }


        public IEnumerator<string> GetEnumerator()
        {
            throw new NotImplementedException();
             
        }

      

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public List<ClassTemplate> ClassList { get; set; }

        public ClassTemplate this[string index]
        {
            get { throw new NotImplementedException(); }
        }

        public PocoSetting PocoSetting { get; set; }
        public string GeneratePoco()
        {
            throw new NotImplementedException();
        }

        public string ClassToString(ClassTemplate ent, bool includeNamespace = false)
        {
            throw new NotImplementedException();
        }

        public List<ClassTemplate> GetClassTemplateList()
        {
            throw new NotImplementedException();
        }
    }
}


#endif