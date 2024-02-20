// Copyright (c) Mohamed Hassan & Contributors. All rights reserved. See License.md in the project root for license information.

namespace OData2Poco.Http;

using System.Net;
using System.Net.Http.Headers;

internal class Authenticator
{
    private readonly CustomHttpClient _customClient;

    public Authenticator(CustomHttpClient customClient)
    {
        _customClient = customClient;
    }

    public async Task Authenticate()
    {
        var ocs = _customClient.OdataConnection;
        if (ocs.Authenticate == AuthenticationType.None)
        {
            return;
        }

        var client = _customClient._client;
        var handler = _customClient._httpHandler;
        CredentialCache credentials = [];
        var nc = ocs.Password.GetCredential(ocs.UserName, ocs.Domain);
        switch (ocs.Authenticate)
        {
            case AuthenticationType.Basic:
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Basic", ocs.Password.GetBasicAuth(ocs.UserName));
                break;

            case AuthenticationType.Token:
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", ocs.Password.GetToken());
                break;

            case AuthenticationType.Oauth2:
                if (!string.IsNullOrEmpty(ocs.TokenUrl))
                {
                    var accessToken = await new TokenEndpoint(ocs).GetAccessTokenAsync().ConfigureAwait(false);
                    if (string.IsNullOrEmpty(accessToken))
                    {
                        return;
                    }

                    AuthenticationHeaderValue headerValue = new("Bearer", accessToken);
                    client.DefaultRequestHeaders.Authorization = headerValue;
                }

                break;

            case AuthenticationType.Ntlm:
                credentials.Add(new Uri(ocs.ServiceUrl), "NTLM", nc);
                handler.Credentials = credentials;
                break;

            case AuthenticationType.Digest:
                credentials.Add(new Uri(ocs.ServiceUrl), "Digest", nc);
                handler.Credentials = credentials;
                break;
        }
    }
}
