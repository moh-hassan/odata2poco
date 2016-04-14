using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;
using OData2Poco;
namespace OData2PocoConsole
{
    class Program
    {
        //Install-Package OData2Poco
        static void Main(string[] args)
        {
            Example1();
            Console.ReadKey();
        }

        static void Example1()
        {
            var url = "http://services.odata.org/V4/OData/OData.svc";
            var code = new O2P(url).Generate();
            Console.WriteLine(code);
        }

        static void Example2()
        {
            var url = "http://services.odata.org/V4/OData/OData.svc";
            var o2p = new O2P(url);
        var    code = o2p.Generate();
           // var version o2p.MetaDataVersion
            Console.WriteLine(code);
        }

    }
}
