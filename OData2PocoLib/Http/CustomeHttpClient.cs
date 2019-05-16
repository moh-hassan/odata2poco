using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using OData2Poco.Extensions;
using OData2Poco.InfraStructure.Logging;

namespace OData2Poco.Http
{
    internal class CustomeHttpClient : IDisposable
    {
        public static ILog Logger = PocoLogger.Default;
        readonly OdataConnectionString _odataConnectionString;
        readonly DelegatingHandler _delegatingHandler;
        public Uri ServiceUri { get; set; }
        public HttpResponseMessage Response;
        private HttpClient _client;
        public CustomeHttpClient(OdataConnectionString odataConnectionString)
        {
            _odataConnectionString = odataConnectionString;
            ServiceUri = new Uri(_odataConnectionString.ServiceUrl);
        }
        public CustomeHttpClient(OdataConnectionString odataConnectionString, DelegatingHandler dh)
            : this(odataConnectionString)
        {
            _delegatingHandler = dh;
        }
        private async Task SetHttpClient()
        {

            //UseDefaultCredentials for NTLM support in windows
            var handler = new HttpClientHandler
            {
                UseDefaultCredentials = true,
            };
            if (_delegatingHandler != null)
            {
                Logger.Trace($"Attaching Delegating Handler");
                _delegatingHandler.InnerHandler = handler;
                _client = new HttpClient(_delegatingHandler);
            }
            else
                _client = new HttpClient(handler);
            _client.Timeout = Timeout.InfiniteTimeSpan;
            _client.DefaultRequestHeaders.Accept.Clear();
            _client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json; charset=utf-8");
            var agent = "OData2Poco";
            _client.DefaultRequestHeaders.Add("User-Agent", agent);

            CredentialCache credentials = new CredentialCache();
            if (_odataConnectionString.Authenticate == AuthenticationType.None) return;

            switch (_odataConnectionString.Authenticate)
            {
                case AuthenticationType.Ntlm:
                    Logger.Trace("Authenticating with NTLM");
                    credentials.Add(ServiceUri, "NTLM",
                        new NetworkCredential(_odataConnectionString.UserName, _odataConnectionString.Password, _odataConnectionString.Domain));
                    handler.Credentials = credentials;
                    break;
                case AuthenticationType.Digest:
                    Logger.Trace("Authenticating with Digest");
                    credentials.Add(ServiceUri, "Digest",
                        new NetworkCredential(_odataConnectionString.UserName, _odataConnectionString.Password, _odataConnectionString.Domain));
                    handler.Credentials = credentials;
                    break;
            }


            if (!string.IsNullOrEmpty(_odataConnectionString.Proxy))
            {
                Logger.Trace($"Using Proxy: '{_odataConnectionString.Proxy}'");
                handler.UseProxy = true;
                handler.Proxy = new WebProxy(_odataConnectionString.Proxy);
            }



            if (_odataConnectionString.Authenticate == AuthenticationType.Basic ||
                _odataConnectionString.Authenticate == AuthenticationType.Token ||
                _odataConnectionString.Authenticate == AuthenticationType.Oauth2)
            {
                Authenticator auth = new Authenticator(_client);
                //authenticate
                await auth.Authenticate(_odataConnectionString);
            }

        }

        internal async Task<string> ReadMetaDataAsync()
        {
            if (_odataConnectionString.ServiceUrl.StartsWith("https"))
                // to avoid the Error Message://An error occurred while sending the request.-->
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12
                                                       | SecurityProtocolType.Tls11
                                                       | SecurityProtocolType.Tls;

            await SetHttpClient();
            string url = ServiceUri.AbsoluteUri.TrimEnd('/') + "/$metadata";

            try
            {
                Response = await _client.GetAsync(url).ConfigureAwait(false);
                Response.EnsureSuccessStatusCode();
                var content = await Response.Content.ReadAsStringAsync().ConfigureAwait(false);
                return content;
            }
            catch (Exception e)
            {
                Logger.Error($"Error in reading: {_odataConnectionString.ServiceUrl}");
                Logger.Error(e.FullExceptionMessage());
                throw;
            }
        }

        public void Dispose()
        {
            _delegatingHandler?.Dispose();
            Response?.Dispose();
            _client?.Dispose();
        }
    }

}
