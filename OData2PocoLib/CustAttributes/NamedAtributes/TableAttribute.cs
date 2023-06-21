// Copyright (c) Mohamed Hassan & Contributors. All rights reserved. See License.md in the project root for license information.

#pragma warning disable S125

namespace OData2Poco.CustAttributes.NamedAtributes;

public class TableAttribute : INamedAttribute
{
    public string Name { get; set; } = "tab"; //"table";
    public string Scope { get; } = "class";
    public bool IsUserDefined { get; } = false;
    public List<string> GetAttributes(PropertyTemplate property)
    {
        return new();
    }

    public List<string> GetAttributes(ClassTemplate classTemplate)
    {
        return !string.IsNullOrEmpty(classTemplate.EntitySetName)
            ? new List<string> { $"[Table(\"{classTemplate.EntitySetName}\")]" }
            : new List<string>();
    }
}