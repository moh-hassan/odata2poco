using System.Collections.Generic;
using OData2Poco.Extension;

namespace OData2Poco.CustAttributes.NamedAtributes
{
 public   class DisplayAttribute : INamedAttribute
    {
        public string Name { get;   } = "display";

        public List<string> GetAttributes(PropertyTemplate property) => 
            new List<string> { $"[Display(Name = {property.PropName.ToTitle().Quote()})]" };

        public List<string> GetAttributes(ClassTemplate property) => new List<string>();
    }
}
