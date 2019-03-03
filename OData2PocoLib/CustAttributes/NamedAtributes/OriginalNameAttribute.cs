using System.Collections.Generic;

namespace OData2Poco.CustAttributes.NamedAtributes
{
    public class OriginalNameAttribute : INamedAttribute
    {
        public string Name { get; } = "origin";

        public List<string> GetAttributes(PropertyTemplate property) =>
            new List<string> {
                property.OriginalName !=property.PropName ? $"//[OriginalName({property.OriginalName})]":"",
                };

        public List<string> GetAttributes(ClassTemplate classTemplate) =>
            new List<string>
            {
               classTemplate.OriginalName !=classTemplate.Name
                ? $"//[OriginalName({classTemplate.OriginalName})]"
                :"",
                };
    }
}

