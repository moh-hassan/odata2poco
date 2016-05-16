//#define local
using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Xml;
using NUnit.Framework;
//using System.Net.Http;


namespace OData2Poco.Tests
{
      

    [TestFixture]
    public partial class MetaDataReaderTests
    {
        const string UrlV4 = "http://services.odata.org/V4/Northwind/Northwind.svc";
        const string UrlV3 = "http://services.odata.org/V3/Northwind/Northwind.svc";
      

        [Test]
        [TestCase(UrlV4)]
        [TestCase(UrlV3)]
        [TestCase(@"data\northwindV4.xml")] //filename 
        [TestCase(@"data\northwindV3.xml")]
        public void ReadMetaDataAsStringTest(string url)
        {
            var metaDataReader = new MetaDataReader(url);
            var meta = metaDataReader.MetaDataAsString;
            Assert.IsNotEmpty(meta);
            var meta2 = metaDataReader.MetaDataAsString;
            Assert.IsNotEmpty(meta2);
        }

        [Test]
        [TestCase(UrlV4,"4.0")]
        [TestCase(UrlV3,"1.0")]
        [TestCase(@"data\northwindV4.xml", "4.0")] //filename 
        [TestCase(@"data\northwindV3.xml", "1.0")]
        public void ReadMetaDataVersionTest(string url, string serviceVersion)
        {
            var metaDataReader = new MetaDataReader(url);
            var version = metaDataReader.MetaDataVersion;
            //Console.WriteLine(version);
            Assert.AreEqual(version, serviceVersion);
            //repeat operation
            var version2 = metaDataReader.MetaDataVersion;
            //Console.WriteLine(version2);
            Assert.AreEqual(version2, serviceVersion);
        }

        [Test]
        [TestCase(UrlV4, 26)]
        [TestCase(UrlV3, 26)]
        [TestCase(@"data\northwindV4.xml", 11)] //filename , countof entities in the model
        [TestCase(@"data\northwindV3.xml", 11)]
        //expecteCount: count of classes /Entities
        public void GeneratePocoFromHttporFileExecuteTest(string url, int expecteCount)
        {
            var metaDataReader = new MetaDataReader(url);
            var code = metaDataReader.Execute().ToString();
            //Debug.WriteLine(code);
            //var code = gen.ToString();
            Assert.IsNotEmpty(code);
            StringAssert.Contains("public class Product", code);
            Assert.AreEqual(metaDataReader.Generator.ClassDictionary.Count, expecteCount);
        }

        [Test]
        [TestCase(UrlV4, 26)]
        [TestCase(UrlV3, 26)]
        [TestCase(@"data\northwindV4.xml", 11)] //filename , countof entities in the model
        [TestCase(@"data\northwindV3.xml", 11)]
        //expecteCount: count of classes /Entities
        public void GeneratePocoFromHttporFileExecuteSettingTest(string url, int expecteCount)
        {
            var metaDataReader = new MetaDataReader(url);

            var code = metaDataReader.Execute(new PocoSetting
            {
                AddKeyAttribute = true,
                AddTableAttribute = true,
                AddRequiredAttribute = true,
                AddNullableDataType = true
            }).ToString();
           
            Assert.IsNotEmpty(code);
            StringAssert.Contains("public class Product", code);
            Assert.AreEqual(metaDataReader.Generator.ClassDictionary.Count, expecteCount);
          }

        [Test]
        [TestCase(UrlV4, 26)]
        [TestCase(UrlV3, 26)]
        [TestCase(@"data\northwindV4.xml", 11)] //filename , countof entities in the model
        [TestCase(@"data\northwindV3.xml", 11)]
        //expecteCount: count of classes /Entities
        public void GeneratePocoFromHttporFileGeneratorTest(string url, int expecteCount)
        {
            var metaDataReader = new MetaDataReader(url);
            var gen = metaDataReader.Generator; //.Execute();
            var code = gen.ToString();
            Assert.IsNotEmpty(code);
            StringAssert.Contains("public class Product", code);
            Assert.AreEqual(metaDataReader.Generator.ClassDictionary.Count, expecteCount);

        }

        [Test]
        [TestCase("http://not_valid_url.com")] //not valid url
        [TestCase("http://www.google.com")] //not odata support
        public void GeneratePocoInvalidODataOrUrlTest(string url)
        {
            var code = "";
            var metaDataReader = new MetaDataReader(url);
            Assert.Throws<WebException>(() => code = metaDataReader.GeneratePoco());
            Assert.IsEmpty(code);
        }

#if local
        //test secured servers
        [Test]
        public void GeneratePocoWithValidAccountTes()
        {
            string url = "http://localhost/odata2/api/northwind";
            var metaDataReader = new MetaDataReader(url,"user","password");
            var code = metaDataReader.GeneratePoco();
            Assert.IsNotEmpty(code);

        }


        [Test]
        public void GeneratePocoWithInValidAccountTest()
        {
            var msg = "The remote server returned an error: (401) Unauthorized";
            string url = "http://alocalhost/odata2/api/northwind";
            MetaDataReader metaDataReader = new MetaDataReader(url, "user_invalid", "password");
            var ex = Assert.Throws<WebException>(() => metaDataReader.GeneratePoco());
            StringAssert.Contains(msg, ex.Message);

        }
#endif

        [Test]
        public void GeneratePocoFromFileNotExistTest()
        {
            string code = "";
            string url = "file_not_exist";
            MetaDataReader metaDataReader = new MetaDataReader(url);
            Assert.Throws<FileNotFoundException>(() => code = metaDataReader.GeneratePoco());

            Assert.IsEmpty(code);

        }
        [Test]
        public void GeneratePocoFromNotValidXmlFileTest()
        {
            string code = "";
            string url = @"data\invalidxml.xml";
            MetaDataReader metaDataReader = new MetaDataReader(url);
            Assert.Throws<XmlException>(() => code = metaDataReader.GeneratePoco());
            Assert.IsEmpty(code);

        }


    }
}
