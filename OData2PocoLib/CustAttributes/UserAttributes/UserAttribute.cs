// Copyright (c) Mohamed Hassan & Contributors. All rights reserved. See License.md in the project root for license information.

using OData2Poco.InfraStructure.Logging;


namespace OData2Poco.CustAttributes.UserAttributes;
#pragma warning disable S927

/// <summary>
/// Generate a custom attribute from a template
/// </summary>
public class UserAttribute : INamedAttribute
{
    private readonly ILog _logger = PocoLogger.Default;
    public string Name { get; set; }
    public string Scope { get; set; }
    public bool IsUserDefined { get; set; } = true;
    public bool IsValid { get; set; } = true;
    private readonly AttDefinition _attDefinition;

    public UserAttribute(AttDefinition ad)
    {
        if (string.IsNullOrEmpty(ad.Name))
            throw new ArgumentException("Attribute Name cannot be null or empty");
        if (string.IsNullOrEmpty(ad.Format))
            throw new ArgumentException($"Invalid Attribute: '{ad.Name}' and will be ignored. 'Format' cannot be null or empty");
        (Name, Scope) = (ad.Name, ad.Scope);
        _attDefinition = ad;
    }
    public UserAttribute(string name, string format) : this(new AttDefinition(name, format))
    {
    }

    public List<string> GetAttributes(PropertyTemplate property)
    {
        return EvaluateTemplate(property, "property");
    }

    public List<string> GetAttributes(ClassTemplate ct) =>
        EvaluateTemplate(ct, "class");

    private List<string> EvaluateTemplate(object obj, string scope)
    {
        if (!IsValid || _attDefinition.Scope != scope)
            return new List<string>();

        try
        {
            var filter = _attDefinition.Filter.Length == 0
                         || _attDefinition.Filter.EvaluateCondition(obj, out _);

            if (!filter)
                return new List<string>();

            var format = _attDefinition.Format.EvaluateTemplate(obj, out _);
            return new List<string> { format };
        }
        catch (Exception e)
        {
            IsValid = false;
            _logger.Warn($"Invalid Attribute {Name}, {e.Message}:\n{_attDefinition.Export()}");
        }
        return new List<string>();
    }

    public static implicit operator AttDefinition(UserAttribute ua)
    {
        return ua._attDefinition;
    }
    public override string ToString()
    {
        return _attDefinition.ToString();
    }
}