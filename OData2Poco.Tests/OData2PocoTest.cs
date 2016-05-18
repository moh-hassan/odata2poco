using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using NUnit.Framework;
using OData2Poco.Extension;

namespace OData2Poco.Tests
{

    [TestFixture]
    class OData2PocoTest
    {
        const string UrlV4 = "http://services.odata.org/V4/Northwind/Northwind.svc";
        const string UrlV3 = "http://services.odata.org/V3/Northwind/Northwind.svc";

        bool MatchFileContent(string fname, string content)
        {
            var text = File.ReadAllText(fname);
            return text == content;
        }
        [Test]
        [TestCase(UrlV4, "4.0")]
        [TestCase(UrlV3, "1.0")]
        [TestCase(@"data\northwindV4.xml", "4.0")]
        [TestCase(@"data\northwindV3.xml", "1.0")]
        public void ImplicitConversionTest(string url, string version)
        {
            var o2p=  new O2P()
                .SetUrl(url)
                // .Generate();
                .SaveCodeTo("north.cs")
                .SaveMetaDataTo("metanorth.xml");

            //implicit conversion
            MetaDataInfo meta = o2p;
            

            //implicit conversion as string
            Console.WriteLine(o2p);
            Assert.IsTrue(MatchFileContent("north.cs", o2p));
            Assert.IsTrue(MatchFileContent("metanorth.xml", ((MetaDataInfo)o2p).MetaDataAsString));
            Assert.IsTrue(((string)o2p).Contains("public class Product"));

            if (meta.MediaType == Media.Http)
            {
                Assert.Greater(meta.ServiceHeader.Count, 1);
            }
            else
            {
                Assert.AreEqual(meta.ServiceHeader.Count, 0); 
            }
           
            Assert.AreEqual(meta.MetaDataVersion, version);

            Assert.AreEqual(((MetaDataInfo)o2p).MetaDataVersion, version);
          
            Console.WriteLine(meta.ServiceHeader.DicToString());
         
            //implicit conversion test

        }

        [Test]
        [TestCase(UrlV4)]
        [TestCase(UrlV3)]
        [TestCase(@"data\northwindV4.xml")]
        [TestCase(@"data\northwindV3.xml")]
        public void GenerateCodeNoAttributeTest(string url)
        {
            string code = new O2P()
                .SetUrl(url)
                // .Generate();
                .SaveCodeTo("north.cs")
                .SaveMetaDataTo();

            var code2 = File.ReadAllText("north.cs");
            Console.WriteLine(code2);
            Assert.IsTrue(code.Contains("public class Product"));
            Assert.IsTrue(code2.Contains("public class Product"));
        }

        [Test]
        [TestCase(UrlV4)]
        [TestCase(UrlV3)]
        [TestCase(@"data\northwindV4.xml")]
        [TestCase(@"data\northwindV3.xml")]
        public void GenerateCodeWithAttributeTest(string url)
        {
            string code = new O2P()
                .SetUrl(url)
                // .BasicAuthenticate("user","pw")
                .AddKeyAttribute()
                .AddRequiredAttribute()
                .AddNavigation()
                .AddTableAttribute()
                .Generate();
            Console.WriteLine(code);
            Assert.IsTrue(code.Contains("public class Product"));
            Assert.IsTrue(code.Contains("[Key]"));
            Assert.IsTrue(code.Contains("[Required]"));
            Assert.IsTrue(code.Contains("[Table(\"Products\")]"));
        }

        [Test]
        [ExpectedException(typeof(NullReferenceException), ExpectedMessage = "Url is empty")]
        public void GenerateWithNoUrlTest()
        {
            //System.Exception : Url is empty
            string code = new O2P()
                // .SetUrl("http://services.odata.org/V4/Northwind/Northwind.svc")
               .AddKeyAttribute()
               .AddTableAttribute()
               .Generate();

            Assert.IsNull(code);


        }
    }
}
