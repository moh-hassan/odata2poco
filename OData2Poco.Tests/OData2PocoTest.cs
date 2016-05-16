using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace OData2Poco.Tests
{

    [TestFixture]
    class OData2PocoTest
    {
        const string UrlV4 = "http://services.odata.org/V4/Northwind/Northwind.svc";
        const string UrlV3 = "http://services.odata.org/V3/Northwind/Northwind.svc";
      
        [Test]
        public void GenerateTest()
        {
            var code = new O2P()
                .SetUrl("http://services.odata.org/V4/Northwind/Northwind.svc")
                .AddKeyAttribute()
                .AddTableAttribute()
                .Generate();
            Console.WriteLine(code);
        }
        [Test]
        public void GenerateTest3()
        {
            var code = new O2P()
                .SetUrl("http://services.odata.org/V4/Northwind/Northwind.svc")
                .SetUser("user")
                .SetPassword("pw")
                .AddKeyAttribute()
                .AddRequiredAttribute()
                .AddNavigation()
                .AddTableAttribute()
                .Generate();
            Console.WriteLine(code);
        }

        [Test]
        public void GenerateTest2()
        {
            var code = new O2P()
               // .SetUrl("http://services.odata.org/V4/Northwind/Northwind.svc")
                .AddKeyAttribute()
                .AddTableAttribute()
                .Generate();
            Console.WriteLine(code);
        }
    }
}
