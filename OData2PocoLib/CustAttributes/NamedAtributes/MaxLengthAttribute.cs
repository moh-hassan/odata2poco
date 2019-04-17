using System.Collections.Generic;

namespace OData2Poco.CustAttributes.NamedAtributes
{
    public class MaxLengthAttribute : INamedAttribute
    {
        public string Name { get;   } = "max";

        public List<string> GetAttributes(PropertyTemplate property) =>
            property.MaxLength>0 ? new List<string> { $"[MaxLength({property.MaxLength})]" } : new List<string>();

        public List<string> GetAttributes(ClassTemplate property) => new List<string>();
    }
}