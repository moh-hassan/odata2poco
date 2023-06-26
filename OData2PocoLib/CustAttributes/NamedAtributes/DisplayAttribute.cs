﻿// Copyright (c) Mohamed Hassan & Contributors. All rights reserved. See License.md in the project root for license information.

using OData2Poco.Extensions;

// ReSharper disable UnusedMember.Global

namespace OData2Poco.CustAttributes.NamedAtributes;

public class DisplayAttribute : INamedAttribute
{
    public string Name { get; set; } = "display";
    public string Scope { get; set; } = "property";
    public bool IsUserDefined { get; set; } = false;
    public bool IsValid { get; set; } = true;

    public List<string> GetAttributes(PropertyTemplate property)
    {
        return new() { $"[Display(Name = {property.PropName.ToTitle().Quote()})]" };
    }

    public List<string> GetAttributes(ClassTemplate classTemplate)
    {
        return new();
    }
}