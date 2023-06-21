using DynamicExpresso;
using System.Text.RegularExpressions;
using OData2Poco.Extensions;

namespace OData2Poco;

internal static class TemplateEvaluation
{
    public static string EvaluateTemplate(this string template, object inputObject, out string[] errors)
    {
        errors = Array.Empty<string>();
        if (string.IsNullOrEmpty(template))
            return string.Empty;
        if (!template.Contains("{{"))
            return template;
        string pattern = @"{{([\s\S]+?)}}";
        var allErrors = new List<string>();

        string outputString = Regex.Replace(template, pattern, match =>
        {
            string expression = match.Groups[1].Value;
            string? result = null;
            try
            {
                result = EvaluateExpression(expression, inputObject, out _).ToString();
            }
            catch
            {
                var msg = $"Fail to evaluate {match.Value}";
                allErrors.Add(msg);
            }
            return result ?? match.Value;
        });
        errors = allErrors.ToArray();
        return outputString;
    }

    public static object EvaluateExpression(this string expression, object inputObject,
        out string error)
    {
        error = string.Empty;
        Interpreter interpreter = new Interpreter();

        interpreter
            .Reference(typeof(StringExtensions))
            .SetVariable("this", inputObject);
        object result;
        try
        {
            return interpreter.Eval(expression);
        }
        catch (Exception ex)
        {
            error = ex.Message;
            result = expression;
            return result;
        }
    }

    public static bool EvaluateConditionExpression(this string expression, object inputObject,
        out string error)
    {
        var result = EvaluateExpression(expression, inputObject, out error);
        if (result is bool b)
            return b;
        return false;
    }
}
