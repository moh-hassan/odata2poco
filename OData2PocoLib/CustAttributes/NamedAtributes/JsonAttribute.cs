using System.Collections.Generic;
using OData2Poco.Extension;

namespace OData2Poco.CustAttributes.NamedAtributes
{
  public  class JsonAttribute : INamedAttribute
    {
        public string Name { get;  } = "json";
        public List<string> GetAttributes(PropertyTemplate property) => 
            new List<string> { $"[JsonProperty(PropertyName = {property.PropName.Quote()})]"};

        public List<string> GetAttributes(ClassTemplate property) => new List<string>();
    }
}
