﻿// Copyright (c) Mohamed Hassan & Contributors. All rights reserved. See License.md in the project root for license information.

namespace OData2Poco.Fake.Common;

using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;

public sealed class OdataService : IDisposable
{
    private static readonly Lazy<OdataService> s_lazy = new(() => new OdataService());
    private readonly WireMockServer _mockServer;
    private bool _disposedValue;

    private OdataService()
    {
        _mockServer = WireMockServer.Start(12399);
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

    public static string Trippin => $"{Instance._mockServer.Urls[0]}/trippin";

    public static string Northwind => $"{Instance._mockServer.Urls[0]}/northwind";

    internal static OdataService Instance => s_lazy.Value;

    internal static bool IsStarted => Instance._mockServer.IsStarted;

    public void Dispose()
    {
        Dispose(true);
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
