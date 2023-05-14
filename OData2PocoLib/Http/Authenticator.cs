// Copyright (c) Mohamed Hassan & Contributors. All rights reserved. See License.md in the project root for license information.

using System.Net;
using System.Net.Http.Headers;
using System.Text;

namespace OData2Poco.Http;

internal class Authenticator
{
    private readonly CustomHttpClient _customClient;
    public Authenticator(CustomHttpClient customClient)
    {
        _customClient = customClient;
    }

    public async Task Authenticate()
    {
        var ocs = _customClient._odataConnectionString;
        if (ocs.Authenticate == AuthenticationType.None) return;
        var client = _customClient._client;
        var handler = _customClient.handler;
        CredentialCache credentials = new();
        NetworkCredential nc = ocs.ToCredential();
        switch (ocs.Authenticate)
        {
            case AuthenticationType.Basic:
                var token = Convert.ToBase64String(Encoding.UTF8
                    .GetBytes($"{ocs.UserName}:{ocs.ToCredential().Password}"));
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Basic", token);
                break;

            case AuthenticationType.Token:
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", ocs.ToCredential().Password);
                break;

            case AuthenticationType.Oauth2:
                if (!string.IsNullOrEmpty(ocs.TokenUrl))
                {
                    var accessToken = await new TokenEndpoint(ocs).GetAccessTokenAsync();
                    if (string.IsNullOrEmpty(accessToken)) return;
                    var headerValue = new AuthenticationHeaderValue("Bearer", accessToken);
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