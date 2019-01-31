using System;
using System.Text.RegularExpressions;

namespace OData2Poco.CommandLine.Test
{
    public static class HelpTestExtension
    {
        public static string[] SplitArgs(this string args)
        {
            return args.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
        }
      
        public static string[] HelpToLines(this string help)
        {
            return Regex.Split(help, "\r\n|\r|\n");
        }
        public static string GetRegexPattern(this string text, string escapeChar = "()[]?")
        {
            var pattern = Regex.Replace(text.Trim(), @"\s+", @"\s*");
            if (!string.IsNullOrEmpty(escapeChar))
            {
                char[] chars = escapeChar.ToCharArray();
                foreach (char c in chars)
                {
                    pattern = pattern.Replace(c.ToString(), $"\\{c}");
                }
            }

            return pattern;
        }
    }
}
