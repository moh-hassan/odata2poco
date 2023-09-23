// Copyright (c) Mohamed Hassan & Contributors. All rights reserved. See License.md in the project root for license information.

using System.IO.Compression;
using OData2Poco.Fake;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;
using WireMock.Settings;

namespace OData2Poco.Tests;

/// <summary>
/// Mock server that generate trippin metadata gzip encoded.
/// To use:  string  url = new GzipMockServer();
/// </summary>
public class GzipMockServer : IDisposable
{
    private readonly WireMockServer _server;
    public string MetaUrl;
    public string BaseUrl;

    public GzipMockServer(string xmlPath)
    {
        string tempFilePath = Path.GetTempFileName();
        string gzipFilePath = Path.ChangeExtension(tempFilePath, ".gz");
        CompressFile(xmlPath, gzipFilePath);
        _server = WireMockServer.Start(new WireMockServerSettings
        {
            Port = 0 //dynamic port
        });
        BaseUrl = $"{_server.Urls[0]}/odata";
        MetaUrl = $"{BaseUrl}/$metadata";
        _server.Given(Request.Create().WithPath("/odata/$metadata").UsingGet())
            .RespondWith(Response.Create()
                .WithHeader("Content-Encoding", "gzip")
                .WithBodyFromFile(gzipFilePath)
            );
    }

    public GzipMockServer() : this(TestSample.TripPin4)
    {
    }

    private void CompressFile(string sourceFilePath, string compressedFilePath)
    {
        using FileStream sourceFile = File.OpenRead(sourceFilePath);
        using FileStream compressedFile = File.Create(compressedFilePath);
        using GZipStream compressionStream = new GZipStream(compressedFile, CompressionMode.Compress);
        sourceFile.CopyTo(compressionStream);
    }

    public static implicit operator string(GzipMockServer server) => server.BaseUrl;

    public void Dispose()
    {
        _server.Stop();
        _server?.Dispose();
    }
}