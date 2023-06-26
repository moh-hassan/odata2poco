// Copyright (c) Mohamed Hassan & Contributors. All rights reserved. See License.md in the project root for license information.

using WireMock;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;
using WireMock.Settings;

namespace OData2Poco.HttpMock;

internal class MockBuilder
{
    private int _port { get; }
    private WireMockServer _server;
    public MockBuilder(int port)
    {
        this._port = port;
    }
    public void Start()
    {
        var serverSettings = new WireMockServerSettings
        {
            StartAdminInterface = true,
            //  Logger = new WireMockConsoleLogger(),
            Port = _port,
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

    public void Check(string name)
    {
        IMapping aa = _server.Mappings.FirstOrDefault(a => a.Title == name);
        // Console.WriteLine($"{Mocks.Server.LogEntries.Count()}");
        //S.Server.Urls.Dump("urls");
        //S.Server.Url.Dump("url");
        //S.Server.Consumer.Dump("Consumer");
        //foreach (var entry in S.Server.LogEntries)
        //{
        //    entry.Dump($"entry: {entry.RequestMessage.AbsoluteUrl}");
        //}
        //foreach (var entry in S.Server.Mappings)
        //{
        //    entry.Dump($"mapping{entry.Guid}");
        //}
    }
}