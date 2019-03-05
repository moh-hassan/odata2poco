using System;
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
            switch (odataConnString.Authenticate)
            {
                case AuthenticationType.none:
                    break;

                case AuthenticationType.basic:
                    Logger.Info("Authenticating with Basic");
                    //Basic auth user/password
                    if (!string.IsNullOrEmpty(odataConnString.UserName) &&
                        !string.IsNullOrEmpty(odataConnString.Password))
                    {

                        Authenticate(odataConnString.UserName, odataConnString.Password);
                    }
                    break; ;

                case AuthenticationType.token:
                    Logger.Info("Authenticating with Token");
                    //token
                    if (!string.IsNullOrEmpty(odataConnString.Password) &&
                        string.IsNullOrEmpty(odataConnString.UserName))
                        Authenticate(odataConnString.Password);
                    break;
                case AuthenticationType.oauth2:
                    Logger.Info("Authenticating with OAuth2");
                    //OAuth2 
                    if (!string.IsNullOrEmpty(odataConnString.TokenUrl))
                    {
                        var accessToken = await new TokenEndpoint(odataConnString).GetAccessTokenAsync();
                        Authenticate(accessToken);
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        private AuthenticationHeaderValue Authenticate(string user, string password)
        {
            //credintial
            if (string.IsNullOrEmpty(user)) return null;
            var token = Convert.ToBase64String(Encoding.UTF8.GetBytes(user + ":" + password));
            var headerValue = new AuthenticationHeaderValue("Basic", token);
            _client.DefaultRequestHeaders.Authorization = headerValue;
            return headerValue;
        }
        private AuthenticationHeaderValue Authenticate(string token)
        {
            //credintial
            if (string.IsNullOrEmpty(token)) return null;
            var headerValue = new AuthenticationHeaderValue("Bearer", token);
            _client.DefaultRequestHeaders.Authorization = headerValue;
            return headerValue;
        }
    }
}