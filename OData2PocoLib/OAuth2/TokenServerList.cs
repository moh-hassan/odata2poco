using System.Collections.Generic;

namespace OData2Poco.OAuth2
{
    class TokenServerList
    {
        static readonly Dictionary<string, string> TokenServers = new Dictionary<string, string>
        {
            ["google"] = "https://accounts.google.com/o/oauth2/token",
        };
    }
}
