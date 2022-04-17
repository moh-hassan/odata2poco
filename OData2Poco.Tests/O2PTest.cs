using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using NUnit.Framework;
using OData2Poco.Api;

namespace OData2Poco.Tests
{

    public class O2PTest
    {

        [OneTimeSetUp]
        public void Init()
        {
            Environment.CurrentDirectory = TestContext.CurrentContext.TestDirectory;

        }

        [Test]
        //[TestCaseSource(typeof(TestSample), nameof(TestSample.UrlCases))]
        [TestCaseSource(typeof(TestSample), nameof(TestSample.FileCases))]
        public async Task GenerateDefaultTest(string url, string version, int n)
        {
            var connString = new OdataConnectionString { ServiceUrl = url };
            var o2p = new O2P();
            var code = await o2p.GenerateAsync(connString);
            Assert.IsTrue(code.Contains("public partial class Product"));
        }
       
        [Test]
        public async Task GenerateDefaultTestV4()
        {
            var url = TestSample.TripPin4;
            Console.WriteLine(url);
            var connString = new OdataConnectionString { ServiceUrl = url };
            var o2p = new O2P();
            var code = await o2p.GenerateAsync(connString);
            Assert.IsTrue(code.Contains("public partial class City"));
        }

        [Test]
        public async Task GenerateDefaultTestV3()
        {
            var url = TestSample.NorthWindV3;
            var connString = new OdataConnectionString { ServiceUrl = url };
            var o2p = new O2P();
            var code = await o2p.GenerateAsync(connString);
            Assert.IsTrue(code.Contains("public partial class Product"));
        }

        [Test]
        public async Task Filter_by_namespace_Test()
        {
            var url = TestSample.NorthWindV4;
            var connString = new OdataConnectionString { ServiceUrl = url };
            var setting = new PocoSetting { Include = new List<string> { "NorthwindModel*" } };
            var o2p = new O2P(setting);
            var code = await o2p.GenerateAsync(connString);
            Assert.IsTrue(code.Contains("public partial class Product"));
        }

        [Test]         
        [TestCaseSource(typeof(TestSample), nameof(TestSample.FileCases))]
        public async Task GenerateFromXmlContents(string fileName, string version, int n)
        {
            string xml = File.ReadAllText(fileName);
            var o2p = new O2P();
            var code = await o2p.GenerateAsync(xml);
            Console.WriteLine(code);
            Assert.IsTrue(code.Contains("public partial class Product"));           
        }

    }
}
