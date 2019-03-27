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
     
        [OneTimeSetUp]
        public void Init()
        {
            Environment.CurrentDirectory = TestContext.CurrentContext.TestDirectory;
             
        }

        [Test]
     
        [TestCaseSource(typeof(TestSample), nameof(TestSample.UrlCases))]
        [TestCaseSource(typeof(TestSample), nameof(TestSample.FileCases))]
        public async Task GenerateDefaultTest(string url, string version, int n)
        {
            var connString = new OdataConnectionString {ServiceUrl = url};
            var o2p = new O2P();
            var code = await o2p.GenerateAsync(connString);
            Assert.IsTrue(code.Contains("public partial class Product"));
        }

       


    }
}
