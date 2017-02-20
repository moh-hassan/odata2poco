
//#define local

using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using NUnit.Framework;
using OData2Poco.Extension;


namespace OData2Poco.Tests
{
    
    [TestFixture]
    class O2PTest
    {
       

        [Test]
        [TestCaseSource(typeof(TestSample), "UrlCases")]
        public async void GenerateFromHttpDefaultSettingTest(string url, string version, int n)
        {
            //var url = "http://services.odata.org/V4/OData/OData.svc";
            O2P o2p = new O2P();
            var code = await o2p.GenerateAsync(new Uri(url));
            Debug.WriteLine(code);
           
            Assert.AreEqual(o2p.MetaDataVersion, version);
            Assert.AreEqual(o2p.ClassList.Count, n);
            Assert.IsTrue(code.Contains(o2p.SchemaNamespace));
            StringAssert.DoesNotContain("System.ComponentModel.DataAnnotations.Schema", code);
            StringAssert.DoesNotContain("System.ComponentModel.DataAnnotations", code);
        }

        [Test]
        [TestCaseSource(typeof(TestSample), "UrlCases")]
        public async void GenerateFromHttpWithSettingTest(string url, string version, int n)
        {
             PocoSetting setting = new PocoSetting
            {
                AddNullableDataType = true,
                AddKeyAttribute = true,
                AddTableAttribute = true,
                AddRequiredAttribute = true,
                AddNavigation = true
            };

             //var url = "http://services.odata.org/V4/OData/OData.svc";
             O2P o2p = new O2P( setting);
             var code = await o2p.GenerateAsync(new Uri(url));
             Debug.WriteLine(code);

             Assert.AreEqual(o2p.MetaDataVersion, version);
             Assert.AreEqual(o2p.ClassList.Count, n);
             Assert.IsTrue(code.Contains(o2p.SchemaNamespace));

             StringAssert.Contains("[Key]", code);
             StringAssert.Contains("[Required]", code);
             StringAssert.Contains("[Table", code);
             StringAssert.Contains("virtual public Supplier Supplier  {get;set;}", code);
             StringAssert.DoesNotContain("public class Product :",code);
             StringAssert.Contains("System.ComponentModel.DataAnnotations.Schema",code); 
             StringAssert.Contains("System.ComponentModel.DataAnnotations",code); 

        }

        [Test]
        [TestCaseSource(typeof(TestSample), "UrlCases")]
        public async void GenerateFromHttpWithSettingEagerTest(string url, string version, int n)
        {
            PocoSetting setting = new PocoSetting
            {
                AddEager = true
            };

            //var url = "http://services.odata.org/V4/OData/OData.svc";
            O2P o2p = new O2P(setting);
            var code = await o2p.GenerateAsync(new Uri(url));
            Debug.WriteLine(code);

            Assert.AreEqual(o2p.MetaDataVersion, version);
            Assert.AreEqual(o2p.ClassList.Count, n);
            Assert.IsTrue(code.Contains(o2p.SchemaNamespace));

            StringAssert.Contains("public Supplier Supplier  {get;set;}", code);
            
        }

        [Test]
        [TestCaseSource(typeof(TestSample), "UrlCases")]
        public async void GenerateFromHttpWithSettingInheritTest(string url, string version, int n)
        {
            PocoSetting setting = new PocoSetting
            {
                Inherit = "MyBaseClass, MyInterface"
            };

            //var url = "http://services.odata.org/V4/OData/OData.svc";
            O2P o2p = new O2P(setting);
            var code = await o2p.GenerateAsync(new Uri(url));
            Debug.WriteLine(code);

            Assert.AreEqual(o2p.MetaDataVersion, version);
            Assert.AreEqual(o2p.ClassList.Count, n);
            Assert.IsTrue(code.Contains(o2p.SchemaNamespace));

            StringAssert.Contains(": MyBaseClass, MyInterface", code);

        }

        [Test]
        [TestCaseSource(typeof(TestSample), "UrlCases")]
        public async void GenerateFromHttpWithSettingNamepaceTest(string url, string version, int n)
        {
            PocoSetting setting = new PocoSetting
            {
                NamespacePrefix = "MyNamespace1.MyNamespace2"
            };

            //var url = "http://services.odata.org/V4/OData/OData.svc";
            O2P o2p = new O2P(setting);
            var code = await o2p.GenerateAsync(new Uri(url));
            Debug.WriteLine(code);

            Assert.AreEqual(o2p.MetaDataVersion, version);
            Assert.AreEqual(o2p.ClassList.Count, n);

            var namespc = (setting.NamespacePrefix + "." + o2p.SchemaNamespace).Replace("..", ".");
            namespc = namespc.TrimEnd('.');
            //Assert.IsFalse(code.Contains(o2p.SchemaNamespace));
            StringAssert.Contains(namespc, code);

        }

        [Test]
        [TestCaseSource(typeof(TestSample), "UrlCases")]
        public async void GenerateFromHttpWithSettingEmptyNamepaceTest(string url, string version, int n)
        {
            PocoSetting setting = new PocoSetting
            {
                NamespacePrefix = ""
            };

            //var url = "http://services.odata.org/V4/OData/OData.svc";
            O2P o2p = new O2P(setting);
            var code = await o2p.GenerateAsync(new Uri(url));
            Debug.WriteLine(code);

            Assert.AreEqual(o2p.MetaDataVersion, version);
            Assert.AreEqual(o2p.ClassList.Count, n);
            Assert.IsTrue(code.Contains($"namespace {o2p.SchemaNamespace}"));
            
        }

        [Test]
        [TestCaseSource(typeof(TestSample), "FileCases")]
        public  void GenerateFromXmlDefaultSettingTest(string fname, string version, int n)
        {
            var xml = File.ReadAllText(fname);
            O2P o2p = new O2P();
            var code = o2p.Generate(xml);
            Debug.WriteLine(code);
            Assert.AreEqual(o2p.MetaDataVersion, version);
            Assert.AreEqual(o2p.ClassList.Count, n);
            Assert.IsTrue(code.Contains(o2p.SchemaNamespace));

        }

        [Test]
        [TestCaseSource(typeof(TestSample), "FileCases")]
        public void GenerateFromXmlWithSettingTest(string fname, string version, int n)
        {
            var xml = File.ReadAllText(fname);
            O2P o2p = new O2P();
            var code = o2p.Generate(xml);
            Debug.WriteLine(code);
            Assert.AreEqual(o2p.MetaDataVersion, version);
            Assert.AreEqual(o2p.ClassList.Count, n);
            Assert.IsTrue(code.Contains(o2p.SchemaNamespace));

        }

 
     
      

        public async Task<string> BaseTest(string url, string version, int n, PocoSetting setting )
        {
            O2P o2P = new O2P(setting);

            var code = await o2P.GenerateAsync(new Uri(url) );
        //    var metaString = await o2P.SaveMetaDataTo("north.xml");
            if (o2P.MediaType == Media.Http)
            {
                Assert.Greater(o2P.ServiceHeader.Count, 0);
            }
            Debug.WriteLine(code);
            //Debug.WriteLine(metaString);

            Assert.AreEqual(o2P.MetaDataVersion, version);
            Assert.AreEqual(o2P.ClassList.Count, n);
            Assert.IsTrue(code.Contains(o2P.SchemaNamespace));
            //Assert.IsTrue(MatchFileContent("north.cs", code));
            //Assert.IsTrue(MatchFileContent("north.xml", metaString));
            return code;
        }

       

       
 
      

      
        [Test]
        [TestCase("http://not_valid_url.com")] //not valid url
        [TestCase("http://www.google.com")] //not odata support

        //[ExpectedException(typeof(WebException))]
        //System.Net.WebException : The remote name could not be resolved: 'not_valid_url.com'
        //System.Net.WebException : The remote server returned an error: (404) Not Found.
        public async void GeneratePocoInvalidODataOrUrlTest(string url)
        {
            var o2P = new O2P();
            var code = "";
            var metaString = "";


            try
            {
                code = await o2P.GenerateAsync(new Uri(url));
                //metaString = o2P.SaveMetaDataTo("north.xml");
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.FullExceptionMessage());
            }

            Assert.IsEmpty(code);
            Assert.IsEmpty(metaString);
        }
      


        [Test]
        public void GeneratePocoFromEmtyXmlTest()
        {

            // <System.Exception> (Metadata is not available)
            var o2P = new O2P();
            var code = "";
            var metaString = "";
            Assert.Throws<ArgumentNullException>(() =>
            {
                code =  o2P.Generate("");
            //    metaString = await o2P.SaveMetaDataTo("north.xml");
            });

            Assert.IsEmpty(code);
            Assert.IsEmpty(metaString);

        }
        [Test]
        public void GeneratePocoFromNullUrlTest()
        {
            var url = "";
            var o2P = new O2P();
           
            Assert.Throws<UriFormatException>(async () =>
            {
               var code = await o2P.GenerateAsync(new Uri(url));
            });

        }

 
        //---------------------------------------------
#if local
        [Test]
        public async void GenerateCodeUrlValidAuthenticateNoSettingTest()
        {
            string url = "http://asd-pc/odata2/api/northwind";
            string version = "4.0";
            int n=26;
            var o2P = new O2P();

            var code = await o2P.GenerateAsync(new Uri(url), "hassan", "123");
            Debug.WriteLine(code);
            StringAssert.Contains("public class Product", code);
        }
        [Test]
        public void GenerateCodeUrlInvalidAuthenticateNoSettingTest()
        {
            string url = "http://asd-pc/odata2/api/northwind";
            string version = "4.0";
            int n = 26;
            var code = "";
            var metaString = "";
            //File.Delete("north.cs");
            //File.Delete("north.xml");

            var o2P = new O2P();
         var ex=   Assert.Throws<WebException>(async () =>
            {
                code = await o2P.GenerateAsync(new Uri(url), "hassan1", "123");
                //metaString = o2P.SaveMetaDataTo("north.xml");
            }); //.GeneratePoco());
            Debug.WriteLine(ex.Message);
            Console.WriteLine(code);
            //Assert.IsFalse(File.Exists("north.cs"));
            //Assert.IsFalse(File.Exists("north.xml"));
            Assert.IsEmpty(code);
            Assert.IsEmpty(metaString);
        }
#endif
    }
}

 
