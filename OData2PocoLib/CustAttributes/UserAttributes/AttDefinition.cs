// Copyright (c) Mohamed Hassan & Contributors. All rights reserved. See License.md in the project root for license information.

using System.Text;
using OData2Poco.Extensions;

namespace OData2Poco.CustAttributes.UserAttributes;

public class AttDefinition
{
    private string _scope = "property";

    public string Name { get; set; } = null!;

    public string Format { get; set; } = null!;
    public string Scope
    {
        get => _scope;
        set
        {
            if (value.IsValidValue(new[] { "property", "class" }))
            {
                _scope = value;
            }
            else
            {
                throw new ArgumentException("AttDefinition: Invalid scope value. Allowed values are: property, class");
            }
        }
    }

    public string Filter { get; set; } = string.Empty;

    public AttDefinition()
    {
        Scope = "property";
    }
    public AttDefinition(string name, string format, string scope = "property")
    {
        Name = name;
        Format = format;
        Scope = scope;
    }
    public INamedAttribute ToNamedAttribute() => new UserAttribute(this);

    public List<string> ToAttribute(object obj)
    {
        return obj switch
        {
            PropertyTemplate prop => new UserAttribute(this).GetAttributes(prop),
            ClassTemplate ct => new UserAttribute(this).GetAttributes(ct),
            _ => throw new OData2PocoException("AttDefinition: Invalid object type. Allowed types are: PropertyTemplate, ClassTemplate")
        };
    }

    public StringBuilder Export()
    {
        var sb = new StringBuilder();
        sb.AppendLine($"[{Name}]");
        sb.AppendLine($"Scope={Scope}");
        sb.AppendLine($"Format={Format}");
        if (!string.IsNullOrEmpty(Filter))
            sb.AppendLine($"Filter={Filter}");
        return sb;
    }
    public static AttDefinition Create(Dictionary<string, object> dict)
    {
        AttDefinition attDefinition = new()
        {
            Name = Get("name"),
            Format = Get("format"),
            Scope = Get("scope"),
            Filter = Get("filter"),
        };
        return attDefinition;

        string Get(string name)
        {
            if (dict.TryGetValue(name, out var value))
                return (string)value;
            return name.ToLower() == "scope" ? "property" : "";
        }
    }
    public static IEnumerable<AttDefinition> Import(string text)
    {
        var attDict = text.ParseIni();
        foreach (var (key, value) in attDict)
        {
            if (key == "__Global__") continue;
            value["Name"] = key;
            var ad = Create(value);
            yield return ad;
        }
    }

    public override string ToString()
    {
        return Export().ToString().Trim();
    }
}
