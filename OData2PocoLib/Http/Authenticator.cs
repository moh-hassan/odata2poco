using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using OData2Poco.InfraStructure.Logging;

namespace OData2Poco.Http
{
    internal class Authenticator
    {
        public static ILog Logger = PocoLogger.Default;
        private readonly HttpClient _client;
        public Authenticator(HttpClient client)
        {
            _client = client;
        }

        public async Task Authenticate(OdataConnectionString odataConnString)
        {
            switch (odataConnString.Authenticate)
            {
                case AuthenticationType.None:
                case AuthenticationType.Ntlm:
                case AuthenticationType.Digest:
                    break;

                case AuthenticationType.Basic:
                    Logger.Trace("Authenticating with Basic");
                        Authenticate(odataConnString.UserName, odataConnString.Password);
                    break; ;

                case AuthenticationType.Token:
                    Logger.Trace("Authenticating with Token");
                    //token
                        Authenticate(odataConnString.Password);
                    break;
                case AuthenticationType.Oauth2:
                    Logger.Trace("Authenticating with OAuth2");
                    //OAuth2 
                    if (!string.IsNullOrEmpty(odataConnString.TokenUrl))
                    {
                        var accessToken = await new TokenEndpoint(odataConnString).GetAccessTokenAsync();
                        Authenticate(accessToken);
                    }
                    break;
            }
        }
        private AuthenticationHeaderValue Authenticate(string user, string password)
        {
            //credintial
            if (string.IsNullOrEmpty(user)) return null;
            var token = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{user}:{password}"));
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