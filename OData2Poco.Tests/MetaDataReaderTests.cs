//#define local

using System;
using System.IO;
using System.Net;
using System.Xml;
using NUnit.Framework;

namespace OData2Poco.Tests
{

    [TestFixture]
    public partial class MetaDataReaderTests
    {
        const string UrlV4 = "http://services.odata.org/V4/Northwind/Northwind.svc";
        const string UrlV3 = "http://services.odata.org/V3/Northwind/Northwind.svc";
        private const string LocalUrl = "http://asd-pc/odata2/api/northwind";

        [Test]
        
        [TestCase(@"data\northwindV4.xml","4.0")] //filename 
        [TestCase(@"data\northwindV3.xml","1.0")]
        public void ReadMetaDataAsStringTest(string url ,string serviceVersion)
        {
            var metaData = new MetaDataReader(url).LoadMetaData();
            Assert.AreEqual(metaData.MetaDataVersion, serviceVersion);
            Assert.IsNotEmpty(metaData.MetaDataAsString);
            Assert.AreEqual(metaData.ServiceHeader.Count, 0);
        }

        //todo: check all metadata properties
        [Test]
        [TestCase(UrlV4,"4.0")]
        [TestCase(UrlV3,"1.0")]
        public void ReadMetaDataHttpTest(string url, string serviceVersion)
        {
            var metaData = new MetaDataReader(url).LoadMetaData();
            Assert.AreEqual(metaData.MetaDataVersion, serviceVersion);
            Assert.IsNotEmpty(metaData.MetaDataAsString);
            Assert.Greater(metaData.ServiceHeader.Count,1);
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
            var code = metaDataReader.Generate(new PocoSetting()).ToString();
            //Debug.WriteLine(code);
            //var code = gen.ToString();
            Assert.IsNotEmpty(code);
            StringAssert.Contains("public class Product", code);
            Assert.AreEqual(metaDataReader.Generate(new PocoSetting()).ClassList.Count, expecteCount);
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

            var code = metaDataReader.Generate(new PocoSetting
            {
                AddKeyAttribute = true,
                AddTableAttribute = true,
                AddRequiredAttribute = true,
                AddNullableDataType = true
            }).ToString();
           
            Assert.IsNotEmpty(code);
            StringAssert.Contains("public class Product", code);
            StringAssert.Contains("[Key]", code);
            Assert.AreEqual(metaDataReader.Generate(new PocoSetting()).ClassList.Count, expecteCount);
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
            var gen = metaDataReader.Generate(new PocoSetting()); //.Generator; //.Execute();
             
            var code = gen.ToString();
            Assert.IsNotEmpty(code);
            StringAssert.Contains("public class Product", code);
            //Assert.AreEqual(metaDataReader.Generate().GetClassTemplateList().Count, expecteCount);

        }

        [Test]
        [TestCase("http://not_valid_url.com")] //not valid url
        [TestCase("http://www.google.com")] //not odata support
        public void GeneratePocoInvalidODataOrUrlTest(string url)
        {
            string  code ="";
            var metaDataReader = new MetaDataReader(url);
            Assert.Throws<WebException>(() => code = metaDataReader.Generate(new PocoSetting()).ToString()); //.GeneratePoco());
            Assert.IsEmpty(code);
        }

#if local
        //test secured servers
        [Test]
        public void GeneratePocoWithValidAccountTes()
        {
            var metaDataReader = new MetaDataReader(LocalUrl, "user", "****");
            var code = metaDataReader.GeneratePoco();
            Assert.IsNotEmpty(code);

        }


        [Test]
        public void GeneratePocoWithInValidAccountTest()
        {
            var msg = "The remote server returned an error: (401) Unauthorized";
            MetaDataReader metaDataReader = new MetaDataReader(LocalUrl, "user_invalid", "password");
            var ex = Assert.Throws<WebException>(() => metaDataReader.GeneratePoco());
            Debug.WriteLine(ex.Message);
            StringAssert.Contains(msg, ex.Message);

        }
#endif

        [Test]
        public void GeneratePocoFromFileNotExistTest()
        {
            string code = "";
            string url = "file_not_exist";
            MetaDataReader metaDataReader = new MetaDataReader(url);
           // Assert.Throws<FileNotFoundException>(() => code = metaDataReader.GeneratePoco());
            Assert.Throws<FileNotFoundException>(() => code = metaDataReader.Generate(new PocoSetting()).ToString()); //.GeneratePoco());
            Assert.IsEmpty(code);
        }

        [Test]
        public void GeneratePocoFromNotValidXmlFileTest()
        {
            string code = "";
            string url = @"data\invalidxml.xml";
            MetaDataReader metaDataReader = new MetaDataReader(url);
            Assert.Throws<XmlException>(() => code = metaDataReader.Generate(new PocoSetting()).ToString()); //.GeneratePoco());
            Assert.IsEmpty(code);

        }


    }
}
