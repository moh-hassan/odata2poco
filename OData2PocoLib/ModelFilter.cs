using OData2Poco.graph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace OData2Poco
{
    static class ModelFilter
    {
        public static IEnumerable<ClassTemplate> FilterList(this List<ClassTemplate> classList,
            List<string> filter)
        {
            var result = new List<ClassTemplate>();
            var list = Search(classList, filter);
            result.AddRange(list);
            var deps = Dependency.Search(classList, list.ToArray());
            result.AddRange(deps);
            return result.Distinct();
        }
        static IEnumerable<ClassTemplate> Search(this List<ClassTemplate> classList,
       List<string> filter)
        {
            if (filter == null)
            {
                foreach (var c in classList)
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
            foreach (var item in classList)
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
