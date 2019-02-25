using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;


namespace OData2Poco.CommandLine
{
    public class EvaluateExprssion
    {
        private Dictionary<string, string> Dict { get; set; }
        const string StartString = "{{";
        const string EndString = "}}";
        public EvaluateExprssion(Dictionary<string, string> dict)
        {
            Dict = dict;

        }

        public void EvalMacro(KeyValuePair<string, string> entry, out string text)
        {
            EvalMacro(entry.Key, entry.Value, out text);
        }

        public void EvalMacro(string key, string expression, out string text)
        {

            if (expression.Contains($"{StartString}{key}{EndString}"))
                throw new Exception($"Recursive expression in evaluating the entry: {key} ={expression}");
            if (!expression.Contains("{{"))
            {
                text = expression;
                Dict[key] = text;
                return;
            }
            var pattern = @"{{([a-zA-Z0-9_]+?)}}";
            text = Regex.Replace(expression, pattern,
                m =>
                {
                    var expr = m.Groups[1].ToString();
                    var val = EvalExpression(expr);
                    return val;
                },
                RegexOptions.IgnorePatternWhitespace | RegexOptions.IgnoreCase);
            EvalMacro(key, text, out text);
        }

        public string EvalExpression(string expression)
        {
            return GetValue(expression, Dict);
        }

        public string GetValue(string key, Dictionary<string, string> dict)
        {
            if (!dict.ContainsKey(key)) throw new Exception($"Variable not found:{key}");
            var result = dict[key];
            return result;
        }

        public Dictionary<string, string> ResolveMacros()
        {
            foreach (var keyValuePair in Dict.ToArray())
            {
                var key = keyValuePair.Key;
                var value = keyValuePair.Value;
                EvalMacro(key, value, out var text);
            }
            return Dict;
        }
    }
}