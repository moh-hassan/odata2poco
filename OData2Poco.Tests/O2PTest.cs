using System;
using System.IO;
using System.Threading.Tasks;
using NUnit.Framework;
using OData2Poco.Api;

namespace OData2Poco.Tests
{

    [TestFixture]
    class O2PTest
    {
        const string UrlV4 = "http://services.odata.org/V4/Northwind/Northwind.svc";
        const string UrlV3 = "http://services.odata.org/V3/Northwind/Northwind.svc";
      

        [Test]
        [TestCase(UrlV4)]
        [TestCase(UrlV3)]
      
        public async Task GenerateDefaultFromHttpTest(string url)
        {
           
            var o2p = new O2P();
            var code = await o2p.GenerateAsync(new Uri(url));
            Assert.IsTrue(code.Contains("public class Product"));
        }

        [Test]
        [TestCase(@"data\northwindV4.xml")]
        [TestCase(@"data\northwindV3.xml")]
        public   void  GenerateDefaultFromXmlFilesTest(string file)
        {
            // var o2p = new O2P()
            var o2p = new O2P();
            var xml = File.ReadAllText(file);
            var code =   o2p.Generate(xml);

            Assert.IsTrue(code.Contains("public class Product"));
        }

    
    }
}
