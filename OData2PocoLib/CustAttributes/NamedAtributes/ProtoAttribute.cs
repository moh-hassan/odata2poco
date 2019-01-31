using System.Collections.Generic;

namespace OData2Poco.CustAttributes.NamedAtributes
{
    public class ProtoAttribute : INamedAttribute
    {
        public string Name { get; } = "proto";

        public List<string> GetAttributes(PropertyTemplate property) => 
            new List<string> { $"[ProtoMember({property.Serial})]"};

        public List<string> GetAttributes(ClassTemplate property) => new List<string> { "[ProtoContract]"};
    }
}