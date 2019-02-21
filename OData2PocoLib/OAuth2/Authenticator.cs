using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using OData2Poco.InfraStructure.Logging;

namespace OData2Poco.OAuth2
{
    internal class Authenticator
    {
        public static ColoredConsole Logger = PocoLogger.Default;
        private readonly HttpClient _client;
        public Authenticator(HttpClient client)
        {
            _client = client;
        }
        public async Task Authenticate(OdataConnectionString odataConnString)
        {
            //Basic auth user/password
            if (!string.IsNullOrEmpty(odataConnString.UserName) &&
                !string.IsNullOrEmpty(odataConnString.Password))
                Authenticate(odataConnString.UserName, odataConnString.Password);

            //token
            if (!string.IsNullOrEmpty(odataConnString.Password) &&
                string.IsNullOrEmpty(odataConnString.UserName))
                Authenticate(odataConnString.Password);

            //OAuth2 
            if (!string.IsNullOrEmpty(odataConnString.TokenUrl))
            {

                Logger.Info("Authenticating with OAuth2");
                //Console.WriteLine(odataConnString.Dump());
                var accessToken = await GetAccessTokenAsync(odataConnString);
                Authenticate(accessToken);
                //Environment.Exit(0); //test

            }
        }


        private void Authenticate(string user, string password)
        {
            //credintial
            if (!string.IsNullOrEmpty(user))
            {
                var token = Convert.ToBase64String(Encoding.UTF8.GetBytes(user + ":" + password));
                Console.WriteLine(token);
                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", token);
            }
        }

        private void Authenticate(string token)
        {
            //credintial
            if (!string.IsNullOrEmpty(token))
            {
                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                _client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json; charset=utf-8");

            }
        }

        public async Task<string> GetAccessTokenAsync(OdataConnectionString odataConnString)
        {
            Logger.Normal($"Start Connecting to Token endpoint: {odataConnString.TokenUrl}");
                if (!string.IsNullOrEmpty(odataConnString.TokenUrl))
            {
                TokenEndpoint tokenEndPoint =
                    new TokenEndpoint(odataConnString);
                string accessToken = await tokenEndPoint.GetAccessTokenAsync();
                odataConnString.Password = accessToken;
                Logger.Normal($"Token endpoint Reply with access_token: {accessToken}");
                File.WriteAllText("access_token.txt", accessToken);
                return accessToken;
            }

            return "";
        }
    }
}