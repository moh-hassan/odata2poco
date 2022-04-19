using System.Threading.Tasks;
using NUnit.Framework;
using OData2Poco.Http;

namespace OData2Poco.Tests
{
    class CustomeHttpClientTest
    {
        [Test]
        [TestCase(TestSample.UrlNorthWindV4)]       
        [TestCase(TestSample.UrlNorthWindV3)]
        public async Task No_auth_ReadMetaDataTest(string url)
        {
            var connection = new OdataConnectionString
            {
                ServiceUrl = url,
            };
            var cc = new CustomeHttpClient(connection);
            var metadata = await cc.ReadMetaDataAsync();
            Assert.That(metadata.Length, Is.GreaterThan(0));
        }

        [Test]
        public async Task Token_Auth_CheckHttpRequestMessage_HttpGet()
        {
            var connection = new OdataConnectionString
            {
               
                ServiceUrl = "http://localhost/odata2/api/northwind",
                Password = "accessToken",
                Authenticate = AuthenticationType.Token


            };
            var client = new CustomeHttpClient(connection, new CustomeHandler(r =>
             {
                 Assert.AreEqual(r.RequestUri.ToString(), "http://localhost/odata2/api/northwind/$metadata");

                 Assert.IsNotNull(r.Headers.UserAgent);
                 Assert.AreEqual(r.Headers.UserAgent.ToString(), "OData2Poco");

                 Assert.IsNotNull(r.Headers.Authorization);
                 Assert.AreEqual(r.Headers.Authorization.ToString(), 
                     "Bearer accessToken");
             }));
            var metadata = await client.ReadMetaDataAsync();

        }

        [Test]
        public async Task Basic_Auth_CheckHttpRequestMessage_HttpGet()
        {
            var connection = new OdataConnectionString
            {
                ServiceUrl = "http://localhost/odata2/api/northwind",
                UserName = "user1",
                Password = "secret",
                Authenticate = AuthenticationType.Basic


            };
            var client = new CustomeHttpClient(connection, new CustomeHandler(r =>
             {
                 Assert.AreEqual(r.RequestUri.ToString(), "http://localhost/odata2/api/northwind/$metadata");

                 Assert.IsNotNull(r.Headers.UserAgent);
                 Assert.AreEqual(r.Headers.UserAgent.ToString(), "OData2Poco");

                 Assert.IsNotNull(r.Headers.Authorization);
                 Assert.AreEqual(r.Headers.Authorization.ToString(),
                     "Basic dXNlcjE6c2VjcmV0");
             }));
            var metadata = await client.ReadMetaDataAsync();
        }
    }
}
