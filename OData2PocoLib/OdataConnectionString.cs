// Copyright (c) Mohamed Hassan & Contributors. All rights reserved. See License.md in the project root for license information.


using System.Net;
using OData2Poco.Http;

namespace OData2Poco;

public class OdataConnectionString
{
    private string? _ServiceUrl;

    public OdataConnectionString()
    {
        TlsProtocol = SecurityProtocolType.Tls12;
        Authenticate = AuthenticationType.None;
        Password = new SecuredPassword();
    }
    public string ServiceUrl
    {
        set => _ServiceUrl = value;
        get => _ServiceUrl
               ?? throw new InvalidOperationException("Uninitialized property: " + nameof(ServiceUrl));
    }

    public string? UserName { get; set; }
    public SecuredPassword Password { get; set; }
    public string? Domain { get; set; }
    public string? Proxy { get; set; }
    public string? TokenUrl { get; set; }
    public string? TokenParams { get; set; }
    public string? ParamFile { get; set; }
    public AuthenticationType Authenticate { get; set; }
    public SecurityProtocolType TlsProtocol { get; set; }

    public static OdataConnectionString Create(string url)
    {
        return new OdataConnectionString { ServiceUrl = url };
    }
}

