// Copyright (c) Mohamed Hassan & Contributors. All rights reserved. See License.md in the project root for license information.



// ReSharper disable UnusedMember.Global

namespace OData2Poco.CustAttributes.NamedAtributes;

public class MaxLengthAttribute : INamedAttribute
{
    public string Name { get; } = "max";

    public List<string> GetAttributes(PropertyTemplate property)
    {
        return property.MaxLength > 0 ? new List<string> { $"[MaxLength({property.MaxLength})]" } : new List<string>();
    }

    public List<string> GetAttributes(ClassTemplate classTemplate)
    {
        return new();
    }
}