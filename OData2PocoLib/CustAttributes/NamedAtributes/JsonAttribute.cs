using System.Collections.Generic;
using OData2Poco.Extensions;

namespace OData2Poco.CustAttributes.NamedAtributes
{
  public  class JsonAttribute : INamedAttribute
    {
        public string Name { get;  } = "json";
        public List<string> GetAttributes(PropertyTemplate property) => 
           string.IsNullOrEmpty(property.OriginalName) || property.OriginalName == property.PropName
        ? new List<string> { $"[JsonProperty(PropertyName = {property.PropName.Quote()})]"}
        : new List<string>();

        public List<string> GetAttributes(ClassTemplate property) => new List<string>();
    }
}
