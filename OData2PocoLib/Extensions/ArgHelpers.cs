// Copyright (c) Mohamed Hassan & Contributors. All rights reserved. See License.md in the project root for license information.

namespace OData2Poco.Extensions;

using System.Text.RegularExpressions;

public static class ArgHelper
{
    public static string[] MergeRepeatingArgs(this string[] args)
    {
        Debug.Assert(args != null, nameof(args) + " != null");
        if (args.Length <= 2 || args.Length == args.Distinct().Count())
        {
            return args;
        }

        Dictionary<string, List<string>> dict = new()
        {
            {
                string.Empty, []
            }
        };
        List<string> list = [];
        Regex regex = new(@"^-{1,2}\w+$");
        for (var i = 0; i < args.Length; i++)
        {
            var arg = args[i];
            if (arg == "-")
            {
                arg = "?";
            }

            if (regex.IsMatch(arg))
            {
                if (!dict.TryGetValue(arg, out var values))
                {
                    values = [];
                    dict.Add(arg, values);
                }

                for (var j = i + 1; j < args.Length; j++)
                {
                    var nextArg = args[j];
                    if (regex.IsMatch(nextArg))
                    {
                        break;
                    }

                    values.Add(nextArg);
                    i++;
                }
            }
            else
            {
                //add arg to the empty entry
                dict[string.Empty].Add(arg);
            }
        }

        foreach (var kvp in dict)
        {
            if (!string.IsNullOrEmpty(kvp.Key))
            {
                list.Add(kvp.Key);
            }

            if (kvp.Value.Count > 0)
            {
                list.AddRange(kvp.Value);
            }
        }

        return list.ToArray();
    }
}
