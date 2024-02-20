// Copyright (c) Mohamed Hassan & Contributors. All rights reserved. See License.md in the project root for license information.

namespace OData2Poco.CustAttributes.NamedAtributes;

using Extensions;

public class DisplayAttribute : INamedAttribute
{
    public string Name { get; set; } = "display";
    public string Scope { get; set; } = "property";
    public bool IsUserDefined { get; set; }
    public bool IsValid { get; set; } = true;

    public List<string> GetAttributes(PropertyTemplate propertyTemplate)
    {
        _ = propertyTemplate ?? throw new ArgumentNullException(nameof(propertyTemplate));
        return [$"[Display(Name = {propertyTemplate.PropName.ToTitle().Quote()})]"];
    }

    public List<string> GetAttributes(ClassTemplate classTemplate)
    {
        return [];
    }
}
