// Copyright (c) Mohamed Hassan & Contributors. All rights reserved. See License.md in the project root for license information.


using System.Net;
using System.Text;
using OData2Poco.Extensions;
using OData2Poco.Http;

namespace OData2Poco;

public class OdataConnectionString
{
    private string? _serviceUrl;
    public OdataConnectionString()
    {
        Authenticate = AuthenticationType.None;
        Password = SecurityContainer.Empty;
    }
    public string ServiceUrl
    {
        set => _serviceUrl = value;
        get => _serviceUrl
               ?? throw new InvalidOperationException("Uninitialized property: " + nameof(ServiceUrl));
    }
    public string? UserName { get; set; }
    public SecurityContainer Password { get; set; }
    public string? Domain { get; set; }
    public string? Proxy { get; set; }
    public string? TokenUrl { get; set; }
    public string? TokenParams { get; set; }
    public string? ParamFile { get; set; }
    public AuthenticationType Authenticate { get; set; }
    public SecurityProtocolType TlsProtocol { get; set; }
    public IEnumerable<string>? HttpHeader { get; set; }
    public bool SkipCertificationCheck { get; set; }

    public static OdataConnectionString Create(string url)
    {
        return new OdataConnectionString
        {
            ServiceUrl = url,
        };
    }
}

