// Copyright (c) Mohamed Hassan & Contributors. All rights reserved. See License.md in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using OData2Poco.Http;

namespace OData2Poco.Tests
{
    public class CustomHttpClientProxyTest
    {
        private bool _isLive;
        private string _token;

        [OneTimeSetUp]
        public void Setup()
        {
            _isLive = !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("LiveTest", EnvironmentVariableTarget.User));
            _token = Environment.GetEnvironmentVariable("Token", EnvironmentVariableTarget.User);

        }

        [Test]
        public async Task Proxy_with_valid_credentials_should_succeed()
        {
            //test is run only on local machine with proxy running
            if (!_isLive)
            {
                Assert.Ignore("Ignore proxy test in CI");
                return;
            }
            string url = OdataService.Northwind;
            OdataConnectionString cs = new OdataConnectionString
            {
                ServiceUrl = url,
                Proxy = "http://localhost:8888",
                ProxyUser = "user:password"
            };
            var customClient = new CustomHttpClient(cs);
            var metaData = await customClient.ReadMetaDataAsync();
            Assert.That(metaData, Does.StartWith(@"<?xml version=""1.0"" encoding=""UTF-8""?>"));
        }

        [Test]
        public void Proxy_with_Invalid_credentials_should_fail()
        {
            //test is run only on local machine with proxy running
            if (!_isLive)
            {
                Assert.Ignore("Ignore proxy test in CI");
                return;
            }
            string url = OdataService.Northwind;
            url = url.Replace("localhost", "ipv4.fiddler");
            OdataConnectionString cs = new OdataConnectionString
            {
                ServiceUrl = url,
                Proxy = "http://127.0.0.1:8888",
                ProxyUser = "user:invalid_password"
            };
            var customClient = new CustomHttpClient(cs);
            var msg = "Response status code does not indicate success: 407 (Proxy Auth Required).";
            Assert.That(async () => await customClient.ReadMetaDataAsync(), Throws.Exception.TypeOf<HttpRequestException>()
                .With.Message.EqualTo(msg));
        }
    }
}
