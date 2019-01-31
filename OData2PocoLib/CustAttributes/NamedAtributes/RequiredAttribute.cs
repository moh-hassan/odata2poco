using System.Collections.Generic;

namespace OData2Poco.CustAttributes.NamedAtributes
{
  public  class RequiredAttribute : INamedAttribute
    {
        public string Name { get; } = "req";

        public List<string> GetAttributes(PropertyTemplate property) =>
            property.IsNullable ? new List<string>() : new List<string> {"[Required]"};

        public List<string> GetAttributes(ClassTemplate property) => new List<string>();
    }
}
