// Copyright (c) Mohamed Hassan & Contributors. All rights reserved. See License.md in the project root for license information.

namespace OData2Poco.CustAttributes.NamedAtributes;

using Extensions;

public class JsonAttribute : INamedAttribute
{
    public string Name { get; set; } = "json";
    public string Scope { get; set; } = "property";
    public bool IsUserDefined { get; set; }
    public bool IsValid { get; set; } = true;

    public List<string> GetAttributes(PropertyTemplate propertyTemplate)
    {
        _ = propertyTemplate ?? throw new ArgumentNullException(nameof(propertyTemplate));
        return string.IsNullOrEmpty(propertyTemplate.OriginalName) || propertyTemplate.OriginalName == propertyTemplate.PropName
            ? [$"[JsonProperty(PropertyName = {propertyTemplate.PropName.Quote()})]"]
            : [];
    }

    public List<string> GetAttributes(ClassTemplate classTemplate)
    {
        return [];
    }
}
