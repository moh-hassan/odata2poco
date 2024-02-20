// Copyright (c) Mohamed Hassan & Contributors. All rights reserved. See License.md in the project root for license information.

namespace OData2Poco;

using System.Text.RegularExpressions;
using Extensions;

/// <summary>
///     handle reserved cs keywords
/// </summary>
internal static class ModelManager
{
    static ModelManager()
    {
        ClassChangedName = [];
        ModelWarning = [];
    }

    public static List<string> ModelWarning { get; set; }

    private static Dictionary<string, string> ClassChangedName { get; }

    public static void RenameReservedWords(List<ClassTemplate> list)
    {
        if (list.Count == 0)
        {
            return;
        }

        list.Update(c => c.Name = RenameClass(c.Name));
        list.ForEach(classTemplate => classTemplate.Properties.Update(RenameProperty));
        ModifyPropertiesType(list);
    }

    public static void AddItem(string key, string value)
    {
        ClassChangedName.TryAdd(key, value);
    }

    public static string RenameClass(string className)
    {
        if (!className.IsCSharpReservedWord())
        {
            return className;
        }

        var newClassName = className.ToggleFirstLetter();
        ModelWarning.Add($"The class: '{className}' is a reserved keyword. It's renamed to '{newClassName}'");
        AddItem(className, newClassName);
        return newClassName;
    }

    public static void ModifyPropertiesType(List<ClassTemplate> list)
    {
        list.ForEach(ct => ct.Properties.Update(c => c.PropType = ModifyPropertyType(c)));
    }

    public static PropertyTemplate RenameProperty(PropertyTemplate property)
    {
        string newName;
        if (property.PropName == property.ClassName)
        {
            //issue12, property name is the same as class name
            //error CS0542: '<PropName>': member names cannot be the same as their enclosing type
            newName = property.PropName.ToggleFirstLetter();
            ModelWarning.Add(
                $"Rename the property '{property.ClassName}.{property.PropName}' to '{newName}' for  avoiding the Compiler error CS0542 ");
            property.PropName = newName;

            property.PropComment += "//Renamed";
        }

        //check if property is reserved keyword
        if (!property.PropName.IsCSharpReservedWord())
        {
            return property;
        }

        newName = property.PropName.ToggleFirstLetter();
        ModelWarning.Add(
            $"Rename the property {property.ClassName}.{property.PropName} to '{newName}' becauuse its name is a reserved keyword");
        property.PropName = newName;
        return property;
    }

    private static string ModifyPropertyType(PropertyTemplate prop)
    {
        var type = prop.PropType;
        const string Pattern = @"List<([\w\.]+)>";
        var m = Regex.Match(type, Pattern);
        string? newType;
        if (m.Success)
        {
            var name = m.Groups[1].ToString();
            if (!ClassChangedName.TryGetValue(name, out var value))
            {
                return type;
            }

            newType = $"List<{value}>";
            ModelWarning.Add(
                $"Modify the type of the property: '{prop.ClassName}.{prop.PropName}' from  {type} to {newType}");
            return newType;
        }

        if (!ClassChangedName.TryGetValue(type, out newType))
        {
            return type;
        }

        ModelWarning.Add(
            $"Modify the type of the property: '{prop.ClassName}.{prop.PropName}' from  {type} to {newType}");
        return newType;
    }
}
