// Copyright (c) Mohamed Hassan & Contributors. All rights reserved. See License.md in the project root for license information.

using System.Text.RegularExpressions;

namespace OData2Poco;

public static class IniParser
{
    public static Dictionary<string, Dictionary<string, object>> ParseIni(this string iniData)
    {
        const string sectionPattern = @"^\[\s*(\w+)\s*\]$";
        const string keyValuePattern = @"^(\w+)\s*=\s*(.*)$";

        var config = new Dictionary<string, Dictionary<string, object>>(StringComparer.OrdinalIgnoreCase);
        var currentSection = "__global__";
        var currentKey = "";
        using var reader = new StringReader(iniData);
        while (reader.ReadLine() is { } line)
        {
            var currentLine = line.Trim();
            if (currentLine == string.Empty
                || currentLine.StartsWith(";")
                || currentLine.StartsWith("#"))
                continue;

            var sectionMatch = Regex.Match(currentLine, sectionPattern);

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
            var keyValueMatch = Regex.Match(currentLine, keyValuePattern);

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