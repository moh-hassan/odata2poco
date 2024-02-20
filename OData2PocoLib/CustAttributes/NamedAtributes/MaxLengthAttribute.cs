// Copyright (c) Mohamed Hassan & Contributors. All rights reserved. See License.md in the project root for license information.

namespace OData2Poco.CustAttributes.NamedAtributes;

public class MaxLengthAttribute : INamedAttribute
{
    public string Name { get; set; } = "max";
    public string Scope { get; set; } = "property";
    public bool IsUserDefined { get; set; }
    public bool IsValid { get; set; } = true;

    public List<string> GetAttributes(PropertyTemplate propertyTemplate)
    {
        _ = propertyTemplate ?? throw new ArgumentNullException(nameof(propertyTemplate));
        return propertyTemplate.MaxLength > 0 ? [$"[MaxLength({propertyTemplate.MaxLength})]"] : [];
    }

    public List<string> GetAttributes(ClassTemplate classTemplate)
    {
        return [];
    }
}
