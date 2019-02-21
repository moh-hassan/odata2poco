using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OData2Poco.Extensions;

namespace OData2Poco.OAuth2
{
    public class TokenEndpoint
    {
        //static readonly Dictionary<string, string> TokenServers = new Dictionary<string, string>
        //{
        //    ["google"] = "https://accounts.google.com/o/oauth2/token",
        //    ["ms"] = "https://login.microsoftonline.com/edgewater.onmicrosoft.com/oauth2/token",

        //};
        public string TokenUrl { get; set; }
        public string TokenParams { get; set; }
        public Dictionary<string, string> TokenParamsCollection { get; set; }
        private string GetTokenServer(string tokenUrl)
        {
            var server = tokenUrl;
            if (tokenUrl.StartsWith("http"))
                server = tokenUrl;
            //else if (TokenServers.ContainsKey(tokenUrl))
            //{
            //    server = TokenServers[tokenUrl];
            //}

            return server;
        }
        //public TokenEndpoint(string tokenUrl, string tokenParams)
        //{
        //    TokenUrl = tokenUrl;
        //    TokenParams = tokenParams;
        //    TokenParamsCollection = ConvertToDictionary(tokenParams);
        //}

        public TokenEndpoint(OdataConnectionString odataConnectionString)
        {
            TokenUrl = odataConnectionString.TokenUrl;
            TokenParams = odataConnectionString.TokenParams;
            TokenParamsCollection = odataConnectionString.TokenParamsAsDictionary();
        }

        //Hashtable GetTokenParams(string[] args)
        //{
        //    return TokenSplitter.ToHashTable(args);
        //}

        //Dictionary<string, string> ConvertToDictionary(string tokenParameters)
        //{
        //    //check if it's a file
        //    if (tokenParameters.StartsWith("@"))
        //    {
        //        var fname = tokenParameters.Substring(1);
        //        //Console.WriteLine(fname);
        //        var json = File.ReadAllText(fname);
        //        var dict1 = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
        //        Console.WriteLine(dict1.Dump());
        //        return dict1;
        //    }

        //    //Console.WriteLine($"parastring: {tokenParameters}");
        //    Dictionary<string, string> dict = new Dictionary<string, string>();

        //    var args = tokenParameters.Split(new[] { '&' }, StringSplitOptions.RemoveEmptyEntries);
        //    foreach (var s in args)
        //    {

        //        var pair = SplitKeyValue(s);
        //        Console.WriteLine($"{s}==> {pair.Key} = {pair.Value}");

        //        dict.Add(pair.Key, pair.Value);
        //    }
        //    return dict;
        //}

        //public KeyValuePair<string, string> SplitKeyValue(string arg)
        //{
        //    var index = arg.IndexOf('=');
        //    var key = arg.Substring(0, index);
        //    var value = arg.Substring(index + 1);
        //    return new KeyValuePair<string, string>(key, value);
        //}
        public async Task<string> GetAccessTokenAsync()
        {

            // return await GetAccessTokenAsync(TokenUrl, TokenParams);
            return await GetToken(new Uri(TokenUrl), TokenParamsCollection);

        }
        public async Task<string> GetToken(Uri authenticationUrl, Dictionary<string, string> authenticationCredentials)
        {
            //Console.WriteLine("++enter GetToken");
            //Console.WriteLine($"authenticationUrl {authenticationUrl}");
            //Console.WriteLine($"{authenticationCredentials.Dump()}");
            HttpClient client = new HttpClient();

            FormUrlEncodedContent content = new FormUrlEncodedContent(authenticationCredentials);

            HttpResponseMessage response = await client.PostAsync(authenticationUrl, content);

            if (response.StatusCode != System.Net.HttpStatusCode.OK)
            {
                string message = $"Fail to get access_token -Error: {response.StatusCode}";
                Console.WriteLine($"{message}");
                throw new Exception(message);
            }

            string responseString = await response.Content.ReadAsStringAsync();
            //
            JToken jtoken = JToken.Parse(responseString);
            //Console.WriteLine("++exit GetToken");
            //  return responseString;
            if (jtoken != null) return jtoken["access_token"].ToString();
            return "";
        }
    }
}
