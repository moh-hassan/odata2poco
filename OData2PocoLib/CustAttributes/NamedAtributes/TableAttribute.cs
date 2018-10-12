using System.Collections.Generic;

namespace OData2Poco.CustAttributes.NamedAtributes
{
    public class TableAttribute : INamedAttribute
    {
        public string Name { get; set; } = "tab"; //"table";

        public List<string> GetAttributes(PropertyTemplate property) => new List<string>();

        public List<string> GetAttributes(ClassTemplate property)
        {
            
            return !string.IsNullOrEmpty(property.EntitySetName)
                ? new List<string> {$"[Table(\"{property.EntitySetName}\")]"}
                : new List<string>();
        }
    }
}