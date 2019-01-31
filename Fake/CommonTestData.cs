public static class TestSample
{
    private static string Root = @"..\..\..\..\fake";
    //net45-bin-debug-project-fake
    public static string NorthWindV4 => $"{Root}\\northwindV4.xml";
    public static string NorthWindV3 = @"..\..\..\..\fake\northwindV3.xml";
    public static string TripPin4Flag = @"..\..\..\..\fake\trippinV4Flags.xml";
    public static string UrlNorthWindV4 = "http://services.odata.org/V4/Northwind/Northwind.svc";
    public const string UrlNorthWindV3 = "http://services.odata.org/V3/Northwind/Northwind.svc";
    public const string UrlTripPinService = "http://services.odata.org/V4/TripPinServiceRW";
    //set url = http://services.odata.org/V4/OData/OData.svc
    public static object[] UrlCases =
        {
            //url ,version ,noOfClasses
            new object[] { UrlNorthWindV4, "4.0", 26},
            new object[] { UrlNorthWindV3, "1.0", 26}
        };


    public static object[] FileCases =
        {
            new object[] { NorthWindV4, "4.0" ,11 },
            new object[] { NorthWindV3, "1.0" ,11 }  

        };

    public static object[] UrlLocalCases =
        {
            new object[] {"http://asd-pc/odata/api/northwind", "4.0", 26}
           
        };

    public static object[] UrlLocalSecuredCases =
        {
            new object[] {"http://asd-pc/odata2/api/northwind", "4.0", 26}
           
        };
}

 