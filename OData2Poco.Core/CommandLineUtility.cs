// Copyright (c) Mohamed Hassan & Contributors. All rights reserved. See License.md in the project root for license information.

using CommandLine;

namespace OData2Poco.CommandLine;

internal static class CommandLineUtility
{
    public static List<string> GetOptions(Options option)
    {
        var list = new List<string>();
        var t = option.GetType();
        var props = t.GetProperties()
            .Where(p => p.GetValue(option) != null)
            .Select(p => new { p, attrs = p.GetCustomAttributes(typeof(OptionAttribute), false) });

        foreach (var p1 in props)
        {
            if (p1.p.Name.Contains("Example")) continue;
            var val = p1.p.GetValue(option);

            switch (val)
            {
                case string[] s:
                    {
                        val = string.Join(",", s);
                        break;
                    }
                case bool b:
                    if (!b) continue;
                    break;
            }

            var att = (OptionAttribute)p1.attrs.FirstOrDefault();
            if (att == null) continue;
            var shortName = att.ShortName == null ? $"--{att.LongName}" : $"-{att.ShortName}";
            if (!string.IsNullOrEmpty(val?.ToString()))
            {
                var text = $"{shortName} {p1.p.Name}= {val} ";
                if (shortName.Equals("-p") || shortName.Equals("--password")) //hide password for security reason
                    text = $"{shortName} {p1.p.Name}= ***** ";
                list.Add(text);
            }
        }
        CodeHeader.SetParameters(list);
        return list;
    }

}