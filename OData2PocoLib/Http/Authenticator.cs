// Copyright (c) Mohamed Hassan & Contributors. All rights reserved. See License.md in the project root for license information.

using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace OData2Poco.Http;

internal class Authenticator
{
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
                Authenticate(odataConnString.UserName, odataConnString.Password.Password);
                break;

            case AuthenticationType.Token:
                //token
                Authenticate(odataConnString.Password.Password);
                break;
            case AuthenticationType.Oauth2:
                //OAuth2 
                if (!string.IsNullOrEmpty(odataConnString.TokenUrl))
                {
                    var accessToken = await new TokenEndpoint(odataConnString).GetAccessTokenAsync();
                    Authenticate(accessToken);
                }

                break;
        }
    }

    private void Authenticate(string? user, string? password)
    {
        //credintial
        if (string.IsNullOrEmpty(user) || string.IsNullOrEmpty(password)) return;
        var token = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{user}:{password}"));
        var headerValue = new AuthenticationHeaderValue("Basic", token);
        _client.DefaultRequestHeaders.Authorization = headerValue;
    }

    private void Authenticate(string? token)
    {
        //credintial
        if (string.IsNullOrEmpty(token)) return;
        var headerValue = new AuthenticationHeaderValue("Bearer", token);
        _client.DefaultRequestHeaders.Authorization = headerValue;
    }
}