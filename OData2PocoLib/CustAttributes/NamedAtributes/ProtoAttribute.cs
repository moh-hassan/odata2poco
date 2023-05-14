// Copyright (c) Mohamed Hassan & Contributors. All rights reserved. See License.md in the project root for license information.


namespace OData2Poco.CustAttributes.NamedAtributes;

public class ProtoAttribute : INamedAttribute
{
    public string Name { get; } = "proto";

    public List<string> GetAttributes(PropertyTemplate property)
    {
        return new() { $"[ProtoMember({property.Serial})]" };
    }

    public List<string> GetAttributes(ClassTemplate classTemplate)
    {
        return new() { "[ProtoContract]" };
    }
}