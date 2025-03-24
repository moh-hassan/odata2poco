// Copyright (c) Mohamed Hassan & Contributors. All rights reserved. See License.md in the project root for license information.

namespace OData2Poco.Fake;
//using static OData2Poco.Fake.TestCaseSources;

using System.Security.Policy;
using static OData2Poco.Fake.TestSample;
internal class TestCaseSources
{
    //internal static readonly string s_url = OdataService.Trippin;

    internal static IEnumerable<TestCaseData> UrlOdataCases
    {
        get
        {
            yield return new TestCaseData(OdataService.Northwind4, "4.0", 26)
                .SetName("NorthWind Url");
            yield return new TestCaseData(OdataService.Northwind3, "1.0", 26)
                .SetName("NorthWind Url");
        }
    }

    internal static IEnumerable<TestCaseData> FileCases
    {
        get
        {
            yield return new TestCaseData(NorthWindV4, "4.0", 11).SetName("NorthWind V4 file");
            yield return new TestCaseData(NorthWindV3, "1.0", 11).SetName("NorthWind V3 file");
        }
    }

    internal static IEnumerable<TestCaseData> UrlNorthwindCases
    {
        get
        {
            yield return new TestCaseData(OdataService.Northwind, "4.0", 26)
                .SetName("NorthWind 4 Url");
            yield return new TestCaseData(OdataService.Northwind3, "1.0", 26).
                SetName("NorthWind 3 Url");
        }
    }

    public static IEnumerable<TestCaseData> TestCases
    {
        get
        {
            yield return new TestCaseData(NorthWindV4, "swagger.json", "\"openapi\": \"3.0.1\"");
            yield return new TestCaseData(NorthWindV4, "swagger.yml", "openapi: 3.0.1");
            yield return new TestCaseData(
                UrlTripPinService,
                "swaggerPin.json",
                @"""openapi"": ""3.0.1""");
            yield return new TestCaseData(UrlTripPinService, "swaggerPin.yml", "openapi: 3.0.1");
        }
    }

    public static IEnumerable<TestCaseData> TestMockCases
    {
        get
        {
            //url ,version
            yield return new TestCaseData(OdataService.Northwind, "4.0")
                .SetName("Northwind external");
            yield return new TestCaseData(OdataService.Northwind2, "1.0").SetName("Northwind2");
            yield return new TestCaseData(OdataService.Northwind3, "1.0").SetName("Northwind3");
            yield return new TestCaseData(OdataService.Northwind4, "4.0").SetName("Northwind4");
            yield return new TestCaseData(OdataService.Trippin, "4.0").SetName("Trippin");
            yield return new TestCaseData(UrlNorthWindV4, "4.0")
                .SetName("UrlNorthWindV4");
        }
    }

    internal static IEnumerable<TestCaseData> RepeatingArgsTestData
    {
        //args,expected
        get
        {
            //repeating args
            string[] arg1 = ["-r", "http://localhost.com", "-H", "key1=abc", "-H", "key2=abc", "--auth", "token", "-a", "key", "-a", "json"];
            string[] arg2 = ["-r", "http://localhost.com", "-H", "key1=abc", "key2=abc", "--auth", "token", "-a", "key", "json"];
            yield return new TestCaseData(arg1, arg2).SetName("1-repeating args");

            //repeating args with = sign at the start
            string[] arg3 = ["--url=http://localhost.com", "-H", "key1=abc", "-H", "key2=abc", "--auth", "token", "-a", "key", "-a", "json"];
            string[] arg4 = ["--url=http://localhost.com", "-H", "key1=abc", "key2=abc", "--auth", "token", "-a", "key", "json"];
            yield return new TestCaseData(arg3, arg4).SetName("2-repeating args with equal sign at start");

            //repeating args with = sign at the end
            string[] arg5 = ["-H", "key1=abc", "-H", "key2=abc", "--auth", "token", "-a", "key", "-a", "json", "--url", "http://localhost.com"];
            string[] arg6 = ["--url", "http://localhost.com", "-H", "key1=abc", "key2=abc", "--auth", "token", "-a", "key", "json"];
            yield return new TestCaseData(arg5, arg6).SetName("3-repeating args with equal sign at end");

            //Non-repeating args
            string[] arg7 = ["-r", "http://localhost.com", "-H", "key1=abc", "--auth", "token"];
            yield return new TestCaseData(arg7, arg7).SetName("4-non repeating args");

            //empty args
            yield return new TestCaseData(Array.Empty<string>(), Array.Empty<string>())
                .SetName("5-empty args");

            //args with two args
            string[] arg8 = ["-r", "http://localhost.com"];
            yield return new TestCaseData(arg8, arg8).SetName("6-args with two args");

            //args with two args
            string[] arg9 = ["-r", "http://localhost.com", "-H", "key1=123;key2=abc"];
            yield return new TestCaseData(arg9, arg9).SetName("7-args have multi-value  arg");
        }
    }

    public static IEnumerable<TestCaseData> TestCases2
    {
        get
        {
            yield return new TestCaseData($"-r {OdataService.Trippin} -v", 0, "public partial class Airline");
            yield return new TestCaseData($"-r {OdataService.Trippin} -v -Y", 1, "ERROR(S)");
            yield return new TestCaseData($"-r {OdataService.Trippin} -v -G record -I", 0, "public partial record Airline");
        }
    }

    internal static IEnumerable<TestCaseData> TestCases3
    {
        get
        {
            yield return new TestCaseData(false, "Flight", "PublicTransportation");
            yield return new TestCaseData(true,
                "MicrosoftODataSampleServiceModelsTripPinFlight",
                "MicrosoftODataSampleServiceModelsTripPinPublicTransportation");
        }
    }

    //(bool useFullName, string expectedProperties)
    internal static IEnumerable<TestCaseData> PropertyTestCases
    {
        get
        {
            yield return new TestCaseData(false, @"
number
string
string
string
number
Date
Date
string[]
Photo[]
PlanItem[]"
                .Trim());
            yield return new TestCaseData(true, @"
number
string
string
string
number
Date
Date
string[]
MicrosoftODataSampleServiceModelsTripPinPhoto[]
MicrosoftODataSampleServiceModelsTripPinPlanItem[]"
                .Trim());
        }
    }
}
