// Copyright (c) Mohamed Hassan & Contributors. All rights reserved. See License.md in the project root for license information.

namespace OData2Poco.CustAttributes.UserAttributes;

using System.Text;
using Extensions;

public class AttDefinition
{
    private string _scope = "property";

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

    public string Name { get; set; } = null!;

    public string Format { get; set; } = null!;

    public string Scope
    {
        get => _scope;
        set => _scope = value.IsValidValue(["property", "class"])
                ? value
                : throw new ArgumentException("AttDefinition: Invalid scope value. Allowed values are: property, class");
    }

    public string Filter { get; set; } = string.Empty;

    public static AttDefinition Create(Dictionary<string, object> dict)
    {
        _ = dict ?? throw new ArgumentNullException(nameof(dict));
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
            var property = name.ToLower().Equals("scope", StringComparison.OrdinalIgnoreCase)
                ? "property" : string.Empty;
            return dict.TryGetValue(name, out var value)
                ? (string)value
                : property;
        }
    }

    public static IEnumerable<AttDefinition> Import(string text)
    {
        var attDict = text.ParseIni();
        foreach (var (key, value) in attDict)
        {
            if (key == "__Global__")
            {
                continue;
            }

            value["Name"] = key;
            var ad = Create(value);
            yield return ad;
        }
    }

    public INamedAttribute ToNamedAttribute()
    {
        return new UserAttribute(this);
    }

    public List<string> ToAttribute(object obj)
    {
        return obj switch
        {
            PropertyTemplate prop => new UserAttribute(this).GetAttributes(prop),
            ClassTemplate ct => new UserAttribute(this).GetAttributes(ct),
            _ => throw new OData2PocoException(
                "AttDefinition: Invalid object type. Allowed types are: PropertyTemplate, ClassTemplate")
        };
    }

    public StringBuilder Export()
    {
        StringBuilder sb = new();
        sb.AppendLine($"[{Name}]")
          .AppendLine($"Scope={Scope}")
          .AppendLine($"Format={Format}");
        if (!string.IsNullOrEmpty(Filter))
        {
            sb.AppendLine($"Filter={Filter}");
        }

        return sb;
    }

    public override string ToString()
    {
        return Export().ToString().Trim();
    }
}
