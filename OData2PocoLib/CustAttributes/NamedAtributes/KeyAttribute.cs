// Copyright (c) Mohamed Hassan & Contributors. All rights reserved. See License.md in the project root for license information.

namespace OData2Poco.CustAttributes.NamedAtributes;

public class KeyAttribute : INamedAttribute
{
    public string Name { get; } = "key";
    public string Scope { get; } = "property";
    public bool IsUserDefined { get; } = false;
    public List<string> GetAttributes(PropertyTemplate property)
    {
        return property.IsKey ? new List<string> { "[Key]" } : new List<string>();
    }

    public List<string> GetAttributes(ClassTemplate classTemplate)
    {
        return new();
    }
}