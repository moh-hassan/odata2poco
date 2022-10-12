// Copyright (c) Mohamed Hassan & Contributors. All rights reserved. See License.md in the project root for license information.

using System.Reflection;
namespace OData2Poco.Fake;
public static class TestSample
{
    public static string BaseDirectory
    {
        get
        {
            var assembly = Assembly.GetExecutingAssembly();
            var codebase = new Uri(assembly.Location);
            var path = codebase.LocalPath;
            return Path.GetDirectoryName(path);
        }
    }

    private static readonly string RelativeFakeFolder = Path.Combine("..", "..", "..", "..", "Fake"); //. /release/bin/project/soln

    public static string FakeFolder => Path.GetFullPath(Path.Combine(BaseDirectory, RelativeFakeFolder));

    public static string NorthWindV4 => GetFullPath("northwindV4.xml");
    public static string NorthWindV3 => GetFullPath("northwindV3.xml");
    public static string TripPin4Flag => GetFullPath("trippinV4Flags.xml");
    public static string TripPin4 => GetFullPath("trippinV4.xml");
    public static string TripPin4Rw => GetFullPath("trippin4rw.xml");
    public const string UrlNorthWindV4 = "https://services.odata.org/V4/Northwind/Northwind.svc";
    public const string UrlNorthWindV3 = "https://services.odata.org/V3/Northwind/Northwind.svc";
    public const string UrlNorthWindV2 = "https://services.odata.org/V2/Northwind/Northwind.svc";
    public const string UrlOdataV4 = "https://services.odata.org/V4/OData/OData.svc";
    public const string UrlOdataV3 = "https://services.odata.org/V3/OData/OData.svc";

    public const string UrlTripPinService = "https://services.odata.org/TripPinRESTierService";
    public static string MultiSchemaV3 => GetFullPath("odata-multischema-v3.xml");
    public static string SampleWebApiV4 => GetFullPath("sample-webapi-V4.xml");
    public static string SampleWebApiInvalidV4 => GetFullPath("sample-webapi-Invalid-V4.xml");
    public static string SampleWebApiInvalidV3 => GetFullPath("sample-webapi-Invalid-V3.xml");
    //parameter file
    public static string Param1 => GetFullPath("param1.txt");
    public static string PostmanParams => GetFullPath("postman.json");

    #region Name Mapping
    public static string RenameMap => GetFullPath("rename_map.json");
    public static string RenameMap2 => GetFullPath("rename_map2.json");
    public static string RenameMap3 => GetFullPath("rename_map3.json");
    #endregion
    public static string GetFullPath(string relative)
    {
        var path = Path.GetFullPath(Path.Combine(BaseDirectory, RelativeFakeFolder, relative));
        return path;
    }

    #region Test Cases


    public static object[] UrlNorthwindCases =
        {
            //url ,version ,noOfClasses
            new object[] { UrlNorthWindV4, "4.0", 26},
            new object[] { UrlNorthWindV3, "1.0", 26}
        };
    public static object[] UrlOdatadCases =
    {
        //url ,version ,noOfClasses
        new object[] { UrlOdataV4, "4.0", 26},
        new object[] { UrlOdataV3, "1.0", 26}
    };

    public static object[] FileCases =
        {
            new object[] { NorthWindV4, "4.0" ,11 },
            new object[] { NorthWindV3, "1.0" ,11 }

        };

    #endregion

    #region Url
    public static object[] UrlLocalCases =
        {
            new object[] {"http://asd-pc/odata/api/northwind", "4.0", 26}

        };

    public static object[] UrlLocalSecuredCases =
        {
            new object[] {"http://asd-pc/odata2/api/northwind", "4.0", 26}

        };
    #endregion
}

