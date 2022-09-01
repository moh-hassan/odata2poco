#nullable disable
using System.Net;
using OData2Poco.Http;

namespace OData2Poco
{
    public class OdataConnectionString
    {
        public string ServiceUrl { get; set; }  
        public string UserName { get; set; }  
        public string Password { get; set; }  
        public string Domain { get; set; } 
        public string Proxy { get; set; }  
        public string TokenUrl { get; set; }  
        public string TokenParams { get; set; }  
        public string ParamFile { get; set; }  
        public AuthenticationType Authenticate { get; set; }
        public SecurityProtocolType TlsProtocol { get; set; } 

        public OdataConnectionString()
        {
            TlsProtocol = SecurityProtocolType.Tls12;
            Authenticate = AuthenticationType.None;
        }
    }

}

#nullable restore
