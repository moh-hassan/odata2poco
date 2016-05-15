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
        //[Test]
        //[TestCase("http://services.odata.org/V4/OData/OData.svc", 11)] //v4
        ////[TestCase("http://services.odata.org/V3/OData/OData.svc", 11)] //v3
        ////expecteCount: count of classes /Entities
        //public void Test1(string url, int expecteCount)
        //{
        //    //1) read metadata from source
        //    var metaDataReader = new MetaDataReader(url);
        //    metaDataReader.LoadMetaData();
        //    //Console.WriteLine(metaDataReader.MetaDataAsString);

        //    //2) create IPocoGenerator using pocoFactory, GeneratePocoList
        //    IPocoGenerator gen = PocoFactory.Create(metaDataReader.MetaDataAsString, url);
        //    var list = gen.GeneratePocoList();
        //    Console.WriteLine(list.Count);

        //    //3) generate text
        //    var classGen = new PocoClassGenerator(gen);

        //    var product = classGen["Product"];
        //    Console.WriteLine(product);
        //    Console.WriteLine(product.ToCsCode());
        //    //var code = classGen.CsClassToString(product,true);
        //    var code = classGen.ToString(); //.CsClassToString(product, true);
        //    Console.WriteLine(code);
        //    Assert.IsNotEmpty(code);
        //    StringAssert.Contains("public class Product", code);

        //}

        [Test]
        [TestCase("http://services.odata.org/V4/OData/OData.svc", 11)] //v4
        [TestCase("http://services.odata.org/V3/OData/OData.svc", 11)] //v3
        [TestCase(@"data\northwindV4.xml", 11)] //filename , countof entities in the model
        [TestCase(@"data\northwindV3.xml", 11)]
        //expecteCount: count of classes /Entities
        public void Test2(string url, int expecteCount)
        {
            //1) read metadata from source
            var gen=  new MetaDataReader(url).Execute();
            //Console.WriteLine(metaDataReader.MetaDataAsString);
            var product = gen["Product"];
            Console.WriteLine(product);
            Console.WriteLine(product.ToCsCode());
            var code = gen.ToString();
            Console.WriteLine(code);
            Assert.IsNotEmpty(code);
            StringAssert.Contains("public class Product", code);

        }
    }
}
