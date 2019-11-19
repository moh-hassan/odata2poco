using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace OData2Poco
{
    class ModelFilter
    {
        public static IEnumerable<ClassTemplate> FilterList(List<ClassTemplate> list1,
            List<string> filter)
        {

            if (filter == null)
            {
                foreach (var c in list1)
                    yield return c;
                yield break;
            }
            //add * prefix to name if it do not contain namespace.
            filter = filter.Select(x => !x.StartsWith("*") || x.Contains(".")
                 ? x
                 : $"*{x}").ToList();
            var list2 = filter.Select(x =>
                x.Replace("*", @"[\w_]*")
                    .Replace("?", @"[\w_]")
                    .Replace(".", "\\."));

            list2 = list2.Select(x => $"\\b{x}\\b");
            var pattern = string.Join("|", list2);
            foreach (var item in list1)
            {
                var name = item.Name;

                if (!string.IsNullOrEmpty(item.NameSpace))
                    name = $"{item.NameSpace}.{item.Name}";

                var match = Regex.Match(name, pattern,
                    RegexOptions.IgnoreCase
                          | RegexOptions.IgnorePatternWhitespace);
                if (match.Success)
                    yield return item;
            }
        }
    }
}
