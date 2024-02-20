// Copyright (c) Mohamed Hassan & Contributors. All rights reserved. See License.md in the project root for license information.

namespace OData2Poco.CustAttributes.NamedAtributes;

public class ProtoAttribute : INamedAttribute
{
    public string Name { get; set; } = "proto";
    public string Scope { get; set; } = "dual";
    public bool IsUserDefined { get; set; }
    public bool IsValid { get; set; } = true;

    public List<string> GetAttributes(PropertyTemplate propertyTemplate)
    {
        _ = propertyTemplate ?? throw new ArgumentNullException(nameof(propertyTemplate));
        return [$"[ProtoMember({propertyTemplate.Serial})]"];
    }

    public List<string> GetAttributes(ClassTemplate classTemplate)
    {
        return ["[ProtoContract]"];
    }
}
