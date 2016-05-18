using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using OData2Poco.Extension;

namespace OData2Poco.Tests
{
    [TestFixture]
    class PocoFactoryTest
    {
        const string UrlV4 = "http://services.odata.org/V4/Northwind/Northwind.svc";
        const string UrlV3 = "http://services.odata.org/V3/Northwind/Northwind.svc";

     
        [Test]
        [TestCase(UrlV4, 26)]
        [TestCase(UrlV3, 26)]
        [TestCase(@"data\northwindV4.xml", 11)] //filename , countof entities in the model
        [TestCase(@"data\northwindV3.xml", 11)]
        //expecteCount: count of classes /Entities
        public void Test2(string url, int expecteCount)
        {
            var gen=  new MetaDataReader(url).Generate();
            var product = gen["Product"];
            var code = gen.ToString();
            Assert.IsNotEmpty(code);
            StringAssert.Contains("public class Product", code);

        }
    }
}
