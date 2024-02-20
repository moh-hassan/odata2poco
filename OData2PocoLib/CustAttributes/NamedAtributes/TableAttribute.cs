// Copyright (c) Mohamed Hassan & Contributors. All rights reserved. See License.md in the project root for license information.

namespace OData2Poco.CustAttributes.NamedAtributes;

public class TableAttribute : INamedAttribute
{
    public string Name { get; set; } = "tab";
    public string Scope { get; set; } = "class";
    public bool IsUserDefined { get; set; }
    public bool IsValid { get; set; } = true;

    public List<string> GetAttributes(PropertyTemplate propertyTemplate)
    {
        return [];
    }

    public List<string> GetAttributes(ClassTemplate classTemplate)
    {
        _ = classTemplate ?? throw new ArgumentNullException(nameof(classTemplate));
        return !string.IsNullOrEmpty(classTemplate.EntitySetName)
            ? [$"[Table(\"{classTemplate.EntitySetName}\")]"]
            : [];
    }
}
