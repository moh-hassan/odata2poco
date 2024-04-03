// Copyright (c) Mohamed Hassan & Contributors. All rights reserved. See License.md in the project root for license information.

namespace OData2Poco.Tests;

using System.Collections;
using Api;
using Fake.Common;

public class O2PTest
{
    [OneTimeSetUp]
    public void Init()
    {
        Environment.CurrentDirectory = TestContext.CurrentContext.TestDirectory;
    }

    [Test]
    [TestCaseSource(typeof(TestSample), nameof(TestSample.FileCases))]
    public async Task GenerateDefaultTest(string url, string version, int n)
    {
        var connString = new OdataConnectionString
        {
            ServiceUrl = url
        };
        var o2P = new O2P();
        var code = await o2P.GenerateAsync(connString).ConfigureAwait(false);
        Assert.That(code, Does.Contain("public partial class Product"));
    }

    [Test]
    public async Task GenerateDefaultTestV4()
    {
        var url = TestSample.TripPin4;
        var connString = new OdataConnectionString
        {
            ServiceUrl = url
        };
        var o2P = new O2P();
        var code = await o2P.GenerateAsync(connString).ConfigureAwait(false);
        Assert.That(code, Does.Contain("public partial class City"));
    }

    [Test]
    public async Task GenerateDefaultTestV3()
    {
        var url = OdataService.Northwind3;
        var connString = new OdataConnectionString
        {
            ServiceUrl = url
        };
        var o2P = new O2P();
        var code = await o2P.GenerateAsync(connString).ConfigureAwait(false);
        Assert.That(code, Does.Contain("public partial class Product"));
    }

    [Test]
    public async Task Filter_by_namespace_Test()
    {
        var url = OdataService.Northwind4;
        var connString = new OdataConnectionString
        {
            ServiceUrl = url
        };
        var setting = new PocoSetting
        {
            Include = ["NorthwindModel*"]
        };
        var o2P = new O2P(setting);
        var code = await o2P.GenerateAsync(connString).ConfigureAwait(false);
        Assert.That(code, Does.Contain("public partial class Product"));
    }

    [Test]
    [TestCaseSource(typeof(TestSample), nameof(TestSample.FileCases))]
    public async Task GenerateFromXmlContents(string fileName, string version, int n)
    {
        var xml = File.ReadAllText(fileName);
        var o2P = new O2P();
        var code = await o2P.GenerateAsync(xml).ConfigureAwait(false);
        Assert.That(code, Does.Contain("public partial class Product"));
    }

    [Test]
    public async Task GenerateFromRemoteXmlfile()
    {
        var url = "https://raw.githubusercontent.com/moh-hassan/odata2poco/master/Fake/trippinV4.xml";
        var connString = new OdataConnectionString
        {
            ServiceUrl = url
        };
        var o2P = new O2P();
        var code = await o2P.GenerateAsync(connString).ConfigureAwait(false);
        Assert.That(code, Does.Contain("public partial class City"));
    }

    [Test]
    public async Task Enable_read_write_properties_even_for_readonly()
    {
        var url = "https://raw.githubusercontent.com/moh-hassan/odata2poco/master/Fake/trippinV4.xml";
        var connString = new OdataConnectionString
        {
            ServiceUrl = url
        };
        var setting = new PocoSetting
        {
            ReadWrite = true //Allow readonly property to be read/write
        };
        var o2P = new O2P(setting);
        var code = await o2P.GenerateAsync(connString).ConfigureAwait(false);
        //TripId is readonly, but overwrite by setting option
        Assert.That(code, Does.Contain(" public int TripId {get;set;}"));
    }

    [Test]
    public async Task O2p_call_static_method_test()
    {
        //Arrange
        var cs = new OdataConnectionString
        {
            ServiceUrl = OdataService.Northwind4
        };

        //Act
        var ps = new PocoSetting();
        var code = await O2P.GeneratePocoAsync(cs, ps).ConfigureAwait(false);

        //Assert
        code.Should().ContainAll("public partial class Category",
            "public partial class CustomerDemographic");
    }

    [Test]
    public async Task O2p_call_static_using_json_test()
    {
        //Arrange
        var config = new Configuration
        {
            ConnectionString = new OdataConnectionString
            {
                ServiceUrl = OdataService.Northwind4
            },
            Setting = new PocoSetting
            {
                NameCase = CaseEnum.Camel
            }
        };
        var json = config.ToJson();

        //Act
        var code = await O2P.GeneratePocoAsync(json).ConfigureAwait(false);

        //Assert
        code.Should().ContainAll("public partial class Category",
            "public partial class CustomerDemographic");
    }

    [Test]
    public async Task O2p_call_static_using_json_test2()
    {
        //Arrange
        var json = TestCaseFactory.Northwind;

        //Act
        var code = await O2P.GeneratePocoAsync(json).ConfigureAwait(false);
        //Assert
        code.Should().ContainAll("public partial class Category",
            "public partial class CustomerDemographic");
    }

    [Test]
    [Category("mock")]
    [TestCaseSource(typeof(TestCaseFactory), nameof(TestCaseFactory.TestMockCases))]
    public async Task OdataService_mock_test(string url, string version)
    {
        var connString = new OdataConnectionString
        {
            ServiceUrl = url
        };
        var o2P = new O2P();
        var code = await o2P.GenerateAsync(connString).ConfigureAwait(false);
        Assert.That(code, Does.Contain("public partial class"));
        Assert.That(o2P.MetaData.MetaDataVersion, Is.EqualTo(version));
    }

#if OPENAPI
        [Category("openapi")]
        [Test]
        [TestCaseSource(typeof(TestCaseFactory), "TestCases")]
        public async Task Generate_openApi(string url, string fileName, string expected)
        {
            var connString = new OdataConnectionString { ServiceUrl = url };
            var setting = new PocoSetting
            {
                OpenApiFileName = fileName
            };
            var o2p = new O2P(setting);
            var text = await o2p.GenerateOpenApiAsync(connString);
            text.Should().Contain(expected);
        }
#endif
}

public static class TestCaseFactory
{
    internal static string Northwind = @$"
       {{'ConnectionString': {{
            'ServiceUrl': '{Path2Json(TestSample.NorthWindV4)}',
            'UserName': null,
            'Password': null,
            'Domain': null,
            'Proxy': null,
            'TokenUrl': null,
            'TokenParams': null,
            'ParamFile': null,
            'Authenticate': 0,
            'TlsProtocol': 3072
          }},
          'Setting': {{
            'AddNullableDataType': false,
            'AddNavigation': false,
            'AddEager': false,
            'Lang': 1,
            'Inherit': '',
            'UseInheritance': true,
            'NamespacePrefix': '',
            'NameCase': 2,
            'EntityNameCase': 0,
            'RenameMap': null,
            'Attributes': [],
            'MultiFiles': false,
            'ModuleName': null,
            'AddReference': false,
            'GeneratorType': 0,
            'Include': [],
            'ReadWrite': false,
            'EnableNullableReferenceTypes': false,
            'InitOnly': false,
            'OpenApiFileName': '',
            'CodeFilename': 'UnDefined.txt',
            'UseFullName': false          
        }}  
     }}
";

    public static IEnumerable TestCases
    {
        get
        {
            yield return new TestCaseData(TestSample.NorthWindV4, "swagger.json", "\"openapi\": \"3.0.1\"");
            yield return new TestCaseData(TestSample.NorthWindV4, "swagger.yml", "openapi: 3.0.1");
            yield return new TestCaseData(
                TestSample.UrlTripPinService,
                "swaggerPin.json",
                @"""openapi"": ""3.0.1""");
            yield return new TestCaseData(TestSample.UrlTripPinService, "swaggerPin.yml", "openapi: 3.0.1");
        }
    }

    public static IEnumerable TestMockCases
    {
        get
        {
            //url ,version
            yield return new TestCaseData(OdataService.Northwind, "4.0");
            yield return new TestCaseData(OdataService.Northwind2, "1.0");
            yield return new TestCaseData(OdataService.Northwind3, "1.0");
            yield return new TestCaseData(OdataService.Northwind4, "4.0");
            yield return new TestCaseData(OdataService.Trippin, "4.0");
        }
    }

    private static string Path2Json(string path)
    {
        return path.Replace(@"\", @"\\");
    }
}
