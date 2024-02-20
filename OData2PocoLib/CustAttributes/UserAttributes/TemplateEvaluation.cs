// Copyright (c) Mohamed Hassan & Contributors. All rights reserved. See License.md in the project root for license information.

namespace OData2Poco.CustAttributes.UserAttributes;

using System.Text.RegularExpressions;
using DynamicExpresso;
using Extensions;

internal static class TemplateEvaluation
{
    public static string EvaluateTemplate(
        this string template,
        object inputObject,
        out string[] errors)
    {
        errors = [];
        if (string.IsNullOrEmpty(template))
        {
            return string.Empty;
        }

        if (!template.Contains("{{"))
        {
            return template;
        }

        const string Pattern = @"{{([\s\S]+?)}}";
        List<string> allErrors = [];

        var outputString = Regex.Replace(template, Pattern, match =>
        {
            var expression = match.Groups[1].Value;
            string? result = null;
            try
            {
                result = expression.EvaluateExpression(inputObject, out _)?.ToString();
            }
            catch
            {
                var msg = $"Fail to evaluate {match.Value}";
                allErrors.Add(msg);
            }

            return result ?? match.Value;
        });
        errors = [.. allErrors];
        return outputString;
    }

    public static object? EvaluateExpression(
        this string expression,
        object? inputObject,
        out string error)
    {
        if (string.IsNullOrEmpty(expression))
        {
            error = "Empty Expression";
            return null;
        }

        if (inputObject == null)
        {
            error = "Null input object";
            return null;
        }

        error = string.Empty;
        Interpreter interpreter = new();

        interpreter
           .Reference(typeof(StringExtensions))
           .SetVariable("this", inputObject);
        try
        {
            return interpreter.Eval(expression);
        }
        catch (Exception ex)
        {
            throw new OData2PocoException($"Expression '{expression}' is not valid. {ex.Message}");
        }
    }

    public static bool EvaluateCondition(
        this string expression,
        object inputObject,
        out string error)
    {
        var result = expression.EvaluateExpression(inputObject, out error);
        return result is bool b ? b : throw new OData2PocoException($"Expression '{expression}' is not a valid condition");
    }
}
