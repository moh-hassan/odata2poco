// Copyright (c) Mohamed Hassan & Contributors. All rights reserved. See License.md in the project root for license information.

namespace OData2Poco.CustAttributes;

public class AttributeFactory
{
    private static readonly Lazy<AttributeFactory> Lazy = new(() => new AttributeFactory());
    private readonly PocoAttributesList _pocoAttributesList;
    private List<string> _attributes;

    private AttributeFactory()
    {
        _pocoAttributesList = new PocoAttributesList();
        _attributes = new List<string>();
    }

    public static AttributeFactory Default => Lazy.Value;

    /// <summary>
    ///     Initialize factory with setting.Attributes
    /// </summary>
    /// <param name="setting"></param>
    /// <returns></returns>
    public AttributeFactory Init(PocoSetting? setting = null)
    {
        setting ??= new PocoSetting();
        _attributes = new List<string>(setting.Attributes); //add attributes of commandline options
        return this;
    }

    private INamedAttribute? GetAttributeObject(string attName)
    {
        return _pocoAttributesList[attName];
    }


    public List<string> GetAttributes(object property, string attName)
    {
        if (attName.StartsWith("[") && property is PropertyTemplate)
            return new List<string> { attName };

        var attributeObject = GetAttributeObject(attName);
        return property switch
        {
            PropertyTemplate p => attributeObject != null ? attributeObject.GetAttributes(p) : new List<string>(),
            ClassTemplate c => attributeObject != null ? attributeObject.GetAttributes(c) : new List<string>(),
            _ => throw new Exception($"{property.GetType()} isn't supported for named attributes")
        };
    }

    public List<string> GetAttributes(object property, List<string> attNames)
    {
        var list = new List<string>();
        foreach (var s in attNames)
        {
            var att = GetAttributes(property, s);
            att.ForEach(x =>
            {
                if (!list.Contains(x))
                    list.Add(x);
            });
        }

        return list;
    }

    public List<string> GetAllAttributes(object property)
    {
        return GetAttributes(property, _attributes);
    }
}