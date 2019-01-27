using System;
using System.Collections.Generic;
using System.Linq;
using CommandLine;

namespace OData2Poco.CommandLine
{
  internal  class CommandLineUtility
    {
        public static void ShowOptions(Options option)
        {
            //format option as: -n Navigation= True
            Console.WriteLine("************* CommandLine options***********");
            var list = GetOptions( option);
            list.ForEach(Console.WriteLine);
            Console.WriteLine("********************************************");
        }

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

                var att = (OptionAttribute)p1.attrs?.FirstOrDefault();
                if (att == null) continue;
                var shortName = att.ShortName == null ? $"--{att.LongName}" : $"-{att.ShortName}";
                var text = $"{shortName} {p1.p.Name}= {val} ";
                list.Add(text);
            }
            return list;
        }

    }
}
