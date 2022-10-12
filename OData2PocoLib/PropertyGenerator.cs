// Copyright (c) Mohamed Hassan & Contributors. All rights reserved. See License.md in the project root for license information.

using System.Text;
using System.Text.RegularExpressions;
using OData2Poco.CustAttributes;
using OData2Poco.Extensions;

namespace OData2Poco;

/// <summary>
///     Generate All attributes of property :KeyAttribut,Required,JsonProperty
///     Convert name to Camel/Pascal Case
///     Generate the declaration of property e.g.   virtual public int? name  {get;set;} //comment
/// </summary>
public class PropertyGenerator
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
            if (mappedName is not null) return mappedName;

            switch (_setting.NameCase)
            {
                case CaseEnum.Pas:
                    return _property.PropName.ToPascalCase();

                case CaseEnum.Camel:
                    return _property.PropName.ToCamelCase();

                case CaseEnum.None:
                    return _property.PropName;
                default:
                    return _property.PropName;
            }
        }
    }

    /// <summary>
    ///     Virtual Modifier
    /// </summary>
    public string VirtualModifier
    {
        get
        {
            if (_setting.GeneratorType == GeneratorType.Record)
                return string.Empty;
            return _setting.AddNavigation && !_setting.AddEager ? " virtual" : string.Empty;
        }
    }

    public string Declaration
    {
        get
        {
            var setter = "{get;set;}";
            if (_setting.InitOnly)
                setter = "{get;init;}";
            return new StringBuilder()
                .Append($"public{VirtualModifier}")
                .Append(" ")
                .Append(ReducedPropertyTypeName)
                .Append(GetNullableModifier())
                .Append(" ")
                .Append(Name)
                .Append(" ")
                .Append(_property.IsReadOnly && !_setting.ReadWrite ? "{get;}" : setter)
                .Append(" ")
                .Append(Comment())
                .ToString();
        }
    }

    private string ReducedPropertyTypeName => ReducedPropertyType(_property);

    /// <summary>
    ///     Get all attributes based on PocoSetting initialization
    /// </summary>
    /// <returns></returns>
    public List<string> GetAllAttributes()
    {
        return _attributeManager.GetAllAttributes(_property);
    }

    /// <summary>
    ///     NullableModifier represented by "?" added to type , e.g int?
    /// </summary>
    public string GetNullableModifier()
    {
        //support -b option
        if (_setting.EnableNullableReferenceTypes && _property.IsNullable
                                                  && !_property.IsKey)
            return "?";

        if (_setting.AddNullableDataType && _property.IsNullable)
            return Helper.GetNullable(_property.PropType);

        return string.Empty;
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
        if (!reducedName.Contains(".")) return reducedName;

        var ns = $"{pt.ClassNameSpace}."; //

        if (pt.PropType.StartsWith(ns))
            reducedName = pt.PropType.Replace(ns, "");
        //collection
        var match = Regex.Match(pt.PropType.Trim(), "List<(.+?)>");
        if (!match.Success) return reducedName;
        var type = match.Groups[1].ToString();
        var typeReduced = type.Replace(ns, "");
        reducedName = $"List<{typeReduced}>";

        return reducedName;
    }

    public static implicit operator string(PropertyGenerator pg)
    {
        return pg.ToString();
    }

    private string Comment()
    {
        var comment = _property.PropComment + (_property.IsReadOnly ? " ReadOnly" : "");
        if (!string.IsNullOrEmpty(comment))
            comment = $"//{comment}";
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
            if (className.Equals("ALL", StringComparison.InvariantCultureIgnoreCase))
            {
                // The ALL is a last resort.
                allMap = map[className];
            }
            else if (className.Equals(_property.ClassName, StringComparison.InvariantCultureIgnoreCase))
            {
                var n = map[className]
                    .FirstOrDefault(n =>
                        n.OldName.Equals(_property.PropName, StringComparison.InvariantCultureIgnoreCase));

                if (n is not null) return string.IsNullOrWhiteSpace(n.NewName) ? null : n.NewName;
            }

        if (allMap is not null)
        {
            var exactNameMap = allMap
                .FirstOrDefault(n => n.OldName.Equals(_property.PropName, StringComparison.InvariantCultureIgnoreCase));

            if (exactNameMap is not null)
                return string.IsNullOrWhiteSpace(exactNameMap.NewName) ? null : exactNameMap.NewName;

            foreach (var regexNameMap in allMap)
                if (regexNameMap.OldName.IndexOf('^') == 0 &&
                    Regex.IsMatch(_property.PropName, regexNameMap.OldName, RegexOptions.IgnoreCase))
                    //it's a regex. We really should compile it and cache it.
                    return string.IsNullOrWhiteSpace(regexNameMap.NewName) ? null : regexNameMap.NewName;
        }

        return null;
    }
}