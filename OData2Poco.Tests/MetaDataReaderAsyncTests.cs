//#define local
using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Xml;
using NUnit.Framework;
using OData2Poco.V4;

//using System.Net.Http;


namespace OData2Poco.Tests
{

    [TestFixture]
    public partial class MetaDataReaderAsyncTests
    {

        //***********************http test*****************************
        [Test]
        
        [TestCase("http://services.odata.org/V4/OData/OData.svc", 11)] //v4
        [TestCase("http://services.odata.org/V3/OData/OData.svc", 11)] //v3
        //expectedCount: number of generated classes
        public void GeneratePocoFromHttpTest(string url, int expecteCount)
        {
            var metaDataReader = new MetaDataReader(url);
            var code = metaDataReader.GeneratePoco();
            Assert.IsNotEmpty(code);
            StringAssert.Contains("public class Product", code);
            StringAssert.Contains("public class FeaturedProduct", code);
            Assert.AreEqual(metaDataReader.ClassList.Count, expecteCount);
        }

        [Test]
        [TestCase("http://not_valid_url.com")] //not valid url
        [TestCase("http://www.google.com")] //not odata support
        public void GeneratePocoInvalidODataOrUrlTest(string url)
        {
            var code = "";
            var metaDataReader = new MetaDataReader(url);
            Assert.Throws<WebException>(async () => code = await metaDataReader.GeneratePocoAsync());
            Assert.IsEmpty(code);
        }

#if local
        //test secured servers
        [Test]
        public async void GeneratePocoWithValidAccountTes()
        {
            string url = "http://localhost/odata2/api/northwind";
            var metaDataReader = new MetaDataReader(url, "user", "password");
            var code = await metaDataReader.GeneratePocoAsync();
            Assert.IsNotEmpty(code);

        }


        [Test]
        public void GeneratePocoWithInValidAccountTest()
        {
            var msg = "The remote server returned an error: (401) Unauthorized";
            string url = "http://localhost/odata2/api/northwind";
            MetaDataReader metaDataReader = new MetaDataReader(url, "user_invalid", "password");
            var ex = Assert.Throws<WebException>(async () => await metaDataReader.GeneratePocoAsync());
            StringAssert.Contains(msg, ex.Message);

        }
#endif

        //********************* xml File test **************************************

        [Test]
        public async void GeneratePocoFromXmlFileV4Test()
        {
            string url = @"data\northwindV4.xml";
            MetaDataReader metaDataReader = new MetaDataReader(url);
            metaDataReader.LoadMetaData();
            Assert.IsNotNull(await metaDataReader.GeneratePocoAsync());
            var count = metaDataReader.ClassList.Count; 
            Assert.AreEqual(count, 11);
        }


        [Test]
        public async void GeneratePocoFromXmlFileV3Test()
        {
            string url = @"data\northwindV3.xml";
            MetaDataReader metaDataReader = new MetaDataReader(url);
            var code = await metaDataReader.GeneratePocoAsync();
            Assert.IsNotNull(code);
            var count = metaDataReader.ClassList.Count; 
            Assert.AreEqual(count, 11);
        }

        [Test]
        public void GeneratePocoFromFileNotExistTest()
        {
            string code = "";
            string url = "file_not_exist";
            MetaDataReader metaDataReader = new MetaDataReader(url);
            Assert.Throws<FileNotFoundException>(async () => code = await metaDataReader.GeneratePocoAsync());
            Assert.IsEmpty(code);

        }
        [Test]
        public void GeneratePocoFromNotValidXmlFileTest()
        {
            string code = "";
            string url = @"data\invalidxml.xml";
            MetaDataReader metaDataReader = new MetaDataReader(url);
            Assert.Throws<XmlException>(async () => code = await metaDataReader.GeneratePocoAsync());
            Assert.IsEmpty(code);

        }

        
    }
}
