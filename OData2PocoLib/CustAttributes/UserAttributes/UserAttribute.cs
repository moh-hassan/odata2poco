// Copyright (c) Mohamed Hassan & Contributors. All rights reserved. See License.md in the project root for license information.

namespace OData2Poco.CustAttributes.UserAttributes;
#pragma warning disable S927

/// <summary>
/// Generate a custom attribute from a template
/// </summary>
public class UserAttribute : INamedAttribute
{
    public string Name { get; }
    public string Scope { get; }
    public bool IsUserDefined { get; } = true;
    private readonly AttDefinition attDefinition;

    public UserAttribute(AttDefinition ad)
    {
        Name = ad.Name;
        Scope = ad.Scope;
        attDefinition = ad;
    }
    public UserAttribute(string name, string format) : this(new AttDefinition(name, format))
    {
    }

    public List<string> GetAttributes(PropertyTemplate property) =>
        EvaluateTemplate(property, "property");
    public List<string> GetAttributes(ClassTemplate ct) =>
        EvaluateTemplate(ct, "class");
    private List<string> EvaluateTemplate(object obj, string scope)
    {
        if (attDefinition.Scope != scope)
            return new List<string>();

        var filter = attDefinition.Filter.Length == 0
                     || attDefinition.Filter.EvaluateConditionExpression(obj, out _);
        if (!filter)
            return new List<string>();

        var format = attDefinition.Format.EvaluateTemplate(obj, out _);
        return new List<string> { format };
    }

    public static implicit operator AttDefinition(UserAttribute ua)
    {
        return ua.attDefinition;
    }
    public override string ToString()
    {
        return attDefinition.ToString();
    }
}