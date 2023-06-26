// Copyright (c) Mohamed Hassan & Contributors. All rights reserved. See License.md in the project root for license information.

using OData2Poco.Fake;
using WireMock.Server;

namespace OData2Poco.HttpMock;
#pragma warning disable S3963
internal static class Mocks
{
#if Test_Cli
    public static int Port = 5554;
#else
    public static int Port = 5555;
#endif
    public static string BaseUrl;
    public static Site TripPin { get; }
    public static Site NorthWindV4 { get; }
    public static Site NorthWindV3 { get; }
    public static Site ODataV3 { get; }
    public static Site Books { get; }

    public static WireMockServer Server;

    static Mocks()
    {
        BaseUrl = $"http://localhost:{Port}";
        TripPin = new()
        {
            Name = "trippin",
            Path = "/trippin",
            Url = $@"{BaseUrl}/trippin",
            OdataPath = "/trippin/$metadata",
            OdataUrl = $@"{BaseUrl}/trippin/$metadata",
            XmlFile = TestSample.TripPin4Rw
        };


        NorthWindV4 = new()
        {
            Name = "northwindV4",
            Path = "/northwind/v4",
            Url = $@"{BaseUrl}/northwind/v4",
            OdataPath = "/northwind/v4/$metadata",
            OdataUrl = $@"{BaseUrl}/northwind/v4/$metadata",
            XmlFile = TestSample.NorthWindV4
        };


        NorthWindV3 = new()
        {
            Name = "northwindV3",
            Path = "/northwind/v3",
            Url = $@"{BaseUrl}/northwind/v3",
            OdataPath = "/northwind/v3/$metadata",
            OdataUrl = $@"{BaseUrl}/northwind/v3/$metadata",
            XmlFile = TestSample.NorthWindV3
        };

        ODataV3 = new()
        {
            Name = "odataV3",
            Path = "/odata/v3",
            Url = $@"{BaseUrl}/odata/v3",
            OdataPath = "/odata/v3/$metadata",
            OdataUrl = $@"{BaseUrl}/odata/v3/$metadata",
            XmlFile = TestSample.TripPin4
        };

        Books = new()
        {
            Name = "books",
            Path = "/books",
            Url = $@"{BaseUrl}/books",
            OdataPath = "/books/$metadata",
            OdataUrl = $@"{BaseUrl}/books/$metadata",
            XmlFile = TestSample.TripPin4
        };


    }

}