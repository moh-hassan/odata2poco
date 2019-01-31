using System;
using System.Collections.Generic;
using System.Text;
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
    }
}
