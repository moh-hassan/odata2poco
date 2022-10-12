// Copyright (c) Mohamed Hassan & Contributors. All rights reserved. See License.md in the project root for license information.

namespace OData2Poco.CustAttributes.NamedAtributes;

public class DbAttribute : INamedAttribute
{
    private readonly List<INamedAttribute> _sharedAttributes;

    public DbAttribute()
    {
        _sharedAttributes = new List<INamedAttribute>
        {
            new KeyAttribute(), new TableAttribute(), new RequiredAttribute()
        };
    }

    public string Name { get; } = "db";

    public List<string> GetAttributes(PropertyTemplate property)
    {
        var att = new List<string>();
        _sharedAttributes.ForEach(x =>
        {
            var a = x.GetAttributes(property);
            if (a.Any())
                att.AddRange(a);
        });
        return att;
    }

    public List<string> GetAttributes(ClassTemplate classTemplate)
    {
        var att = new List<string>();
        _sharedAttributes.ForEach(x =>
        {
            var a = x.GetAttributes(classTemplate);
            if (a.Any())
                att.AddRange(a);
        });
        return att;
    }
}