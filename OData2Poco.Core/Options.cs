// Copyright (c) Mohamed Hassan & Contributors. All rights reserved. See License.md in the project root for license information.

using System.Net;
using CommandLine;
using CommandLine.Text;
using OData2Poco.Http;
// ReSharper disable ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
#pragma warning disable S3267

namespace OData2Poco.CommandLine;

// Options of commandline
public partial class Options
{
    //-----------OdataConnectionString-------------------

    [Option('r', "url", Required = true, HelpText = "URL of OData feed.")]
    public string ServiceUrl { get; set; }

    [Option('u', "user", HelpText = "User name in basic authentication /Or client_id in oauth2.")]
    public string UserName { get; set; }

    [Option('p', "password", HelpText = "password or/token Or access_token /Or client_secret in oauth2.")]
    public SecuredContainer Password { get; set; }
    [Option("domain", HelpText = "Domain in Active Directory.")]
    public string Domain { get; set; }
    [Option("proxy", HelpText = "Http Proxy in the form: 'server:port'.")]
    public string Proxy { get; set; }

    [Option('t', "token-endpoint", HelpText = "OAuth2 Token Endpoint.")]
    public string TokenUrl { get; set; }

    [Option("token-params", HelpText =
        "OAuth2 Token Parameters with key=value separated by Ampersand '&' formated as: 'client_id=xxx&client_secret=xxx&...', no space allowed.")]
    public string TokenParams { get; set; }

    [Option("param-file", Hidden = true, HelpText = "Path to parameter file (json or text format. Postman Environment is supported)")]
    public string ParamFile { get; set; } //v3.1

    [Option('o', "auth", Default = AuthenticationType.None, HelpText = "Authentication type, allowed values: none, basic, token, oauth2.")]
    public AuthenticationType Authenticate { get; set; }
    [Option('H', "http-header", Separator = ';', HelpText = "Http Header as a list of key/value pair separated by ';' e.g. key1=value1;ky2=value2.")]
    public IEnumerable<string> HttpHeader { get; set; }

   
    /// <summary>
    /// Skip Certification Check. This switch is only intended to be used for hosts using a self-signed certificate for testing purposes. This is not recommended in production environment
    /// </summary>
    [Option('S', "skip-check", HelpText = "Skips certificate validation checks that include all validations such as trusted root authority, expiration, ... .")]
    public bool SkipCertificationCheck { get; set; }

    [Option("ssl", HelpText = "Sets the SSL/TSL protocols. Allowed values: tls, tls11, tls12,tls13. Multiple values separated by comma are allowed,e.g, tls11,tls12.")]
    public SecurityProtocolType TlsProtocol { get; set; }

    //-----------pocoSetting-----------------

    [Option('f', "filename", HelpText = "filename to save generated c# code.")]
    public string CodeFilename { get; set; }

    [Option('b', "nullable", HelpText = "Add nullable data types")]
    public bool AddNullableDataType { get; set; }

    [Option('n', "navigation", HelpText = "Add navigation properties")]
    public bool AddNavigation { get; set; }

    [Option('e', "eager", HelpText = "Add non virtual navigation Properties for Eager Loading")]
    public bool AddEager { get; set; }

    [Option("lang", Default = Language.CS, HelpText = "Type cs for CSharp, ts for typescript")]
    public Language Lang { get; set; } //v3

    [Option('i', "inherit", HelpText = "for class inheritance from  BaseClass and/or interfaces")]
    public string Inherit { get; set; }

    [Option('m', "namespace", HelpText = "A namespace prefix for the OData namespace")]
    public string NamespacePrefix { get; set; }

    [Option('c', "case", Default = CaseEnum.None,
        HelpText = "Convert Class Property  case. Allowed values are: pas, camel, kebab, snake or none")]
    public CaseEnum NameCase { get; set; }

    /// <summary>
    /// Convert case of Entity Name`
    /// </summary>
    [Option('C', "entity-case", Default = CaseEnum.None,
        HelpText = "Type pas or camel to Convert Entity Name to PascalCase or CamelCase")]
    public CaseEnum EntityNameCase { get; set; }

    [Option("name-map", HelpText = "A JSON file to map class and property names.")]
    public string NameMapFile { get; set; }

    // NOT AN OPTION! A a place holder because we try to parse the file
    // at the time of option parsing so that if someone is using this
    // programatically, they can directly setup the rename map how they
    // see fit.
    public RenameMap RenameMap { get; set; }

    [Option('a', "attribute",
        HelpText = "Attributes, Allowed values: key, req, json, json3, tab, dm, proto, db, display")]
    public IEnumerable<string> Attributes { get; set; }

    [Option('g', "gen-project", HelpText = "Generate a class library (.Net Stnadard) project csproj/vbproj.")]
    public bool GenerateProject { get; set; }

    [Option('w', "show-warning", HelpText = "Show warning messages of renaming properties/classes whose name is a reserved keyword.")]
    public bool ShowWarning { get; set; }

    /// <summary>
    /// Filter the Entities by FullName, case insensitive. Use comma delemeted list of entity names. Name may include the special characters * and ?. The char *  represents a string of characters and ? match any single char.
    /// </summary>
    [Option("include",
        HelpText = "Filter the Entities by FullName, case insensitive. Use space delemeted list of entity names. Name may include the special characters * and ?. The char *  represents a string of characters and ? match any single char.")]
    public IEnumerable<string> Include { get; set; }

    //feature #41
    [Option("read-write", HelpText = "All properties are read/write")]
    public bool ReadWrite { get; set; }
    /// <summary>
    /// Allow Nulable Reference Type in c#8, feature #43
    /// </summary>
    [Option('B', "enable-nullable-reference", HelpText = "Enable nullable for all reference types including option -b for primitive  types by adding ? to types")]
    public bool EnableNullableReferenceTypes { get; set; }
    [Option('I', "init-only", HelpText = "Allow setter of class property to be 'init' instead of 'set' (c# 9 feature)")]
    public bool InitOnly { get; set; }

    [Option('O', "open-api", Hidden = true, HelpText = "Path of file .json /.yml for OpenApi or Swagger Specification version 3.")]
    public string OpenApiFileName { get; set; }
    [Option('G', "generator-type", HelpText = "Generator Type. Allowd values: class or interface or record")]
    public GeneratorType GeneratorType { get; set; }

    [Option("multi-files", HelpText = "Generate multi files.")]
    public bool MultiFiles { get; set; }
    [Option("full-name", HelpText = "Use fullname prfixed by namespace as a name for Poco Class.")]
    public bool UseFullName { get; set; }

    //----------------------Action options
    [Option('x', "metafile", HelpText = "Xml filename to save metadata.")]
    public string MetaFilename { get; set; }

    [Option('v', "verbose", HelpText = "Prints C# code to the standard output.")]
    public bool Verbose { get; set; }

    [Option('h', "header", HelpText = "print  http header of the service to the standard output.")]
    public bool Header { get; set; }

    [Option('l', "list", HelpText = "List POCO classes to standard output.")]
    public bool ListPoco { get; set; }
    public List<string> Errors { get; set; }

    public Options()
    {
        Attributes = new List<string>();
        Errors = new List<string>();
        //set default
        Authenticate = AuthenticationType.None;
        NameCase = CaseEnum.None;
        Lang = Language.CS;
        Include = new List<string>();
        Password = SecuredContainer.Empty;
        HttpHeader = new List<string>();
        //TlsProtocol = new List<SecurityProtocolType>(){ }
        //  Tls = new List<SecurityProtocolType>();
    }

#if NETCOREAPP
    [Usage(ApplicationAlias = "dotnet-o2pgen")]
#else
    [Usage(ApplicationAlias = "o2pgen")]
#endif
    public static IEnumerable<Example> Examples
    {
        get
        {
            yield return new Example("Default setting",
                new Options { ServiceUrl = "http://services.odata.org/V4/OData/OData.svc" });
            yield return new Example("Add json, key Attributes with camel case and nullable types", new Options
            {
                ServiceUrl = "http://services.odata.org/V4/OData/OData.svc",
                Attributes = new List<string> { "json", "key" },
                NameCase = CaseEnum.Camel,
                AddNullableDataType = true
            });
        }
    }

}