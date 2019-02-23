using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace OData2Poco.OAuth2
{
    public class TokenEndpoint
    {
        public string TokenUrl { get; set; }
        public string TokenParams { get; set; }
        public Dictionary<string, string> TokenParamsCollection { get; set; }
        public TokenEndpoint(OdataConnectionString odataConnectionString)
        {
            TokenUrl = odataConnectionString.TokenUrl;
            TokenParams = odataConnectionString.TokenParams;
            TokenParamsCollection = odataConnectionString.TokenParamsAsDictionary();
        }

        public async Task<string> GetAccessTokenAsync() => await GetToken(new Uri(TokenUrl), TokenParamsCollection);

        public async Task<string> GetToken(Uri authenticationUrl, Dictionary<string, string> authenticationCredentials)
        {
            var client = new HttpClient();
            var content = new FormUrlEncodedContent(authenticationCredentials);
            var response = await client.PostAsync(authenticationUrl, content);
            if (response.StatusCode != System.Net.HttpStatusCode.OK)
                throw new Exception($"Fail to get access_token -Error: {response.StatusCode}");
            var responseString = await response.Content.ReadAsStringAsync();
            var jtoken = JToken.Parse(responseString);
            return jtoken != null ? jtoken["access_token"].ToString() : string.Empty;
        }
    }
}
