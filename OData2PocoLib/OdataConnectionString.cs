// Copyright (c) Mohamed Hassan & Contributors. All rights reserved. See License.md in the project root for license information.

namespace OData2Poco;

using System.Net;
using Http;
using Security;

public class OdataConnectionString
{
    private string? _serviceUrl;

    public string ServiceUrl
    {
        get => _serviceUrl
               ?? throw new InvalidOperationException("Uninitialized property: " + nameof(ServiceUrl));
        set => _serviceUrl = value;
    }

    public string? UserName { get; set; }
    public SecurityContainer Password { get; set; } = SecurityContainer.Empty;
    public string? Domain { get; set; }
    public string? Proxy { get; set; }
    public string? ProxyUser { get; set; }
    public string? TokenUrl { get; set; }
    public string? TokenParams { get; set; }
    public string? ParamFile { get; set; }
    public AuthenticationType Authenticate { get; set; } = AuthenticationType.None;
    public SecurityProtocolType TlsProtocol { get; set; }
    public IEnumerable<string>? HttpHeader { get; set; }
    public bool SkipCertificationCheck { get; set; }
    public string? LogInUrl { get; set; }
    public static OdataConnectionString Create(string url)
    {
        return new OdataConnectionString
        {
            ServiceUrl = url,
        };
    }
}
