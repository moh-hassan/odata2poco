// Copyright (c) Mohamed Hassan & Contributors. All rights reserved. See License.md in the project root for license information.

#pragma warning disable CA1062
namespace OData2Poco.CustAttributes.UserAttributes;

using InfraStructure.Logging;

/// <summary>
/// Generate a custom attribute from a template
/// </summary>
public class UserAttribute : INamedAttribute
{
    private readonly AttDefinition _attDefinition;
    private readonly ILog _logger = PocoLogger.Default;

    public UserAttribute(AttDefinition ad)
    {
        _ = ad ?? throw new ArgumentNullException(nameof(ad));
        if (string.IsNullOrEmpty(ad.Name))
        {
            throw new ArgumentException("Attribute Name cannot be null or empty");
        }

        if (string.IsNullOrEmpty(ad.Format))
        {
            throw new ArgumentException(
                $"Invalid Attribute: '{ad.Name}' and will be ignored. 'Format' cannot be null or empty");
        }

        (Name, Scope) = (ad.Name, ad.Scope);
        _attDefinition = ad;
    }

    public UserAttribute(string name, string format) : this(new AttDefinition(name, format))
    {
    }

    public string Name { get; set; }
    public string Scope { get; set; }
    public bool IsUserDefined { get; set; } = true;
    public bool IsValid { get; set; } = true;

    public static implicit operator AttDefinition(UserAttribute ua)
    {
        return ua._attDefinition;
    }

    public List<string> GetAttributes(PropertyTemplate propertyTemplate)
    {
        return EvaluateTemplate(propertyTemplate, "property");
    }

    public List<string> GetAttributes(ClassTemplate classTemplate)
    {
        return EvaluateTemplate(classTemplate, "class");
    }

    public override string ToString()
    {
        return _attDefinition.ToString();
    }

    private List<string> EvaluateTemplate(object obj, string scope)
    {
        if (!IsValid || _attDefinition.Scope != scope)
        {
            return [];
        }

        try
        {
            var filter = _attDefinition.Filter.Length == 0
                         || _attDefinition.Filter.EvaluateCondition(obj, out _);

            if (!filter)
            {
                return [];
            }

            var format = _attDefinition.Format.EvaluateTemplate(obj, out _);
            return [format];
        }
        catch (Exception e)
        {
            IsValid = false;
            _logger.Warn($"Invalid Attribute {Name}, {e.Message}:\n{_attDefinition.Export()}");
        }

        return [];
    }
}
