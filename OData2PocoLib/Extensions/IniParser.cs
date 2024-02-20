// Copyright (c) Mohamed Hassan & Contributors. All rights reserved. See License.md in the project root for license information.

namespace OData2Poco.Extensions;

using System.Text.RegularExpressions;

public static class IniParser
{
    public static Dictionary<string, Dictionary<string, object>> ParseIni(this string iniData)
    {
        const string SectionPattern = @"^\[\s*(\w+)\s*\]$";
        const string KeyValuePattern = @"^(\w+)\s*=\s*(.*)$";

        Dictionary<string, Dictionary<string, object>> config = new(StringComparer.OrdinalIgnoreCase);
        var currentSection = "__global__";
        var currentKey = string.Empty;
        using StringReader reader = new(iniData);
        while (reader.ReadLine() is { } line)
        {
            var currentLine = line.Trim();
            if (string.IsNullOrEmpty(currentLine)
                || currentLine.StartsWith(";")
                || currentLine.StartsWith("#"))
            {
                continue;
            }

            var sectionMatch = Regex.Match(currentLine, SectionPattern);

            if (sectionMatch.Success)
            {
                currentSection = sectionMatch.Groups[1].Value.Trim();

                if (!config.ContainsKey(currentSection))
                {
                    config[currentSection] = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
                }

                continue;
            }

            //match key value pairs
            var keyValueMatch = Regex.Match(currentLine, KeyValuePattern);

            if (keyValueMatch.Success)
            {
                currentKey = keyValueMatch.Groups[1].Value.Trim();
                var value = keyValueMatch.Groups[2].Value.Trim().Trim('"');
                config[currentSection][currentKey] = value;
            }
            else
            {
                config[currentSection][currentKey] = $"{config[currentSection][currentKey]}\n{currentLine}";
            }
        }

        return config;
    }
}
