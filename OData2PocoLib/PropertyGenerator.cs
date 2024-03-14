// Copyright (c) Mohamed Hassan & Contributors. All rights reserved. See License.md in the project root for license information.

namespace OData2Poco;

using System.Text;
using System.Text.RegularExpressions;
using CustAttributes;
using Extensions;

/// <summary>
///     Generate All attributes of property :KeyAttribut,Required,JsonProperty
///     Convert name to Camel/Pascal Case
///     Generate the declaration of property e.g.   virtual public int? name  {get;set;} //comment
/// </summary>
internal partial class PropertyGenerator
{
    private readonly AttributeFactory _attributeManager = AttributeFactory.Default;
    private readonly string _nl = Environment.NewLine;
    private readonly PropertyTemplate _property;
    private readonly PocoSetting _setting;

    /// <summary>
    ///     Initialize in cto
    /// </summary>
    /// <param name="propertyTemplate"></param>
    /// <param name="pocoSetting"></param>
    public PropertyGenerator(PropertyTemplate propertyTemplate, PocoSetting pocoSetting)
    {
        _property = propertyTemplate;
        _setting = pocoSetting;
    }

    /// <summary>
    ///     Name in camlcase /pascase
    /// </summary>
    public string Name
    {
        get
        {
            var mappedName = MappedPropertyName();
            return mappedName ?? _setting.NameCase switch
            {
                CaseEnum.Pas => _property.PropName.ToPascalCase(),
                CaseEnum.Camel => _property.PropName.ToCamelCase(),
                CaseEnum.None => _property.PropName,
                _ => _property.PropName
            };
        }
    }

    /// <summary>
    ///     Virtual Modifier
    /// </summary>
    public string VirtualModifier
    {
        get
        {
            var virtualModifier = _setting is { AddNavigation: true, AddEager: false }
                ? " virtual"
                : string.Empty;
            return _setting.GeneratorType == GeneratorType.Record
                ? string.Empty
                : virtualModifier;
        }
    }

    public string Declaration
    {
        get
        {
            var setter = "{get;set;}";
            if (_setting.InitOnly)
            {
                setter = "{get;init;}";
            }

            return new StringBuilder()
                .Append($"public{VirtualModifier}")
                .Append(' ')
                .Append(ReducedPropertyTypeName)
                .Append(GetNullableModifier())
                .Append(' ')
                .Append(Name)
                .Append(' ')
                .Append(_property.IsReadOnly && !_setting.ReadWrite ? "{get;}" : setter)
                .Append(' ')
                .Append(Comment())
                .ToString();
        }
    }

    private string ReducedPropertyTypeName => ReducedPropertyType(_property);

    public static implicit operator string(PropertyGenerator pg)
    {
        return pg != null ? pg.ToString() : string.Empty;
    }

    /// <summary>
    ///     Get all attributes based on PocoSetting initialization
    /// </summary>
    /// <returns></returns>
    public List<string> GetAllAttributes()
    {
        return _attributeManager.GetAllAttributes(_property);
    }

    /// <summary>
    ///     NullableModifier represented by "?" added to type , e.g. int?
    /// </summary>
    public string GetNullableModifier()
    {
        //support -b option
        var nullableModifier = _setting.AddNullableDataType && _property.IsNullable
            ? Helper.GetNullable(_property.PropType)
            : string.Empty;
        return _setting.EnableNullableReferenceTypes && _property is { IsNullable: true, IsKey: false }
            ? "?"
            : nullableModifier;
    }

    public override string ToString()
    {
        var atts = string.Join(_nl, GetAllAttributes());
        var text = $"{atts}{_nl}{Declaration}";
        return text;
    }

    internal string ReducedPropertyType(PropertyTemplate pt)
    {
        var reducedName = pt.PropType;
        //not prefixed with namespace
        if (!reducedName.Contains('.'))
        {
            return reducedName;
        }

        var ns = $"{pt.ClassNameSpace}.";
        if (pt.PropType.StartsWith(ns))
        {
            reducedName = pt.PropType.Replace(ns, string.Empty);
        }

        //collection
        var match = Regex.Match(pt.PropType.Trim(), "List<(.+?)>");
        if (!match.Success)
        {
            return reducedName;
        }

        var type = match.Groups[1].ToString();
        var typeReduced = type.Replace(ns, string.Empty);
        return $"List<{typeReduced}>";
    }

    private string Comment()
    {
        var comment = _property.PropComment + (_property.IsReadOnly ? " ReadOnly" : string.Empty);
        if (!string.IsNullOrEmpty(comment))
        {
            comment = $"//{comment}";
        }

        return comment;
    }

    private string? MappedPropertyName()
    {
        if (_setting.RenameMap is null)
        {
            return null;
        }

        List<PropertyNameMap>? allMap = null;
        var map = _setting.RenameMap.PropertyNameMap;
        foreach (var className in map.Keys)
        {
            if (className.Equals("ALL", StringComparison.OrdinalIgnoreCase))
            {
                // The ALL is a last resort.
                allMap = map[className];
            }
            else if (className.Equals(_property.ClassName, StringComparison.OrdinalIgnoreCase))
            {
                var n = map[className]
                    .Find(n =>
                        n.OldName.Equals(_property.PropName, StringComparison.OrdinalIgnoreCase));

                if (n is not null)
                {
                    return string.IsNullOrWhiteSpace(n.NewName) ? null : n.NewName;
                }
            }
        }

        if (allMap is not null)
        {
            var exactNameMap = allMap
                .Find(n => n.OldName.Equals(_property.PropName, StringComparison.OrdinalIgnoreCase));

            if (exactNameMap is not null)
            {
                return string.IsNullOrWhiteSpace(exactNameMap.NewName) ? null : exactNameMap.NewName;
            }

            foreach (var regexNameMap in allMap)
            {
                if (regexNameMap.OldName.StartsWith("^") &&
                    Regex.IsMatch(_property.PropName, regexNameMap.OldName, RegexOptions.IgnoreCase))
                {
                    //it's a regex. We really should compile it and cache it.
                    return string.IsNullOrWhiteSpace(regexNameMap.NewName) ? null : regexNameMap.NewName;
                }
            }
        }

        return null;
    }
}
