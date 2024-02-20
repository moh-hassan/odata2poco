// Copyright (c) Mohamed Hassan & Contributors. All rights reserved. See License.md in the project root for license information.

namespace OData2Poco.CustAttributes.NamedAtributes;

public class DataMemberAttribute : INamedAttribute
{
    public string Name { get; set; } = "dm";
    public string Scope { get; set; } = "dual";
    public bool IsUserDefined { get; set; }
    public bool IsValid { get; set; } = true;

    public List<string> GetAttributes(PropertyTemplate propertyTemplate)
    {
        return ["[DataMember]"];
    }

    public List<string> GetAttributes(ClassTemplate classTemplate)
    {
        return ["[DataContract]"];
    }
}
