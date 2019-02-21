using System;
using System.Collections.Generic;

namespace OData2Poco
{
    public class OdataConnectionString
    {

        public string ServiceUrl { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public AuthenticationEnum AuthenticationType { get; set; }
        public string TokenUrl { get; set; }
        public string TokenParams { get; set; }
        public string ParamFile { get; set; }

        //internal properties
        public Dictionary<string, string> EnvironmentVariables { get; set; } //=> LoadEnvironment();

        //todo if EnvironmentPath not null=>load the file in jobject=>then resolve any ? or {{}} in parameters
        //file maybe text or postman environment=>start text
        //public void Init()
        //{
        //    Console.WriteLine(this.Dump());

        //    //load parameter file
        //    Console.WriteLine($"init:ParamFile= {ParamFile}");
        //    if (!string.IsNullOrEmpty(ParamFile))
        //        EnvironmentVariables = LoadEnvironment();
        //    //replace ?
        //    //todo check value =? ==>substitue its vale from env. vars
        //    //todo value with  {{}}
        //    if (ServiceUrl != null && ServiceUrl.Contains("{{"))
        //        ServiceUrl = EvalMacro(ServiceUrl);
           
        //    //if ( Password != null && Password.Contains("{{"))
        //        Password = EvalMacro(Password);
          
        //    //if (UserName != null && UserName.Contains("{{"))
        //        UserName = EvalMacro(UserName);

        //    //if (TokenUrl != null && TokenUrl.Contains("{{"))
        //        TokenUrl = EvalMacro(TokenUrl);
          
        //    //if (TokenParams != null && TokenParams.Contains("{{"))
        //        TokenParams = EvalMacro(TokenParams);
        //    Console.WriteLine($"+++++ {this.Dump()}");
        //}

        //public string EvalMacro(string expression)
        //{
        //    if (string.IsNullOrEmpty(expression))
        //        return "";
        //    if (!expression.Contains("{{"))
        //        return expression;
        //    //Dict[key] = expression;
        //    //var original = expression;
        //    //Console.WriteLine($"start evaluate: key='{key}' , expression= '{expression}'");
        //    var pattern = @"{{([a-zA-Z0-9_]+?)}}";
        //    var text = Regex.Replace(expression, pattern,
        //          m =>
        //          {
        //              var expr = m.Groups[1].ToString();
        //              var val = GetVariable(expr);
        //            //var val = EvalExpression(expr, Dict);

        //            //fill the new value
        //            //if (!string.IsNullOrEmpty(val)) dict[expr] = val;
        //            Console.WriteLine($"regex: exp={expr}, val= {val}");

        //              return val;
        //          },
        //          RegexOptions.IgnorePatternWhitespace | RegexOptions.IgnoreCase);
        //    Console.WriteLine($"Regex final Result: '{text}'");
        //    return text;


        //}
        
        //unused
        //public string GetMacroVariable(string key)
        //{

        //    var result = "";
        //    Console.WriteLine($"in Dict search for: '{key}'");
        //    if (EnvironmentVariables.ContainsKey(key))
        //    {
        //        result = EnvironmentVariables[key];
        //        Console.WriteLine($"in Dict, find result: '{result}'");
        //        return result;
        //    }
        //    //throw new Exception($"Variable not found:{key}");
        //    //todo wan/error key not found
        //    return "";
        //}
        ////OAuth2
        ////todo validate tokenparameters
        //public async Task<string> GetAccessTokenAsync()
        //{
        //    Console.WriteLine($"+++++ Connecting to Token EndPoint: {TokenUrl}");
        //    if (!string.IsNullOrEmpty(TokenUrl))
        //    {
        //        TokenEndPoint tokenEndPoint =
        //            new TokenEndPoint(TokenUrl, TokenParams.ToArray());
        //        string accessToken = await tokenEndPoint.GetAccessTokenAsync();
        //        Password = accessToken;
        //        Console.WriteLine($"+++++Reply with access_token: {accessToken}");
        //        return Password;
        //    }

        //    return "";
        //}


        //Dictionary<string, string> LoadEnvironment()
        //{
        //    Console.WriteLine("LoadEnvironment");
        //    Dictionary<string, string> dic = new Dictionary<string, string>();
        //    if (!string.IsNullOrEmpty(ParamFile))
        //    {
        //        var text = File.ReadAllText(ParamFile);
        //        if (text.StartsWith("{")) //json
        //        {
        //            //do json stuff
        //        }
        //        else   //text file
        //        {
        //            Console.WriteLine("read file");
        //            Console.WriteLine(text);
        //            var lines = Regex.Split(text, "\r\n|\r|\n");
        //            Console.WriteLine($"lines.Length {lines.Length}");
        //            foreach (var line in lines)
        //            {
        //                Console.WriteLine($"line {line}");
        //                if (!string.IsNullOrEmpty(line))
        //                {
        //                    var entry = StringToKeyValue(line);
        //                    dic.Add(entry.Key.Trim(), entry.Value.Trim());
        //                    Console.WriteLine($"{entry.Key} = {entry.Value}");
        //                }
        //            }
        //        }

        //    }

        //    return dic;
        //}

        //internal string GetVariable(string name)
        //{
        //    if (EnvironmentVariables.ContainsKey(name))
        //        return EnvironmentVariables[name];
        //    //todo warn/error for null value
        //    return "";
        //}
        public Dictionary<string, string> TokenParamsAsDictionary()
        {
            if (string.IsNullOrEmpty(TokenParams))
                return new Dictionary<string, string>();

            //check if it's a file
            //if (TokenParams.StartsWith("@"))
            //{
            //    var fname = TokenParams.Substring(1);
            //    //Console.WriteLine(fname);
            //    var json = File.ReadAllText(fname);
            //    var dict1 = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
            //    Console.WriteLine(dict1.Dump());
            //    return dict1;
            //}

            //Console.WriteLine($"parastring: {tokenParameters}");
            Dictionary<string, string> dict = new Dictionary<string, string>();

            var args = TokenParams.Split(new[] { '&' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var s in args)
            {

                var pair = StringToKeyValue(s);
                Console.WriteLine($"{s}==> {pair.Key} = {pair.Value}");

                dict.Add(pair.Key, pair.Value);
            }
            return dict;
        }
        public KeyValuePair<string, string> StringToKeyValue(string arg)
        {
            Console.WriteLine(arg);
            var index = arg.IndexOf('=');
            var key = arg.Substring(0, index);
            var value = arg.Substring(index + 1).Trim();
            return new KeyValuePair<string, string>(key, value);
        }

    }

    public enum AuthenticationEnum
    {
        None = 0,
        Basic,
        OAuth,
        Token
    }
}
