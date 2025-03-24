// Copyright (c) Mohamed Hassan & Contributors. All rights reserved. See License.md in the project root for license information.

namespace OData2Poco.Extensions;

using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using static System.Net.Mime.MediaTypeNames;

/// <summary>
///     Utility for CamelCase/PascalCase Conversion
/// </summary>
public static partial class StringExtensions
{
    /// <summary>
    ///     onvert the string to Pascal case.
    ///     see definition: https://en.wikipedia.org/wiki/PascalCase
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    public static string ToPascalCase(this string text)
    {
        if (string.IsNullOrEmpty(text)) return text;
        text = text.Trim();
        if (text.Length < 2) return text.ToUpper(); //one char

        // Split the string into words.
        char[] delimiterChars = [' ', '-', '_', '.'];
        var words = text.Split(delimiterChars, StringSplitOptions.RemoveEmptyEntries);
        //use stringBuilder for better performance
        var result = new StringBuilder();
        foreach (var word in words)
        {
            result.Append(char.ToUpper(word[0])); //Convert First Char in word to Capital letter
            result.Append(word[1..]); // Combine with the rest of word
        }

        return result.ToString();
    }

    /// <summary>
    ///     Convert the string to camel case.
    ///     see Definition  https://en.wikipedia.org/wiki/Camel_case
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    public static string ToCamelCase(this string text)
    {
        text = ToPascalCase(text);
        return text[..1].ToLower() + text[1..];
    }

    public static string ToKebabCase(this string text)
    {
        return Regex.Replace(text, "([a-z](?=[A-Z])|[A-Z](?=[A-Z][a-z]))", "$1-").ToLower();
    }

    public static string ChangeCase(this string text, CaseEnum caseEnum)
    {
        return caseEnum switch
        {
            CaseEnum.Pas => text.ToPascalCase(),
            CaseEnum.Camel => text.ToCamelCase(),
            CaseEnum.Kebab => text.ToKebabCase(),
            CaseEnum.Snake => text.ToSnakeCase(),
            _ => text
        };
    }

    /// <summary>
    ///     remove extra white spaces and keeping CRLF if needed
    /// </summary>
    /// <param name="text"></param>
    /// <param name="keepCrLf"></param>
    /// <returns></returns>
    public static string TrimAllSpace(this string text, bool keepCrLf = false)
    {
        if (string.IsNullOrEmpty(text)) return text;
        var result = string.Empty;
        Regex trimmer = new(@"\s+");
        if (!keepCrLf)
        {
            return trimmer.Replace(text.Trim(), " ");
        }

        char[] separator = ['\n', '\r'];
        var lines = text.Split(separator, StringSplitOptions.RemoveEmptyEntries);

        return lines.Aggregate(result, (current, line)
            => current + $"{trimmer.Replace(line.Trim(), " ")}\n");
    }

    /// <summary>
    ///     Convert name string to C# Attribute
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    public static string ToCsAttribute(this string text)
    {
        return "[" + text + "]";
    }

    public static string NewLine(this string text)
    {
        return text + Environment.NewLine;
    }

    /// <summary>
    ///     Format xml
    /// </summary>
    /// <param name="xml"></param>
    /// <returns></returns>
    public static string FormatXml(this string xml)
    {
        var doc = XDocument.Parse(xml);
        return doc.ToString();
    }

    /// <summary>
    ///     Validate json string
    /// </summary>
    /// <param name="json"></param>
    /// <returns></returns>
    public static bool ValidateJson(this string json)
    {
        try
        {
            JToken.Parse(json);
            return true;
        }
        catch (JsonReaderException)
        {
            return false;
        }
    }

    public static string Quote(this string text, char c = '"')
    {
        return $"{c}{text}{c}";
    }

    public static string UnQuote(this string text)
    {
        if (string.IsNullOrEmpty(text)) return text;
        return text.Trim('\'').Trim('"');
    }

    public static string ToTitle(this string text)
    {
        if (string.IsNullOrEmpty(text)) return text;
        if (text.Length <= 2) return text;
        text = text.ToPascalCase();
        StringBuilder builder = new();
        foreach (var c in text)
        {
            if (char.IsUpper(c) && builder.Length > 0) builder.Append(' ');
            builder.Append(c);
        }

        return builder.ToString();
    }

    public static string ToSnakeCase(this string str)
    {
        var text = string.Concat(str.Select((x, i) => i > 0 && char.IsUpper(x) ? "_" + x : x.ToString())).ToLower();
        return text.Replace("__", "_");
    }

    public static string RemoveEmptyLines(this string input)
    {
        return Regex.Replace(input, @"^\s+$[\r\n]*", string.Empty, RegexOptions.Multiline);
    }

    //---------- moved from Extension classs--------
    /// <summary>
    ///     Merge list to one string
    /// </summary>
    /// <param name="list"></param>
    /// <param name="separator"></param>
    /// <returns></returns>
    public static string List2String(this List<string> list, string separator = ",")
    {
        var text = string.Join(separator, list);
        return text;
    }

    public static string GetInnerException(this Exception ex)
    {
        if (ex == null) return string.Empty;
        return ex.InnerException != null
            ? $"{ex.InnerException.Message} ---> {GetInnerException(ex.InnerException)} "
            : string.Empty;
    }

    public static string FullExceptionMessage(this Exception ex, bool showTrace = false)
    {
        if (ex == null) return string.Empty;
        var s = ex.GetInnerException();
        var msg = string.IsNullOrEmpty(s) ? ex.Message : ex.Message + "-->\n" + s;

        if (showTrace)
            return $"{msg}\nException Details:\n {ex}";
        return msg;
    }

    /// <summary>
    ///     Generte C# code for a given  Entity using FluentCsTextTemplate
    /// </summary>
    /// <returns></returns>
    public static string DicToString(this Dictionary<string, string> header)
    {
        if (header == null) return string.Empty;
        StringBuilder builder = new();
        foreach (var item in header) builder.Append(item.Key).Append(": ").Append(item.Value).AppendLine();
        var result = builder.ToString();
        return result;
    }

    public static string ToggleFirstLetter(this string name)
    {
        if (string.IsNullOrEmpty(name))
            return name;
        var letters = name.ToCharArray();
        if (letters[0] >= 'a' && letters[0] <= 'z')
        {
            //Convert lowercase to uppercase
            letters[0] = (char)(letters[0] - 32);
        }
        else if (letters[0] >= 'A' && letters[0] <= 'Z')
        {
            //Convert uppercase to lowercase
            letters[0] = (char)(letters[0] + 32);
        }

        return new string(letters);
    }

    public static string ToNullable(this string name, bool isNullable)
    {
        return isNullable ? $"{name}?" : name;
    }

    public static T ToEnum<T>(this string value)
    {
        try
        {
            return (T)Enum.Parse(typeof(T), value, true);
        }
        catch (Exception)
        {
            // ignored
        }

        return default!;
    }

    public static JObject ToJObject(this string json)
    {
        var j = JObject.Parse(json);
        return j;
    }

    public static List<string> ToLines(this string text)
    {
        if (string.IsNullOrEmpty(text)) return [];
        return text.Trim().Split('\n')
            .Select(a => a.Trim(' ', '\t', '{', '}', '\r'))
            .Where(a => a.Length > 0)
            .ToList();
    }

    public static string EnumToString(this Type type)
    {
        if (type == null) return string.Empty;
        if (!type.IsEnum) return string.Empty;
        var values = Enum.GetNames(type);
        return string.Join(", ", values);
    }

    public static string Prefix(this string name, string? prefix)
    {
        if (string.IsNullOrEmpty(prefix))
            return name;
        return $"{prefix}{name}";
    }

    public static string Suffix(this string name, string? suffix)
    {
        return string.IsNullOrEmpty(suffix) ? name : $"{name}{suffix}";
    }

    public static Match MatchPattern(this string inputText, string pattern)
    {
        const RegexOptions Option = RegexOptions.IgnoreCase
                                    | RegexOptions.Compiled
                                    | RegexOptions.CultureInvariant
                                    | RegexOptions.IgnorePatternWhitespace;

        Regex myRegex = new(pattern, Option);
        var m = myRegex.Match(inputText);

        return m;
    }

    public static string Reduce(this string text, char separator = '.')
    {
        return string.IsNullOrEmpty(text) ? text : text.Split(separator)[^1];
    }

    public static string RemoveNamespace(this string text, string ns)
    {
        if (string.IsNullOrEmpty(ns) || string.IsNullOrEmpty(text))
            return text;
        return text.StartsWith(ns, StringComparison.Ordinal)
            ? text.Replace(ns, string.Empty).TrimStart('.')
            : text;
    }

    public static string RemoveDot(this string name)
    {
        return string.IsNullOrEmpty(name) ? name : name.Replace(".", string.Empty);
    }

    public static string NormalizeName(this string name, string ns, bool useFullName)
    {
        if (useFullName)
            return name.RemoveDot();
        return name.RemoveNamespace(ns);
    }

    public static string[] SplitArgs(this string? command, bool keepQuote = false)
    {
        if (string.IsNullOrEmpty(command))
            return [];

        var inQuote = false;
        var chars = command.ToCharArray().Select(v =>
        {
            if (v == '"')
                inQuote = !inQuote;
            return !inQuote && v == ' ' ? '\n' : v;
        }).ToArray();

        return new string(chars).Split('\n')
            .Select(x => keepQuote ? x : x.Trim('"'))
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .ToArray();
    }

    /// <summary>
    /// Convert string to base64
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public static string ToBase64(this string input)
    {
        var bytes = Encoding.UTF8.GetBytes(input);
        var base64 = Convert.ToBase64String(bytes);
        return base64;
    }

    /// <summary>
    /// convert base64 to string
    /// </summary>
    /// <param name="base64"></param>
    /// <returns></returns>
    public static string FromBase64(this string base64)
    {
        var bytes = Convert.FromBase64String(base64);
        var input = Encoding.UTF8.GetString(bytes);
        return input;
    }

    /// <summary>
    /// Any string that ends with {xxx} will be replaced with base64 of xxx
    /// </summary>
    /// <param name="header"></param>
    /// <param name="header2">header in base64 if contained in { }</param>
    /// <returns></returns>
    public static string ReplaceToBase64(this string text)
    {
        _ = text ?? throw new ArgumentNullException(nameof(text));
        // Check if the text contains '{' and '}'
        var startIndex = text.IndexOf('{');
        var endIndex = text.IndexOf('}');

        if (startIndex >= 0 && endIndex > startIndex)
        {
            // Extract the content inside '{' and '}'
            var placeholder = text.Substring(startIndex + 1, endIndex - startIndex - 1);

            // Base64 encode the placeholder
            var base64Encoded = Convert.ToBase64String(Encoding.UTF8.GetBytes(placeholder));

            // Replace the placeholder with the Base64-encoded value
            var result = text.Remove(startIndex, endIndex - startIndex + 1).Insert(startIndex, base64Encoded);

            return result;
        }

        // If no '{' and '}' are found, return the original string
        return text;
    }

    public static (string, string) SplitKeyValue(this string input)
    {
        if (input == null)
        {
            return (string.Empty, string.Empty);
        }

        char[] separator = ['='];
        var parts = input.Split(separator, 2, StringSplitOptions.RemoveEmptyEntries);
        return parts.Length switch
        {
            1 => (parts[0], string.Empty),
            2 => (parts[0], parts[1]),
            _ => (string.Empty, string.Empty)
        };
    }

    public static string AsString(this IEnumerable<string> list, string separator = ", ")
        => string.Join(separator, list);
}
