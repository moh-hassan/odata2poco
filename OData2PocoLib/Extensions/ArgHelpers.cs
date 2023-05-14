// Copyright (c) Mohamed Hassan & Contributors. All rights reserved. See License.md in the project root for license information.

using System.Text.RegularExpressions;

namespace OData2Poco.Extensions;

public static class ArgHelper
{
    public static string[] MergeRepeatingArgs(this string[] args)
    {
        if (args.Length <= 2 || args.Length == args.Distinct().Count()) return args;
        var dict = new Dictionary<string, List<string>> { { "", new List<string>() } };
        var list = new List<string>();
        var regex = new Regex(@"^-{1,2}\w+$");
        for (var i = 0; i < args.Length; i++)
        {
            var arg = args[i];
            if (arg == "-") arg = "?";
            if (regex.IsMatch(arg))
            {
                if (!dict.TryGetValue(arg, out var values))
                {
                    values = new List<string>();
                    dict.Add(arg, values);
                }

                for (var j = i + 1; j < args.Length; j++)
                {
                    var nextArg = args[j];
                    if (regex.IsMatch(nextArg)) break;
                    values.Add(nextArg);
                    i++;
                }
            }
            else
            {
                //add arg to the empty entry
                dict[""].Add(arg);
            }
        }

        foreach (var kvp in dict)
        {
            if (kvp.Key != string.Empty)
                list.Add(kvp.Key);
            if (kvp.Value.Count > 0)
                list.AddRange(kvp.Value);
        }

        return list.ToArray();
    }
}