using System;
using System.IO;
using System.Threading.Tasks;
using NUnit.Framework;
using OData2Poco.Api;

namespace OData2Poco.Tests
{

    [TestFixture]
    class O2PTest
    {
        const string UrlV4 = "http://services.odata.org/V4/Northwind/Northwind.svc";
        const string UrlV3 = "http://services.odata.org/V3/Northwind/Northwind.svc";

        
        [OneTimeSetUp]
        public void Init()
        {
            Environment.CurrentDirectory = TestContext.CurrentContext.TestDirectory;
             
        }

        [Test]
        //[TestCase(UrlV4)]
        //[TestCase(UrlV3)]
        [TestCaseSource(typeof(TestSample), nameof(TestSample.UrlCases))]
        public async Task GenerateDefaultFromHttpTest(string url, string version, int n)
        {
           
            var o2p = new O2P();
            var code = await o2p.GenerateAsync(new Uri(url));
            Assert.IsTrue(code.Contains("public partial class Product"));
        }

        [Test]
        [TestCaseSource(typeof(TestSample), nameof(TestSample.FileCases))]
        public void GenerateDefaultFromXmlFilesTest(string file, string version, int n)
        {
            Console.WriteLine($"BaseDirectory: {TestSample.BaseDirectory}");
            Console.WriteLine($"FakeFolder: {TestSample.FakeFolder}");
            Console.WriteLine($"NorthWindV4: {TestSample.NorthWindV4}");
          
            var o2p = new O2P();
            var xml = File.ReadAllText(file);
            var code = o2p.Generate(xml);

            Assert.IsTrue(code.Contains("public partial class Product"));
        }


    }
}
