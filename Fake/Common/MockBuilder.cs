// Copyright (c) Mohamed Hassan & Contributors. All rights reserved. See License.md in the project root for license information.

using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;
using WireMock.Settings;

namespace OData2Poco.HttpMock;

internal class MockBuilder
{
    private int Port { get; }
    private WireMockServer _server;
    public MockBuilder(int port)
    {
        this.Port = port;
    }
    public void Start()
    {
        var serverSettings = new WireMockServerSettings
        {
            StartAdminInterface = true,
            //  Logger = new WireMockConsoleLogger(),
            Port = Port,
        };
        _server = WireMockServer.Start(serverSettings);
        CreateStubs();
        Mocks.Server = _server;
    }

    public void CreateStubs()
    {
        void BuildService(Site site)
        {
            var url = site.OdataPath;
            Console.WriteLine(url);
            _server
                .Given(Request.Create()
                    .WithPath(url))
                .WithTitle(site.Name)
                .WithDescription(site.Url)
                .RespondWith(Response.Create()
                    .WithBodyFromFile(site.XmlFile)
                    .WithStatusCode(200)
                    .WithHeader("Content-Type", "application/xml"));
        }
        void DefaultService()
        {
            _server
                .Given(Request.Create()
                    .WithPath("/status"))
                .WithTitle("status")
                .RespondWith(Response.Create()
                    .WithStatusCode(200));
        }

        BuildService(Mocks.TripPin);
        BuildService(Mocks.NorthWindV4);
        BuildService(Mocks.NorthWindV3);
        BuildService(Mocks.ODataV3);
        BuildService(Mocks.Books);

        DefaultService();
    }
    public void Stop()
    {
        _server.Stop();
    }

}