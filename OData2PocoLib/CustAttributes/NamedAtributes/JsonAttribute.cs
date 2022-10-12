// Copyright (c) Mohamed Hassan & Contributors. All rights reserved. See License.md in the project root for license information.

using OData2Poco.Extensions;

namespace OData2Poco.CustAttributes.NamedAtributes;

public class JsonAttribute : INamedAttribute
{
    public string Name => "json";

    public List<string> GetAttributes(PropertyTemplate property)
    {
        return string.IsNullOrEmpty(property.OriginalName) || property.OriginalName == property.PropName
            ? new List<string> { $"[JsonProperty(PropertyName = {property.PropName.Quote()})]" }
            : new List<string>();
    }

    public List<string> GetAttributes(ClassTemplate classTemplate)
    {
        return new();
    }
}