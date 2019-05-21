using System.Collections.Generic;

namespace OData2Poco.CustAttributes.NamedAtributes
{
    public class MyKeyAttribute : INamedAttribute
    {
        public string Name { get;   } = "mykey";

        public List<string> GetAttributes(PropertyTemplate property) =>
            property.IsKey ? new List<string> { "[MyKey]" } : new List<string>();

        public List<string> GetAttributes(ClassTemplate property) => new List<string>();
    }
}