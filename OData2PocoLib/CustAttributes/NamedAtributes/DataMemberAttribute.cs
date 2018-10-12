using System.Collections.Generic;

namespace OData2Poco.CustAttributes.NamedAtributes
{
  public  class DataMemberAttribute : INamedAttribute
    {
        public string Name { get;   } ="dm" ;//"datamember";
        public List<string> GetAttributes(PropertyTemplate property) => new List<string> {"[DataMember]"};
        public List<string> GetAttributes(ClassTemplate property) => new List<string> {"[DataContract]"};

}
}
