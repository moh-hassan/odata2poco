// Copyright (c) Mohamed Hassan & Contributors. All rights reserved. See License.md in the project root for license information.

using System.Text.RegularExpressions;

namespace OData2Poco.TestUtility;

public static class HelpTestExtension
{
    public static string[] HelpToLines(this string help)
    {
        return Regex.Split(help, "\r\n|\r|\n");
    }
    public static string GetRegexPattern(this string text, string escapeChar = "()[]?")
    {
        var pattern = text;
        if (!string.IsNullOrEmpty(escapeChar))
        {
            char[] chars = escapeChar.ToCharArray();
            pattern = chars.Aggregate(pattern, (current, c) => current.Replace(c.ToString(), $"\\{c}"));
        }
        pattern = Regex.Replace(pattern, @"\s+", @"\s*");
        return pattern;
    }

}