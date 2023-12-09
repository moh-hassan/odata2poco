// Copyright (c) Mohamed Hassan & Contributors. All rights reserved. See License.md in the project root for license information.

namespace OData2Poco.CustAttributes.NamedAtributes;

public class DbAttribute : INamedAttribute
{
    public string Name { get; set; } = "db";
    public string Scope { get; set; } = "dual";
    public bool IsUserDefined { get; set; } = false;
    public bool IsValid { get; set; } = true;
    private readonly List<INamedAttribute> _sharedAttributes;

    public DbAttribute()
    {
        _sharedAttributes =
        [
            new KeyAttribute(),
            new TableAttribute(),
            new RequiredAttribute()
        ];
    }
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