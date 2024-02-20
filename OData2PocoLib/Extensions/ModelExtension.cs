// Copyright (c) Mohamed Hassan & Contributors. All rights reserved. See License.md in the project root for license information.

namespace OData2Poco.Extensions;

using System.Text;
using System.Text.RegularExpressions;
using Graphs;

public static class ModelExtension
{
    public static StringBuilder GetImports(this ClassTemplate ct, IEnumerable<ClassTemplate> model, PocoSetting setting)
    {
        StringBuilder imports = new();
        var allDeps = Dependency.Search(model, ct);
        foreach (var item in allDeps)
        {
            var fileName = item.GlobalName(setting);
            imports.AppendLine($"import {{{item.GlobalName(setting)}}} from './{fileName}';");
        }

        imports.AppendLine();
        return imports;
    }

    //Use FullName or name according to PocoSetting.UseFullName
    public static string GlobalName(this ClassTemplate ct, PocoSetting setting)
    {
        _ = ct ?? throw new ArgumentNullException(nameof(ct));
        _ = setting ?? throw new ArgumentNullException(nameof(setting));
        return setting.UseFullName ? ct.FullName.RemoveDot() : ct.Name;
    }

    internal static string GetGenericBaseType(this string type)
    {
        const string Pattern = "^List<(.+)>$";
        var match = Regex.Match(type, Pattern);
        return match.Success ? match.Groups[1].Value : type;
    }

    internal static ClassTemplate?
        BaseTypeToClassTemplate(this ClassTemplate ct, IEnumerable<ClassTemplate> model)
    {
        return ct.BaseType.ToClassTemplate(model);
    }

    internal static ClassTemplate? ToClassTemplate(this string type, IEnumerable<ClassTemplate> list)
    {
        return list.FirstOrDefault(a => a.FullName == type);
    }

    internal static string TailComment(this PropertyTemplate property)
    {
        List<string> comments = [];
        if (!property.IsNullable)
        {
            comments.Add("Not null");
        }

        if (property.IsKey)
        {
            comments.Add("Primary key");
        }

        if (property.IsReadOnly)
        {
            comments.Add("ReadOnly");
        }

        if (property.IsNavigate)
        {
            comments.Add("navigator");
        }

        return comments.Count > 0 ? " //" + string.Join(", ", comments) : string.Empty;
    }
}
