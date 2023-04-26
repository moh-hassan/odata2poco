// Copyright (c) Mohamed Hassan & Contributors. All rights reserved. See License.md in the project root for license information.

#pragma warning disable S1643

using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OData2Poco.InfraStructure.FileSystem;

namespace OData2Poco.Extensions;

/// <summary>
///     Utility for CamelCase/PascalCase Conversion
/// </summary>
public static class StringExtensions
{
    /// <summary>
    ///     onvert the string to Pascal case.
    ///     see definition: https://en.wikipedia.org/wiki/PascalCase
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    public static string ToPascalCase(this string text)
    {
        text = text.Trim();
        if (string.IsNullOrEmpty(text)) return text;
        if (text.Length < 2) return text.ToUpper(); //one char

        // Split the string into words.
        char[] delimiterChars = { ' ', '-', '_', '.' };
        var words = text.Split(delimiterChars, StringSplitOptions.RemoveEmptyEntries);


        var result = "";
        foreach (var word in words)
        {
            result += char.ToUpper(word[0]); //Convert First Char in word to Capita letter
            result += word[1..]; // Combine with the rest of word
        }

        return result;
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
        var result = "";
        Regex trimmer = new(@"\s+");
        if (!keepCrLf)
        {
            result = trimmer.Replace(text.Trim(), " ");
            return result;
        }

        var lines = text.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
        foreach (var line in lines) result += $"{trimmer.Replace(line.Trim(), " ")}\n";

        return result;
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
        return text.Trim('\'').Trim('"');
    }

    public static string ToTitle(this string text)
    {
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
        text = text.Replace("__", "_");
        return text;
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
        if (ex.InnerException != null)
            return $"{ex.InnerException.Message} ---> {GetInnerException(ex.InnerException)} "; //recursive
        return "";
    }

    public static string FullExceptionMessage(this Exception ex, bool showTrace = false)
    {
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
            //Convert lowercase to uppercase 
            letters[0] = (char)(letters[0] - 32);
        else if (letters[0] >= 'A' && letters[0] <= 'Z')
            //Convert uppercase to lowercase 
            letters[0] = (char)(letters[0] + 32);
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
        return text.Trim().Split('\n')
            .Select(a => a.Trim(' ', '\t', '{', '}', '\r'))
            .Where(a => a.Length > 0)
            .ToList();
    }

    public static string EnumToString(this Type t)
    {
        if (!t.IsEnum) return "";
        var values = Enum.GetNames(t);
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
        if (string.IsNullOrEmpty(suffix))
            return name;
        return $"{name}{suffix}";
    }

    public static Match MatchPattern(this string inputText, string pattern)
    {
        var option = RegexOptions.IgnoreCase
                     | RegexOptions.Compiled
                     | RegexOptions.CultureInvariant
                     | RegexOptions.IgnorePatternWhitespace;

        Regex myRegex = new(pattern, option);
        var m = myRegex.Match(inputText);

        return m;
    }

    public static string Reduce(this string text, char separator = '.')
    {
        return text.Split(separator).Last();
    }

    public static string RemoveNamespace(this string text, string ns)
    {
        if (string.IsNullOrEmpty(ns))
            return text;
        if (text.StartsWith(ns, StringComparison.Ordinal))
            return text.Replace(ns, "").TrimStart('.');
        return text;
    }

    public static string RemoveDot(this string name)
    {
        return name.Replace(".", "");
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
            return new string[0];

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
}