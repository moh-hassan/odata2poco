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

        [TestCase("http://services.odata.org/V4/OData/OData.svc", 11)] //v4 //url , countof entities in the model
        [TestCase("http://services.odata.org/V3/OData/OData.svc", 11)] //v3
        [TestCase(@"data\northwindV4.xml", 11)] //filename , countof entities in the model
        [TestCase(@"data\northwindV3.xml", 11)]
        //expectedCount: number of generated classes
        public void GeneratePocoFromHttporFileTest(string url, int expecteCount)
        {
            var metaDataReader = new MetaDataReader(url);
            var code = metaDataReader.Execute().ToString();
            Assert.IsNotEmpty(code);
            StringAssert.Contains("public class Product", code);
            StringAssert.Contains("public class FeaturedProduct", code);
            Assert.AreEqual(metaDataReader.Generator.ClassDictionary.Count, expecteCount);
        }

        //[Test]
        //[TestCase("http://not_valid_url.com")] //not valid url
        //[TestCase("http://www.google.com")] //not odata support
        //public void GeneratePocoInvalidODataOrUrlTest(string url)
        //{
        //    var code = "";
        //    var metaDataReader = new MetaDataReader(url);
        //    Assert.Throws<WebException>(async () => code = await metaDataReader.GeneratePocoAsync());
        //    Assert.IsEmpty(code);
        //}

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

       
     
        //[Test]
        //public void GeneratePocoFromFileNotExistTest()
        //{
        //    string code = "";
        //    string url = "file_not_exist";
        //    MetaDataReader metaDataReader = new MetaDataReader(url);
        //    Assert.Throws<FileNotFoundException>(async () => code = await metaDataReader.GeneratePocoAsync());
        //    Assert.IsEmpty(code);

        //}
        //[Test]
        //public void GeneratePocoFromNotValidXmlFileTest()
        //{
        //    string code = "";
        //    string url = @"data\invalidxml.xml";
        //    MetaDataReader metaDataReader = new MetaDataReader(url);
        //    Assert.Throws<XmlException>(async () => code = await metaDataReader.GeneratePocoAsync());
        //    Assert.IsEmpty(code);

        //}

        
    }
}
