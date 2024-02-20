// Copyright (c) Mohamed Hassan & Contributors. All rights reserved. See License.md in the project root for license information.

namespace OData2Poco.CustAttributes;

using InfraStructure.Logging;
using UserAttributes;

public class AttributeFactory
{
    private static readonly Lazy<AttributeFactory> s_lazy = new(() => new AttributeFactory());
    private readonly ILog _logger = PocoLogger.Default;
    private readonly PocoAttributesList _pocoAttributesList;
    private List<string> _attributes; //attributes from commandline options

    private AttributeFactory()
    {
        _pocoAttributesList = [];
        _attributes = [];
    }

    public static AttributeFactory Default => s_lazy.Value;

    /// <summary>
    ///     Initialize factory with setting.Attributes
    /// </summary>
    /// <param name="setting"></param>
    /// <returns></returns>
    public AttributeFactory Init(PocoSetting? setting = null)
    {
        setting ??= new PocoSetting();
        _attributes = [.. setting.Attributes]; //add attributes of commandline options
        //load user attributes
        if (!string.IsNullOrEmpty(setting.AtributeDefs))
        {
            if (File.Exists(setting.AtributeDefs))
            {
                var text = File.ReadAllText(setting.AtributeDefs);
                AddUserAttributes(text);
            }
            else
            {
                _logger.Warn($"User attributes file {setting.AtributeDefs} doesn't exist");
            }
        }

        return this;
    }

    public List<string> GetAttributes(object property, string attName)
    {
        _ = property ?? throw new ArgumentNullException(nameof(property));
        _ = attName ?? throw new ArgumentNullException(nameof(attName));
        if (attName.StartsWith("[") && property is PropertyTemplate)
        {
            return [attName];
        }

        var attributeObject = GetAttribute(attName);
        return property switch
        {
            PropertyTemplate p => attributeObject != null ? attributeObject.GetAttributes(p) : [],
            ClassTemplate c => attributeObject != null ? attributeObject.GetAttributes(c) : [],
            _ => throw new OData2PocoException(
                $"{property.GetType()} isn't supported for named attributes")
        };
    }

    public List<string> GetAttributes(object property, List<string> attNames)
    {
        _ = property ?? throw new ArgumentNullException(nameof(property));
        _ = attNames ?? throw new ArgumentNullException(nameof(attNames));

        List<string> list = [];
        foreach (var s in attNames)
        {
            var att = GetAttributes(property, s);
            att.ForEach(x =>
            {
                if (!list.Contains(x))
                {
                    list.Add(x);
                }
            });
        }

        return list;
    }

    public List<string> GetAllAttributes(object property)
    {
        return GetAttributes(property, _attributes);
    }

    public PocoAttributesList GetAllAttributes()
    {
        return _pocoAttributesList;
    }

    public void AddUserAttribute(AttDefinition ad)
    {
        try
        {
            _pocoAttributesList.Add(ad);
        }
        catch (ArgumentNullException e)
        {
            _logger.Warn($"{e.Message}");
        }
    }

    public PocoAttributesList GetAttributeList()
    {
        return _pocoAttributesList;
    }

    public bool IsExists(string attName)
    {
        return _pocoAttributesList[attName] is not null;
    }

    private INamedAttribute? GetAttribute(string attName)
    {
        return _pocoAttributesList[attName];
    }

    private void AddUserAttributes(string text)
    {
        var attDefs = AttDefinition.Import(text);
        foreach (var ad in attDefs)
        {
            AddUserAttribute(ad);
        }
    }
}
