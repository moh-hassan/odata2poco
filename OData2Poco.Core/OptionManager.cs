using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;
using OData2Poco.Exceptions;
using OData2Poco.Extensions;


namespace OData2Poco.CommandLine
{
    internal class OptionManager
    {
        private Options Options { get; set; }
        internal Dictionary<string, string> EnvironmentVariables { get; set; }

        public OptionManager()
        {
            var comparer = StringComparer.OrdinalIgnoreCase;
            EnvironmentVariables = new Dictionary<string, string>(comparer);
        }
        public OptionManager(Options options) : this()
        {
            Options = options;
            Init();
            options.Validate();
        }

        public PocoSetting GetPocoSetting()
        {
            return new PocoSetting
            {
                AddNavigation = Options.Navigation,
                AddNullableDataType = Options.AddNullableDataType,
                AddEager = Options.Eager,
                Inherit = string.IsNullOrWhiteSpace(Options.Inherit) ? null : Options.Inherit,
                NamespacePrefix = string.IsNullOrEmpty(Options.Namespace) ? string.Empty : Options.Namespace,
                NameCase = Options.NameCase.ToCaseEnum(),
                Attributes = Options.Attributes?.ToList(),
                //obsolete
                AddKeyAttribute = Options.Key,
                AddTableAttribute = Options.Table,
                AddRequiredAttribute = Options.Required,
                AddJsonAttribute = Options.AddJsonAttribute,
            };
        }

        public OdataConnectionString GetOdataConnectionString()
        {
            var connString = new OdataConnectionString
            {
                ServiceUrl = Options.Url,
                UserName = Options.User,
                Password = Options.Password,
                TokenUrl = Options.TokenEndpoint,
                TokenParams = Options.TokenParams,
                ParamFile = Options.ParamFile
            };
            return connString;
        }

        #region Environment Variables

        public void Init()
        {
            if (string.IsNullOrEmpty(Options.ParamFile)) return;
            EnvironmentVariables = LoadEnvironment();
            //replace expressions containin 'key=?' placeholder with macro {{key}}
            foreach (var keyValuePair in EnvironmentVariables.ToArray())
            {
                var pattern = "(\\w+?)=\\?";
                var text2 = Regex.Replace(keyValuePair.Value, pattern, "$1={{$1}}");
                EnvironmentVariables[keyValuePair.Key] = text2;
            }

            EnvironmentVariables = new EvaluateExprssion(EnvironmentVariables).ResolveMacros();
            Options.Url = EvalMacro(Options.Url);
            Options.Password = EvalMacro(Options.Password);
            Options.User = EvalMacro(Options.User);
            Options.TokenEndpoint = EvalMacro(Options.TokenEndpoint);
            Options.TokenParams = EvalMacro(Options.TokenParams);
        }

        internal Dictionary<string, string> LoadEnvironment()
        {
            try
            {

                var comparer = StringComparer.OrdinalIgnoreCase;
                Dictionary<string, string> dic = new Dictionary<string, string>(comparer);
                if (string.IsNullOrEmpty(Options.ParamFile)) return dic;
                var text = File.ReadAllText(Options.ParamFile);
                return LoadEnvironment(text);
            }
            catch (Exception ex)
            {
                throw new Odata2PocoException($"Fail to read the parameter file. {ex.Message}");
            }
        }


        internal Dictionary<string, string> LoadEnvironment(string text)
        {
            KeyValuePair<string, string> StringToKeyValue(string arg)
            {
                var index = arg.IndexOf('=');
                var key = arg.Substring(0, index);
                var value = arg.Substring(index + 1).Trim();
                return new KeyValuePair<string, string>(key, value);
            }

            if (string.IsNullOrEmpty(text)) return EnvironmentVariables;
            if (text.StartsWith("{")) //json
            {

                JObject jo = JObject.Parse(text);
                JToken token = jo["values"] as JArray;
                if (token != null)
                {
                    foreach (var jToken in token)
                    {
                        EnvironmentVariables.Add(jToken["key"].ToString(), jToken["value"].ToString());
                    }
                }
            }
            else   //text file
            {
                var lines = Regex.Split(text, "\r\n|\r|\n");
                foreach (var line in lines)
                {
                    if (string.IsNullOrEmpty(line)) continue;
                    var entry = StringToKeyValue(line);
                    EnvironmentVariables.Add(entry.Key.Trim(), entry.Value.Trim());
                }
            }

            return EnvironmentVariables;
        }


        //evaluate simple expression : "{{url}}" and get its value from dictionary
        //or complex expression "client_id={{client_id}}&client_secret={{client_secret}}"
        public string EvalMacro(string expression)
        {
            if (string.IsNullOrEmpty(expression))
                return "";
            if (!expression.Contains("{{"))
                return expression;
            var pattern = @"{{(\w+?)}}";
            var text = Regex.Replace(expression, pattern,
                m =>
                {
                    var expr = m.Groups[1].ToString();
                    var val = GetVariable(expr);
                    return val;
                }
               );
            return text;
        }

        internal string GetVariable(string name)
        {
            if (EnvironmentVariables.ContainsKey(name))
                return EnvironmentVariables[name];
            //todo warn/error for null value
            throw new Odata2PocoException($"Variable not found:{name}");
        }
        #endregion

        public static implicit operator Options(OptionManager optionManager)
        {
            return optionManager.Options;
        }
    }
}
