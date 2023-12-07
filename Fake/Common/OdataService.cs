// Copyright (c) Mohamed Hassan & Contributors. All rights reserved. See License.md in the project root for license information.

using OData2Poco.Fake;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;

public sealed class OdataService : IDisposable
{
    private static readonly Lazy<OdataService> lazy = new(() => new OdataService());
    internal static OdataService Instance => lazy.Value;
    private readonly WireMockServer _mockServer;
    internal static bool IsStarted => Instance._mockServer.IsStarted;
    public static string Trippin => $"{Instance._mockServer.Urls[0]}/trippin";
    public static string Northwind => $"{Instance._mockServer.Urls[0]}/northwind";
    private bool disposedValue;
    private OdataService()
    {
        _mockServer = WireMockServer.Start();
        _mockServer
            .Given(Request.Create().WithPath("/trippin/$metadata"))
            .RespondWith(Response.Create()
                .WithStatusCode(200)
                .WithHeader("Content-Encoding", "gzip")
                .WithBodyFromFile(TestSample.TripPin4Gzip));

        _mockServer
            .Given(Request.Create().WithPath("/northwind/$metadata"))
            .RespondWith(Response.Create()
                .WithStatusCode(200)
                .WithBodyFromFile(TestSample.NorthWindV4));
    }
    private void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                _mockServer.Stop();
                _mockServer.Dispose();
            }
            disposedValue = true;
        }
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}

