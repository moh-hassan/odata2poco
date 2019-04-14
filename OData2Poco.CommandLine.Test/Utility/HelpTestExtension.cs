using System;
using System.Linq;
using System.Text.RegularExpressions;
// ReSharper disable CheckNamespace

namespace OData2Poco.TestUtility
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
            var pattern=text;
            //char[] escapes=   {'\\' ,  '*', '+', '?', '|', '{', '[', '(',')', '^', '$', '.', '#'};
            if (!string.IsNullOrEmpty(escapeChar))
            {
                char[] chars = escapeChar.ToCharArray();
                pattern = chars.Aggregate(pattern, (current, c) => current.Replace(c.ToString(), $"\\{c}"));
            }
             pattern = Regex.Replace(pattern, @"\s+", @"\s*");
            return pattern;
        }

    }
}
