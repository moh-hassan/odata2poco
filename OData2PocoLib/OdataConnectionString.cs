using System;
using System.Collections.Generic;
using System.Linq;

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
        public Dictionary<string, string> TokenParamsAsDictionary()
        {
            if (string.IsNullOrEmpty(TokenParams))
                return new Dictionary<string, string>();
            var args = TokenParams.Split(new[] { '&' }, StringSplitOptions.RemoveEmptyEntries);
            return args.Select(StringToKeyValue).ToDictionary(pair => pair.Key, pair => pair.Value);
        }
        public KeyValuePair<string, string> StringToKeyValue(string arg)
        {
            if (string.IsNullOrEmpty(arg))
                return new KeyValuePair<string, string>();
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
        OAuth2,
        Token
    }
}
