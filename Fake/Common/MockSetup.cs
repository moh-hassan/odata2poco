// Copyright (c) Mohamed Hassan & Contributors. All rights reserved. See License.md in the project root for license information.

using System.Net;
using FluentAssertions;
using NUnit.Framework;
using OData2Poco.HttpMock;


#if Test_Cli
namespace OData2Poco.CommandLine.Test;
#else
namespace OData2Poco.Tests;
#endif

[SetUpFixture]
public class MockSetup
{
    private MockBuilder _mocks;

    [OneTimeSetUp]
    public void Setup()
    {
        Environment.CurrentDirectory = TestContext.CurrentContext.TestDirectory;
        _mocks = new MockBuilder(Mocks.Port);
        _mocks.Start();
    }

    [OneTimeTearDown]
    public void Teardown()
    {
        _mocks.Stop();
    }
}

internal class MocksTest
{
    [Test]
    [Category("mocks")]
    [TestCaseSource(nameof(MockTestData))]
    public async Task Available_mocks_test_should_success(string url, object expectedCode)
    {
        var client = new HttpClient();
        var response = await client.GetAsync(url);
        var code = response.StatusCode;
        code.Should().Be((HttpStatusCode)expectedCode);
    }
    private static IEnumerable<TestCaseData> MockTestData
    {
        get
        {
            yield return new TestCaseData(Mocks.TripPin.OdataUrl, 200);
            yield return new TestCaseData(Mocks.NorthWindV4.OdataUrl, 200);
            yield return new TestCaseData(Mocks.NorthWindV3.OdataUrl, 200);
            yield return new TestCaseData(Mocks.ODataV3.OdataUrl, 200);
            yield return new TestCaseData(Mocks.Books.OdataUrl, 200);
            yield return new TestCaseData($"{Mocks.BaseUrl}/mysite", 404);//unknown
        }
    }
}
