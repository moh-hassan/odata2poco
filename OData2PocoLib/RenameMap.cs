// Copyright (c) Mohamed Hassan & Contributors. All rights reserved. See License.md in the project root for license information.

namespace OData2Poco;

public class RenameMap
{
    // This is a list so we have the option to control how we match
    // the OldName.
    public List<ClassNameMap> ClassNameMap { get; set; } = new();

    public Dictionary<string, List<PropertyNameMap>> PropertyNameMap { get; set; } = new();
}

public class ClassNameMap
{
    public string OldName { get; set; } = string.Empty;

    public string NewName { get; set; } = string.Empty;
}

public class PropertyNameMap
{
    public string OldName { get; set; } = string.Empty;

    public string NewName { get; set; } = string.Empty;
}