// Copyright (c) Mohamed Hassan & Contributors. All rights reserved. See License.md in the project root for license information.



// ReSharper disable UnusedMember.Global

namespace OData2Poco.CustAttributes.NamedAtributes;

public class MaxLengthAttribute : INamedAttribute
{
    public string Name { get; set; } = "max";
    public string Scope { get; set; } = "property";
    public bool IsUserDefined { get; set; } = false;
    public bool IsValid { get; set; } = true;
    public List<string> GetAttributes(PropertyTemplate property)
    {
        return property.MaxLength > 0 ? [$"[MaxLength({property.MaxLength})]"] : [];
    }

    public List<string> GetAttributes(ClassTemplate classTemplate)
    {
        return [];
    }
}