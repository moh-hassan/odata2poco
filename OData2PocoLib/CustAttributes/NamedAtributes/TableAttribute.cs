// Copyright (c) Mohamed Hassan & Contributors. All rights reserved. See License.md in the project root for license information.


namespace OData2Poco.CustAttributes.NamedAtributes;

public class TableAttribute : INamedAttribute
{
    public string Name { get; set; } = "tab"; //"table";
    public string Scope { get; set; } = "class";
    public bool IsUserDefined { get; set; } = false;
    public bool IsValid { get; set; } = true;
    public List<string> GetAttributes(PropertyTemplate property)
    {
        return [];
    }

    public List<string> GetAttributes(ClassTemplate classTemplate)
    {
        return !string.IsNullOrEmpty(classTemplate.EntitySetName)
            ? [$"[Table(\"{classTemplate.EntitySetName}\")]"]
            : [];
    }
}