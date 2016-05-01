//#define local
using System;
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

        //***********************http test*****************************
        [Test]
        [TestCase("http://services.odata.org/V4/OData/OData.svc",11)] //v4
        [TestCase("http://services.odata.org/V3/OData/OData.svc",11)] //v3
        //expecteCount: count of classes /Entities
        public void GeneratePocoFromHttpTest(string url,int expecteCount)
        {
            var metaDataReader = new MetaDataReader(url);
            var code = metaDataReader.GeneratePoco();
            Assert.IsNotEmpty(code);
            StringAssert.Contains("public class Product", code);
            StringAssert.Contains("public class FeaturedProduct", code);
            Assert.AreEqual(metaDataReader.ClassList.Count, expecteCount);
            //metaDataReader.ClassList.ForEach(m =>
            //{
            //    Console.WriteLine(m.Name);
            //});
            
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

        //********************* xml File test **************************************
        
        [Test]
        public void GeneratePocoFromXmlFileV4Test()
        {
            string url = @"data\northwindV4.xml";
            MetaDataReader metaDataReader = new MetaDataReader(url);
            metaDataReader.LoadMetaData();
            Assert.IsNotNull(metaDataReader.GeneratePoco());
            var count = metaDataReader.ClassList.Count;  
            Console.Write(count);
            Assert.AreEqual(count, 11);
        }


        [Test]
        public void GeneratePocoFromXmlFileV3Test()
        {
            string url = @"data\northwindV3.xml";
            MetaDataReader metaDataReader = new MetaDataReader(url);
          var code=  metaDataReader.GeneratePoco();
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
