// Copyright (c) Mohamed Hassan & Contributors. All rights reserved. See License.md in the project root for license information.

using System.Text;
using OData2Poco;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;

internal sealed class OdataService : IDisposable
{
    private static readonly Lazy<OdataService> s_instance = new(() =>
    new OdataService());
    internal static OdataService Instance => s_instance.Value;
    public static string BaseAddress => Instance._mockServer.Urls[0];
    internal static bool IsStarted => Instance._mockServer.IsStarted;
    public static string Trippin => Instance.TrippinUrl;
    public static string TrippinBasic => Instance.TrippinBasicUrl;
    public static string TrippinBearer => Instance.TrippinBearerUrl;
    public static string Northwind => Instance.NorthwindUrl;
    public static string Northwind4 => Instance.Northwind4Url;
    public static string Northwind3 => Instance.Northwind3Url;
    public static string Northwind2 => Instance.Northwind2Url;

    internal readonly WireMockServer _mockServer;

#if TESTLIB
    private readonly int _port = 5678;
#else
    private readonly int _port = 5679;
#endif
    private bool _disposedValue;

    private OdataService()
    {
        var setting = new WireMock.Settings.WireMockServerSettings
        {
            Port = IsRunningInVisualStudio() ? _port : 0,
            StartAdminInterface = true,
        };
        _mockServer = WireMockServer.Start(setting);
        InitializeWireMockServer();
    }

    public string TrippinUrl { get; private set; }

    public string TrippinBasicUrl { get; private set; }
    public string TrippinBearerUrl { get; private set; }
    public string NorthwindUrl { get; private set; }
    public string Northwind4Url { get; private set; }
    public string Northwind3Url { get; private set; }
    public string Northwind2Url { get; private set; }

    public void ShowWireMockServerInfo()
    {
        var isStarted = IsStarted;
        if (!isStarted)
        {
            throw new OData2PocoException("Failed to start OData service");
        }
        Console.WriteLine("OData service is started");
        Console.WriteLine($"OData service is running on {_mockServer.Urls[0]}");
        Console.WriteLine($"Trippin: {Trippin}");
        Console.WriteLine($"TrippinBasic: {TrippinBasic}");
        Console.WriteLine($"TrippinBearer: {TrippinBearer}");
        Console.WriteLine($"Northwind: {Northwind}");
        Console.WriteLine($"Northwind2: {Northwind2}");
        Console.WriteLine($"Northwind3: {Northwind3}");
        Console.WriteLine($"Northwind4: {Northwind4}");
    }
    private void InitializeWireMockServer()
    {
        TrippinUrl = CreateGzipService("/trippin", TestSample.TripPin4Gzip);
        NorthwindUrl = CreateService("/northwind", TestSample.NorthWindV4);
        Northwind4Url = CreateService("/v4/northwind", TestSample.NorthWindV4);
        Northwind3Url = CreateService("/v3/northwind", TestSample.NorthWindV3);
        Northwind2Url = CreateService("/v2/northwind", TestSample.NorthWindV2);
        TrippinBasicUrl = CreateServiceWithBasicAuth("/trippin/basic", TestSample.TripPin4);
        TrippinBearerUrl = CreateServiceWithBearerTokenAuth("/trippin/bearer", TestSample.TripPin4);
    }


    private string CreateService(string path, string body)
    {
        _mockServer
            .Given(Request.Create().WithPath($"{path}/$metadata").UsingGet())
            .RespondWith(Response.Create()
                .WithStatusCode(200)
                .WithHeader("Content-Type", "application/xml")
                .WithBodyFromFile(body));
        return $"{_mockServer.Urls[0]}{path}";
    }

    private string CreateGzipService(string path, string body)
    {
        _mockServer
            .Given(Request.Create().WithPath($"{path}/$metadata").UsingGet())
            .RespondWith(Response.Create()
                .WithStatusCode(200)
                .WithHeader("Content-Encoding", "gzip")
                .WithBodyFromFile(body));
        return $"{_mockServer.Urls[0]}{path}";
    }

    private string CreateServiceWithBasicAuth(string path, string body, string user = "user", string password = "secret")
    {
        _mockServer
            .Given(Request.Create().WithPath($"{path}/$metadata").UsingGet()
            .WithHeader("Authorization", "Basic " + Convert.ToBase64String(Encoding.UTF8.GetBytes($"{user}:{password}"))))
            .RespondWith(Response.Create()
                .WithStatusCode(200)
                .WithHeader("Content-Type", "application/xml")
                .WithBodyFromFile(body));
        return $"{_mockServer.Urls[0]}{path}";
    }

    private string CreateServiceWithBearerTokenAuth(string path,
        string body,
        string token = "secret_token")
    {
        _mockServer
           .Given(Request.Create().WithPath($"{path}/$metadata").UsingGet()
           .WithHeader("Authorization", $"Bearer {token}"))
           .RespondWith(Response.Create()
               .WithStatusCode(200)
               .WithHeader("Content-Type", "application/xml")
               .WithBodyFromFile(body));
        return $"{_mockServer.Urls[0]}{path}";
    }

    private bool IsRunningInVisualStudio()
    {
        var vs = Environment.GetEnvironmentVariable("VisualStudioVersion");
        var status = !string.IsNullOrEmpty(vs);
        Console.WriteLine($"VisualStudioVersion: {vs}");
        Console.WriteLine($"IsRunningInVisualStudio: {status}");
        return status;
    }
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    private void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                _mockServer.Stop();
                _mockServer.Dispose();
            }

            _disposedValue = true;
        }
    }
}
