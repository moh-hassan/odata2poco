using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using OData2Poco.InfraStructure.Logging;

namespace OData2Poco.Http
{
    /// <summary>
    /// Generate access_token for OAuth2
    /// </summary>
    internal class TokenEndpoint
    {
        public static ILog Logger = PocoLogger.Default;
        public string TokenUrl { get; set; }
        public string TokenParams { get; set; }
        public Dictionary<string, string> TokenParamsCollection { get; set; }
        //json string 
        public string? LastToken { get; set; }
        public TokenEndpoint(OdataConnectionString odataConnectionString)
        {
            //OConnectionString = odataConnectionString;
            TokenUrl = odataConnectionString.TokenUrl;
            TokenParams = SetTokenParams(odataConnectionString);
            TokenParamsCollection = TokenParamsAsDictionary();

        }

        internal Dictionary<string, string> TokenParamsAsDictionary()
        {
            if (string.IsNullOrEmpty(TokenParams))
                return new Dictionary<string, string>();
            var args = TokenParams.Split(new[] { '&' }, StringSplitOptions.RemoveEmptyEntries);
            return args.Select(StringToKeyValue).ToDictionary(pair => pair.Key, pair => pair.Value);
        }

        internal KeyValuePair<string, string> StringToKeyValue(string arg)
        {
            if (string.IsNullOrEmpty(arg))
                return new KeyValuePair<string, string>();
            var index = arg.IndexOf('=');
            var key = arg.Substring(0, index);
            var value = arg.Substring(index + 1).Trim();
            return new KeyValuePair<string, string>(key, value);
        }

        private string SetTokenParams(OdataConnectionString odataConnectionString)
        {
            var clientParams = $"grant_type=client_credentials&client_id={odataConnectionString.UserName}&client_secret={odataConnectionString.Password}";
            string tokenParams = string.IsNullOrEmpty(odataConnectionString.TokenParams)
                ? clientParams
                : $"{clientParams}&{odataConnectionString.TokenParams}";

            return tokenParams;
        }
        public async Task<string?> GetAccessTokenAsync()
        {
            Logger.Normal($"Start connecting to Token endpoint: {TokenUrl}");
            var token = await GetToken(new Uri(TokenUrl), TokenParamsCollection);
            Logger.Normal($"Token endpoint reply with access_token");
            return token;
        }

        /*
 Note: The json token is in the form:

token_type     : Bearer
expires_in     : 3600
ext_expires_in : 3600
expires_on     : 1553441019
not_before     : 1553437119
resource       : https://resource.com
access_token   : bi05REFMcXdodUhZbkhRNjNHZUNYYyIsImtpZCI6Ik4tbEMwbi05REFMcXdod...

*/
        public async Task<string?> GetToken(Uri authenticationUrl, 
            Dictionary<string, string> authenticationCredentials)
        {
            var client = new HttpClient();
            var content = new FormUrlEncodedContent(authenticationCredentials!);
            var response = await client.PostAsync(authenticationUrl, content);
            if (response.StatusCode != System.Net.HttpStatusCode.OK)
                throw new Exception($"Fail to get access_token, Http status code: ({(int)response.StatusCode}) {response.StatusCode}");
            var responseString = await response.Content.ReadAsStringAsync();
            LastToken = responseString;
            return ParseTokenResponse(responseString, "access_token");
        }
        public DateTime ToLocalDateTime(long unixDate)
        {
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            DateTime date = epoch.AddSeconds(unixDate);
            return date.ToLocalTime();
        }

        internal DateTime ExpiresOn()
        {
            if (string.IsNullOrEmpty(LastToken))
                return DateTime.MinValue;
            var expireOn = ParseTokenResponse(LastToken, "expires_on");
            var expireOnLong = Convert.ToInt64(expireOn);
            var date = ToLocalDateTime(expireOnLong);
            return date;
        }
        internal string? ParseTokenResponse(string? content, string key)
        {
            if (string.IsNullOrEmpty(content) || string.IsNullOrEmpty(key))
                return null;
            var token = JObject.Parse(content!).SelectToken(key);
            return token?.ToString();

        }
    }
}


