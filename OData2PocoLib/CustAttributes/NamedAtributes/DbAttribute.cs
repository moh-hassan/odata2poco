// Copyright (c) Mohamed Hassan & Contributors. All rights reserved. See License.md in the project root for license information.

namespace OData2Poco.CustAttributes.NamedAtributes;

public class DbAttribute : INamedAttribute
{
    private readonly List<INamedAttribute> _sharedAttributes =
    [
        new KeyAttribute(),
        new TableAttribute(),
        new RequiredAttribute()
    ];

    public string Name { get; set; } = "db";
    public string Scope { get; set; } = "dual";
    public bool IsUserDefined { get; set; }
    public bool IsValid { get; set; } = true;

    public List<string> GetAttributes(PropertyTemplate propertyTemplate)
    {
        List<string> att = [];
        _sharedAttributes.ForEach(x =>
        {
            var a = x.GetAttributes(propertyTemplate);
            if (a.Count > 0)
            {
                att.AddRange(a);
            }
        });
        return att;
    }

    public List<string> GetAttributes(ClassTemplate classTemplate)
    {
        List<string> att = [];
        _sharedAttributes.ForEach(x =>
        {
            var a = x.GetAttributes(classTemplate);
            if (a.Count > 0)
            {
                att.AddRange(a);
            }
        });
        return att;
    }
}
