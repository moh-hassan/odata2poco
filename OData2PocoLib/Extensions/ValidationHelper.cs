// Copyright (c) Mohamed Hassan & Contributors. All rights reserved. See License.md in the project root for license information.

using System.Text.RegularExpressions;

namespace OData2Poco.Extensions;
public static partial class StringExtensions
{
    public static bool IsValidValue(this string value, string[] validValues, bool isCaseSensitive = false)
    {
        return isCaseSensitive
            ? validValues.Contains(value)
            : validValues.Contains(value, StringComparer.OrdinalIgnoreCase);
    }
    /// <summary>
    /// check if the name is in the validValues
    /// </summary>
    /// <param name="name"></param>
    /// <param name="validValues"></param>
    /// <returns></returns>
    public static bool In(this string name, params string[] validValues)
    {
        return IsValidValue(name, validValues);
    }

    /// <summary>
    /// check if the name is not in the validValues
    /// </summary>
    /// <param name="name"></param>
    /// <param name="validValues"></param>
    /// <returns></returns>
    public static bool NotIn(this string name, params string[] validValues)
    {
        return !In(name, validValues);
    }

    /// <summary>
    /// check if the name is like any of the patterns
    /// </summary>
    /// <param name="name"></param>
    /// <param name="patterns">is * ? </param>
    /// <returns></returns>
    public static bool Like(this string name, params string[] patterns)
    {
        return patterns
            .Select(p => p.Trim().Replace("*", ".*").Replace("?", "."))
            .Select(p2 => new Regex($"^{p2}$", RegexOptions.IgnoreCase))
            .Any(r => r.IsMatch(name));
    }

    /// <summary>
    /// check if the name is not like any of the patterns
    /// </summary>
    /// <param name="name"></param>
    /// <param name="patterns"></param>
    /// <returns></returns>
    public static bool NotLike(this string name, params string[] patterns)
    {
        return !Like(name, patterns);
    }
}