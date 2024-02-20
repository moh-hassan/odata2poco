// Copyright (c) Mohamed Hassan & Contributors. All rights reserved. See License.md in the project root for license information.

namespace OData2Poco.CustAttributes;

using System.Reflection;
using UserAttributes;

public class PocoAttributesList : IEnumerable<INamedAttribute>
{
    private readonly List<INamedAttribute> _namedAttributes;

    public PocoAttributesList()
    {
        _namedAttributes = [];
        FillNamedAttributes();
    }

    public INamedAttribute? this[string index] => GetAttributeObject(index);

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public IEnumerator<INamedAttribute> GetEnumerator()
    {
        return _namedAttributes.GetEnumerator();
    }

    public List<string> SupportedAttributes()
    {
        return _namedAttributes.Select(x => x.Name).ToList();
    }

    public INamedAttribute? GetAttributeObject(string attName)
    {
        return _namedAttributes.Find(x => x.Name == attName);
    }

    public void LoadPluginAttributes()
    {
        var foldr = "plugin";
        foldr = Path.GetFullPath(foldr);
        if (!Directory.Exists(foldr))
        {
            return;
        }

        var pluginList = Helper.LoadPlugin<INamedAttribute>(foldr)
            .Cast<INamedAttribute>().ToList();

        if (pluginList.Count > 0)
        {
            _namedAttributes.AddRange(pluginList);
        }
    }

    public void Add(AttDefinition ad)
    {
        if (ad != null)
        {
            _namedAttributes.Add(ad.ToNamedAttribute());
        }
    }

    public void Add(INamedAttribute att)
    {
        _namedAttributes.Add(att);
    }

    public void AddRange(IEnumerable<INamedAttribute> atts)
    {
        _namedAttributes.AddRange(atts);
    }

    private void FillNamedAttributes()
    {
        var asm = typeof(INamedAttribute).GetTypeInfo().Assembly;
        var types = asm.DefinedTypes
            .Where(x => x.ImplementedInterfaces.Contains(typeof(INamedAttribute)) &&
             !x.Name.Contains("UserAttribute"));

        foreach (var type in types)
        {
            if (Activator.CreateInstance(type) is INamedAttribute item)
            {
                _namedAttributes.Add(item);
            }
        }
    }
}
