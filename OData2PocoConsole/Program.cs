using System;
using System.IO;
using System.Threading.Tasks;
using OData2Poco;
//using O2P = OData2Poco.O2P;

namespace OData2PocoConsole
{
    class Program
    {
        //Install-Package OData2Poco
        static void Main(string[] args)
        {
            try
            {
                    var s =   Example1().Result;
                    Console.WriteLine(s);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception {0}",ex.Message); //.FullExceptionMessage());
            }
            Console.ReadKey();
           
           
        }

        //from url
        static async Task<string> Example1()
        {
            Console.WriteLine("example 1: loading from url with setting");
            PocoSetting setting = new PocoSetting
            {
                AddNullableDataType = true,
                AddKeyAttribute = true,
                AddTableAttribute = true,
                AddRequiredAttribute = true,
                AddNavigation = true
            };
            var url = "http://services.odata.org/V4/OData/OData.svc";
            var o2p = new O2P( setting);
            var code = await o2p.GenerateAsync(new Uri(url));
          //  Console.WriteLine(code);
            return code;
        }

        
        //load from xml
        static async Task<string> Example2()
        {
            Console.WriteLine("example 2: loading from xml");
            PocoSetting setting = new PocoSetting
            {
                AddNullableDataType = true,
                AddKeyAttribute = true,
                AddTableAttribute = true,
                AddRequiredAttribute = true,
                AddNavigation = true
            };
           // var url = "http://services.odata.org/V4/OData/OData.svc";
            string xml = File.ReadAllText(@"data\northwindV4.xml");
            var o2p = new O2P(setting);
            
           // o2p.SetMetadataSource(xml);
           // var code = await o2p.GenerateAsync();
            var code =  o2p.Generate(xml);
            Console.WriteLine(code);
            return code;
        }


    }
}
