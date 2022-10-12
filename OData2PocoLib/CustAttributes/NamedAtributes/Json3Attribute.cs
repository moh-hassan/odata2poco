// Copyright (c) Mohamed Hassan & Contributors. All rights reserved. See License.md in the project root for license information.

using OData2Poco.Extensions;

// ReSharper disable UnusedMember.Global

namespace OData2Poco.CustAttributes.NamedAtributes;

public class Json3Attribute : INamedAttribute
{
    public string Name { get; } = "json3";

    public List<string> GetAttributes(PropertyTemplate property)
    {
        return string.IsNullOrEmpty(property.OriginalName) || property.OriginalName == property.PropName
            ? new List<string> { $"[JsonPropertyName({property.PropName.Quote()})]" }
            : new List<string>();
    }

    public List<string> GetAttributes(ClassTemplate classTemplate)
    {
        return new();
    }
}