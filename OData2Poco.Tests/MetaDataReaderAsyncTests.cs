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
        const string UrlV4 = "http://services.odata.org/V4/Northwind/Northwind.svc";
        const string UrlV3 = "http://services.odata.org/V3/Northwind/Northwind.svc";
      
        //***********************http test*****************************
        [Test]
        [TestCase(UrlV4,26)]
        [TestCase(UrlV3,26)]
        [TestCase(@"data\northwindV4.xml", 11)] //filename , countof entities in the model
        [TestCase(@"data\northwindV3.xml", 11)]
        //expectedCount: number of generated classes
        public void GeneratePocoFromHttporFileTest(string url, int expecteCount)
        {
            var metaDataReader = new MetaDataReader(url);
            var code = metaDataReader.Generate(new PocoSetting()).ToString();
            Assert.IsNotEmpty(code);
            StringAssert.Contains("public class Product", code);
            Assert.AreEqual(metaDataReader.Generate(new PocoSetting()).ClassList.Count, expecteCount);
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
