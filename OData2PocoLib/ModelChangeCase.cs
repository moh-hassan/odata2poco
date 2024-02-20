// Copyright (c) Mohamed Hassan & Contributors. All rights reserved. See License.md in the project root for license information.

namespace OData2Poco;

using System.Text.RegularExpressions;
using Extensions;

/// <summary>
///     Change EntityName case
/// </summary>
internal sealed class ModelChangeCase
{
    private ModelChangeCase()
    {
        ClassChangedName = [];
    }

    private Dictionary<string, string> ClassChangedName { get; }

    /// <summary>
    ///     Change case of class with its parent and related Property Type
    /// </summary>
    /// <param name="list">Class list</param>
    /// <param name="caseEnum">caseEnum</param>
    /// <param name="renameMap">renameMap</param>
    /// <returns></returns>
    public static ModelChangeCase RenameClasses(List<ClassTemplate> list, CaseEnum caseEnum, RenameMap? renameMap)
    {
        ModelChangeCase cc = new();
        if (caseEnum == CaseEnum.None && renameMap is null)
        {
            return cc;
        }

        if (list.Count == 0)
        {
            return cc;
        }

        //rename classes
        list.Update(c =>
        {
            c.Name = cc.RenameClass(c.Name, caseEnum, renameMap);
            return c;
        });
        //rename Parent
        list.Update(c =>
        {
            c.BaseType = cc.RenameParent(c.BaseType);
            return c;
        });
        //rename properties type
        cc.RenamePropertiesType(list);
        return cc;
    }

    private void AddItem(string key, string value)
    {
        ClassChangedName.TryAdd(key, value);
    }

    private string RenameClass(string className, CaseEnum caseEnum, RenameMap? renameMap)
    {
        if (renameMap is { })
        {
            // It is questionable if we should do InvariantCultureIgnoreCase.
            var nameMap = renameMap.ClassNameMap
                .Find(cnm => cnm.OldName.Equals(className, StringComparison.OrdinalIgnoreCase));

            if (nameMap is not null && !string.IsNullOrWhiteSpace(nameMap.NewName))
            {
                AddItem(className, nameMap.NewName);
                return nameMap.NewName;
            }
        }

        var newClassName = className.ChangeCase(caseEnum);
        AddItem(className, newClassName);
        return newClassName;
    }

    private string RenameParent(string className)
    {
        return GetNewName(className);
    }

    private void RenamePropertiesType(List<ClassTemplate> list)
    {
        if (list.Count == 0)
        {
            return;
        }

        list.ForEach(ct => ct.Properties.Update(c => c.PropType = RenamePropertyType(c)));
    }

    private string GetNewName(string name)
    {
        if (string.IsNullOrEmpty(name))
        {
            return string.Empty;
        }

        if (ClassChangedName.TryGetValue(name, out var newName))
        {
            return newName;
        }

        //check if name contains namespace
        var index = name.LastIndexOf('.');
        if (index < 0)
        {
            return name;
        }

        var entity = name[(index + 1)..];
        return ClassChangedName.GetValueOrDefault(entity, name);
    }

    private string RenamePropertyType(PropertyTemplate prop)
    {
        var type = prop.PropType;
        const string Pattern = @"List<([\w\.]+)>";
        var m = Regex.Match(type, Pattern);
        if (m.Success)
        {
            var name = m.Groups[1].ToString();
            var newName = GetNewName(name);
            var newType = $"List<{newName}>";
            return newType;
        }

        return GetNewName(type);
    }
}
