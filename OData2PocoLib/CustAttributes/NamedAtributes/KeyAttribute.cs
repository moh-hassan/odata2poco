using System.Collections.Generic;

namespace OData2Poco.CustAttributes.NamedAtributes
{
    public class KeyAttribute : INamedAttribute
    {
        public string Name { get;   } = "key";

        public List<string> GetAttributes(PropertyTemplate property) =>
            property.IsKey ? new List<string> { "[Key]" } : new List<string>();

        public List<string> GetAttributes(ClassTemplate property) => new List<string>();
    }
}