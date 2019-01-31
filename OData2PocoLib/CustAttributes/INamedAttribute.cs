using System.Collections.Generic;

namespace OData2Poco.CustAttributes
{
    public interface INamedAttribute
    {
        string Name { get; }
        List<string> GetAttributes(PropertyTemplate property);
        List<string> GetAttributes(ClassTemplate property);
    }
}
