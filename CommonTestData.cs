using System;
using System.Collections;


 
public static class TestSample
{
    public static object[] UrlCases =
        {
            //url ,version ,noOfClasses
            new object[] {"http://services.odata.org/V4/Northwind/Northwind.svc", "4.0", 26},
            new object[] {"http://services.odata.org/V3/Northwind/Northwind.svc", "1.0", 26}
             //new object[] {" http://services.odata.org/V4/TripPinServiceRW", "4.0", 14}
              
           
        };
    public static object[] FileCases =
        {
            new object[] {@"data\northwindV4.xml","4.0" ,11 },
            new object[] {@"data\northwindV3.xml","1.0" ,11 }  
           // new object[] {@"data\trippinV4.xml","4.0" ,14 } 

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

 