using System;
using System.Collections;
using System.Collections.Generic;

namespace OData2Poco.OAuth2
{
    public class TokenSplitter
    {
        const char   separator ='=';
        //: in http , = in values, get first colon :
        //it should as json string { }
        //public static KeyValuePair<string, string> SplitKeyValue0(string arg)
        //{
        //    var tokens = arg.Split(new[] { '=' }, StringSplitOptions.RemoveEmptyEntries);
        //    if (tokens.Length < 2)
        //        return default;
        //    return new KeyValuePair<string, string>(tokens[0], tokens[1]);
        //}

        public static KeyValuePair<string, string> SplitKeyValue(string arg)
        {
            var index = arg.IndexOf(separator);
            var key = arg.Substring(0, index );
            var value= arg.Substring(index +1);
            return new KeyValuePair<string, string>(key, value);
        }
        public static Hashtable ToHashTable(string[] args)
        {
            Hashtable tokenParameters = new Hashtable(StringComparer.InvariantCultureIgnoreCase);
            foreach (var s in args)
            {
                KeyValuePair<string, string> entry = TokenSplitter.SplitKeyValue(s);
                if (entry.Key != null)
                    tokenParameters.Add(entry.Key, entry.Value);
            }
            //foreach (DictionaryEntry entry in tokenParameters)
            //{
            //    Console.WriteLine($"{entry.Key} | {entry.Value}");
            //}

            return tokenParameters;
        }
    }
}
