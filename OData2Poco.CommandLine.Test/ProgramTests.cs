// Copyright (c) Mohamed Hassan & Contributors. All rights reserved. See License.md in the project root for license information.
#pragma warning disable IDE0060  // Remove unused parameter
using OData2Poco.TestUtility;
namespace OData2Poco.CommandLine.Test;

[TestFixture]
public partial class ProgramTests : BaseTest
{

    private readonly ArgumentParser _argumentParser;

    public ProgramTests()
    {
        _argumentParser = new ArgumentParser();

    }
    [OneTimeSetUp]
    public void SetupOneTime()
    {
        //WorkingDirectory
        Environment.CurrentDirectory = TestContext.CurrentContext.TestDirectory;
    }

    private async Task<Tuple<int, string>> RunCommand(string s)
    {
        _argumentParser.ClearLogger();
        _argumentParser.SetLoggerSilent();

        string[] args = s.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
        var retcode = await _argumentParser.RunOptionsAsync(args);
        string outText = ArgumentParser.OutPut;
        var exitCode = retcode;
        Tuple<int, string> tuple = new Tuple<int, string>(exitCode, outText);
        return tuple;
    }

    [Test]
    [TestCaseSource(typeof(TestSample), nameof(TestSample.FileCases))]
    public async Task DefaultSettingTest(string url, string version, int n)
    {
        //Arrange
        var a = $"-r {url} -v";
        //Act
        var tuble = await RunCommand(a);
        var output = tuble.Item2;
        //Assert
        Assert.Multiple(() =>
        {
            Assert.That(tuble.Item1, Is.EqualTo(0));
            Assert.That(output, Does.Contain("public partial class Product"));
            Assert.That(output, Does.Not.Contain("System.ComponentModel.DataAnnotations"));
            Assert.That(output, Does.Not.Contain("System.ComponentModel.DataAnnotations.Schema"));
        });
    }

    [Test]
    [TestCaseSource(typeof(TestSample), nameof(TestSample.FileCases))]
    public async Task PocoSettingTest(string url, string version, int n)
    {
        //Arrange
        var a = $"-r {url} -v -a key tab req -n -b";
        //Act
        var tuple = await RunCommand(a);
        var output = tuple.Item2;
        //Assert
        tuple.Item1.Should().Be(0);

        Assert.That(output, Does.Contain("public partial class Product"));
        Assert.That(output, Does.Contain("[Table(\"Products\")]")); //-a tab/ -t
        Assert.That(output, Does.Contain("System.ComponentModel.DataAnnotations.Schema")); //-a tab  
        Assert.That(output, Does.Contain("[Key]"));
        Assert.That(output, Does.Contain("System.ComponentModel.DataAnnotations")); //-a key  
        Assert.That(output, Does.Contain("[Required]"));
        Assert.That(output, Does.Contain("public virtual Supplier Supplier {get;set;}")); //-n
        Assert.That(output, Does.Contain("int?"));  //-b

    }


    [Test]
    public async Task PocoWithInheritanceTest()
    {
        //Arrange
        var url = TestSample.TripPin4Flag;
        var a = $"-r {url} -v";
        //Act
        var tuble = await RunCommand(a);
        var output = tuble.Item2;
        //Assert
        Assert.That(output, Does.Contain("public partial class PublicTransportation : PlanItem"));
    }

    [Test(Description = "If model inheritance is used (the default) check that the propterties of a base calsss are not duplicated inderived classes")]
    public async Task PropertyInheritenceTest()
    {
        //Arrange
        var url = TestSample.TripPin4Flag;
        var a = $"-r {url} -v";
        //Act
        var tuble = await RunCommand(a);
        var output = tuble.Item2;

        var lines = output.Split('\n');
        var occurneces = lines.Count(l => l.Contains("public int PlanItemId"));
        //Assert
        // Assert.IsTrue(occurneces == 1); // For inheritance, check that PlanItemId property only occurs in the base class
        Assert.That(occurneces, Is.EqualTo(1));

    }
    [Test]
    public async Task PocoWithBaseClassTest()
    {
        //Arrange
        var url = TestSample.TripPin4Flag;
        const string myBaseClass = nameof(myBaseClass);

        var a = $"-r {url} -v -i {myBaseClass}";
        //Act
        var tuble = await RunCommand(a);
        var output = tuble.Item2;
        Assert.That(output, Does.Contain($"public partial class PublicTransportation : {myBaseClass}"));

        var lines = output.Split('\n');
        var occurneces = lines.Count(l => l.Contains("public int PlanItemId"));
        //Assert        
        Assert.That(occurneces, Is.GreaterThan(1));
    }

    [Test(Description = "If model inheritance is not used, the properties from a base class should by duplicated in the derived classes.")]
    public async Task PropertyDuplicationTest()
    {
        //Arrange
        var url = TestSample.TripPin4Flag;
        const string myBaseClass = nameof(myBaseClass);

        var a = $"-r {url} -v -i {myBaseClass}";
        //Act
        var tuble = await RunCommand(a);
        var output = tuble.Item2;
        var lines = output.Split('\n');
        var occurneces = lines.Count(l => l.Contains("public int PlanItemId"));
        //Assert       
        Assert.That(occurneces, Is.GreaterThan(1));
        // If not using model inheritance, check that the PlanItemId property is duplicated in derived classes


    }

    [Test]
    [TestCaseSource(typeof(TestSample), nameof(TestSample.FileCases))]
    public async Task NullableDatatypeTest(string url, string version, int n)
    {
        //Arrange
        var a = $"-r {url} -v  -n -b"; //navigation propertis with complex data type
        //Act
        var tuble = await RunCommand(a);
        //Assert
        var output = tuble.Item2;
        //Assert
        tuble.Item1.Should().Be(0);
        Assert.That(output, Does.Contain("public partial class Product"));
        Assert.That(output, Does.Contain("public virtual Supplier Supplier {get;set;}")); //-n
        Assert.That(output, Does.Contain("int?"));  //-b

    }
    //feature #43
    [Test]
    [TestCase("-B")]
    [TestCase("--enable-nullable-reference")]
    [TestCase("-B -b")]
    public async Task NullableReferencetypeTest(string arg)
    {
        //Arrange
        string url = TestSample.TripPin4;
        var a = $"-r {url} -v {arg}";

        //Act
        var tuble = await RunCommand(a);

        //Assert
        var output = tuble.Item2;
        var list = new List<string>
        {
            "public partial class Person",
            "public string UserName {get;} //PrimaryKey not null ReadOnly",
            "public string FirstName {get;set;} // not null",
            "public string LastName {get;set;} // not null",
            "public List<string>? Emails {get;set;}",
            "public List<Location>? AddressInfo {get;set;}",
            "public PersonGender? Gender {get;set;}",
            "public long Concurrency {get;} // not null ReadOnly",
        };
        output.Should().ContainAll(list);
    }

    [Test]
    [TestCaseSource(typeof(TestSample), nameof(TestSample.FileCases))]
    [Category("json")]
    public async Task PocoSettingWithJsonAttributeTest(string url, string version, int n)
    {
        //Arrange
        //  var a = $"-r {url}  -j -v"; //obsolete
        var a = $"-r {url}  -a json -v";
        //Act
        var tuble = await RunCommand(a);
        var output = tuble.Item2;

        //Assert
        tuble.Item1.Should().Be(0); //exit code
        Assert.That(output, Does.Contain("public partial class Category"));
        Assert.That(output, Does.Contain("[JsonProperty(PropertyName = \"CategoryID\")]"));
        Assert.That(output, Does.Contain("Category"));
        Assert.That(output, Does.Contain(" [JsonProperty(PropertyName = \"CategoryName\")]"));
        Assert.That(output, Does.Contain("CategoryName"));
    }

    [Test]
    [TestCaseSource(typeof(TestSample), nameof(TestSample.FileCases))]
    [Category("json3")]
    public async Task PocoSettingWithJsonAttributeNetCore3Test(string url, string version, int n)
    {
        //Arrange           
        var a = $"-r {url}  -a json3 -v";
        //Act
        var tuble = await RunCommand(a);
        var output = tuble.Item2;

        //Assert
        tuble.Item1.Should().Be(0); //exit code
        Assert.That(output, Does.Contain("public partial class Category"));
        Assert.That(output, Does.Contain("[JsonPropertyName(\"CategoryID\")]"));
        Assert.That(output, Does.Contain("Category"));
        Assert.That(output, Does.Contain(" [JsonPropertyName(\"CategoryName\")]"));
        Assert.That(output, Does.Contain("CategoryName"));
    }
    [Test]
    [TestCaseSource(typeof(TestSample), nameof(TestSample.FileCases))]
    public async Task PocoSettingWithJsonAttributeAndCamelCaseTest(string url, string version, int n)
    {
        //Arrange            
        var a = $"-r {url}  -a json -c camel -v";
        //Act
        var tuble = await RunCommand(a);
        var output = tuble.Item2;
        //Assert
        Assert.That(output, Does.Contain("public partial class Category"));
        Assert.That(output, Does.Contain("[JsonProperty(PropertyName = \"CategoryName\")]"));
        Assert.That(output, Does.Contain("CategoryName"));
        Assert.That(output, Does.Contain("[JsonProperty(PropertyName = \"CategoryID\")]"));
        Assert.That(output, Does.Contain("category"));
    }

    [Test]
    [TestCaseSource(typeof(TestSample), nameof(TestSample.FileCases))]
    public async Task PocoSettingWithJsonAttributePasCaseTest(string url, string version, int n)
    {
        //Arrange
        var a = $"-r {url}   -a json -c PAS -v";
        //Act
        var tuble = await RunCommand(a);
        var output = tuble.Item2;
        //Assert
        tuble.Item1.Should().Be(0);
        Assert.That(output, Does.Contain("[JsonProperty(PropertyName = \"CategoryID\")]"));
        Assert.That(output, Does.Contain("Category"));
        Assert.That(output, Does.Contain(" [JsonProperty(PropertyName = \"CategoryName\")]"));
        Assert.That(output, Does.Contain("CategoryName"));

    }


    [Test]
    [TestCaseSource(typeof(TestSample), nameof(TestSample.FileCases))]
    public async Task PocoSettingEagerTest(string url, string version, int n)
    {
        //Arrange
        var a = $"-r {url} -v -e";
        //Act
        var tuble = await RunCommand(a);
        var output = tuble.Item2;
        tuble.Item1.Should().Be(0);
        //Assert
        Assert.That(output, Does.Contain("public partial class Product")); //-v
        Assert.That(output, Does.Contain("public Supplier Supplier {get;set;}")); //-e

    }

    [Test]
    [TestCaseSource(typeof(TestSample), nameof(TestSample.FileCases))]
    public async Task PocoSettingInheritTest(string url, string version, int n)
    {
        //Arrange
        var a = $"-r {url} -v -i MyBaseClass,MyInterface";
        //Act
        var tuble = await RunCommand(a);
        var output = tuble.Item2;
        //Assert
        tuble.Item1.Should().Be(0);
        Assert.That(output, Does.Contain("public partial class Product : MyBaseClass,MyInterface")); //-i, -v

    }

    [Test]
    [TestCaseSource(typeof(TestSample), nameof(TestSample.FileCases))]
    public async Task PocoSettingNamespaceTest(string url, string version, int n)
    {
        var a = $"-r {url} -v -m MyNamespace1.MyNamespace2";
        //Act
        var tuble = await RunCommand(a);
        var output = tuble.Item2;
        //Assert
        tuble.Item1.Should().Be(0);
        Assert.That(output, Does.Contain("MyNamespace1.MyNamespace2.")); //-m, -v

    }

    [Test]
    [TestCaseSource(typeof(TestSample), nameof(TestSample.FileCases))]
    public async Task FileWithSettingTest(string url, string version, int n)
    {
        //Arrange
        var a = $"-r {url} -v -a key tab req -n";
        //Act
        var tuble = await RunCommand(a);
        var output = tuble.Item2;
        //Assert
        tuble.Item1.Should().Be(0);
        Assert.That(output, Does.Contain("public partial class Product"));
        Assert.That(output, Does.Contain("[Table")); //-t
        Assert.That(output, Does.Contain("[Key]")); //-k
        Assert.That(output, Does.Contain("[Required]"));  //-q
        Assert.That(output, Does.Contain("virtual")); //-n
    }

    [Test]
    [TestCaseSource(typeof(TestSample), nameof(TestSample.FileCases))]
    public async Task FolderMaybeNotExistTest(string url, string version, int n)
    {
        //Arrange
        var fname = @"xyz.cs";
        var a = $"-r {url} -v -f {fname}";
        //Act
        var tuble = await RunCommand(a);
        var output = tuble.Item2;
        //Assert
        tuble.Item1.Should().Be(0);
        Assert.That(output, Does.Contain("public partial class Product")); //-v

    }
    [Test]
    public async Task Enum_issue_7_test()
    {
        //Arrange
        var url = TestSample.TripPin4Flag;


        var a = $"-r {url} -v";
        //Act
        var tuble = await RunCommand(a);
        var output = tuble.Item2;

        var expected = @"
public enum Feature
	 {
 		Feature1=0,
 		Feature2=1,
 		Feature3=2,
 		Feature4=3 
	}
";
        //Assert
        Assert.That(output, Does.Match(expected.GetRegexPattern()));


    }
    [Test]
    public async Task Enum_issue_7_flag_support_test()
    {
        //Arrange
        var url = TestSample.TripPin4Flag;


        var a = $"-r {url} -v";
        //Act
        var tuble = await RunCommand(a);
        var output = tuble.Item2;
        var expected = @"
[Flags] public enum PersonGender
         {
                Male=0,
                Female=1,
                Unknow=2
        }
";
        //Assert
        Assert.That(output, Does.Match(expected.GetRegexPattern()));


    }

    [Test]
    public async Task Url_test()
    {
        //Arrange
        string url = TestSample.UrlTripPinService;
        var a = $"-r {url} -v ";
        //Act
        var tuble = await RunCommand(a);
        var output = tuble.Item2;
        //Assert
        tuble.Item1.Should().Be(0);
        Assert.That(output, Does.Contain("public partial class Trip")); //-v

    }


    [Test]
    [Category("code_header")]
    [TestCaseSource(typeof(TestSample), nameof(TestSample.FileCases))]
    [TestCaseSource(typeof(TestSample), nameof(TestSample.UrlNorthwindCases))]
    public async Task CodeHeaderTest(string url, string version, int n)
    {
        //Arrange         
        var a = $"-r {url} -v --include Category";
        //Act
        var tuble = await RunCommand(a);
        var output = tuble.Item2;
        //Assert  
        var line = $"//     Service Url: {url}";
        Assert.That(output, Does.Contain(line));

        Assert.That(output, Does.Contain("//     Parameters:"));
        Assert.That(output, Does.Contain("// 	-v Verbose= True"));
    }

    #region Name Case



    [Test]
    public async Task Entity_case_camel_change_test()
    {
        //Arrange
        string url = TestSample.TripPin4Rw;
        var a = $"-r {url} --entity-case camel -v ";
        //Act
        var tuble = await RunCommand(a);
        var output = tuble.Item2;
        //Assert
        var expected = new List<string> {
            "public partial class location",
            "public airportLocation Location {get;set;}",
            "public partial class planItem",
            "public partial class publicTransportation : planItem",
            "public List<location> AddressInfo {get;set;}",
            "public List<feature> Features {get;set;}"
        };
        foreach (var s in expected)
        {
            Assert.That(output, Does.Contain(s));
        }
    }

    [Test]
    [TestCase("--entity-case none")]
    [TestCase("--entity-case pas")]
    [TestCase("")]
    public async Task Entity_case_pas_change_test(string caseOption)
    {
        //Arrange
        string url = TestSample.TripPin4Rw;
        var a = $"-r {url}  {caseOption} -v ";
        //Act
        var tuble = await RunCommand(a);
        var output = tuble.Item2;
        //Assert
        var expected = new List<string> {
            "public partial class Location",
            "public AirportLocation Location {get;set;}",
            "public partial class PlanItem",
            "public partial class PublicTransportation : PlanItem",
            "public List<Location> AddressInfo {get;set;}",
            "public List<Feature> Features {get;set;}",
        };
        foreach (var s in expected)
        {
            Assert.That(output, Does.Contain(s));
        }
    }
    #endregion

    #region filter

    [Test]
    public async Task Model_filter_star_test()
    {
        //Arrange
        string url = TestSample.NorthWindV4;
        var a = $"-r {url} --include * -v ";
        //Act
        var tuble = await RunCommand(a);
        var output = tuble.Item2;
        //Assert
        Assert.That(output, Does.Contain("public partial class Product"));
        Assert.That(output, Does.Contain("public partial class Customer"));

    }
    [Test]
    public async Task Model_filter_q_mark_test()
    {
        //Arrange
        string url = TestSample.NorthWindV4;
        var a = $"-r {url} --include *Suppli?? -v ";
        //Act
        var tuble = await RunCommand(a);
        var output = tuble.Item2;
        //Assert
        Assert.That(output, Does.Contain("public partial class Supplier"));
    }
    [Test]
    public async Task Model_filter_namespace_star_test()
    {
        //Arrange
        string url = TestSample.NorthWindV4;
        var a = $"-r {url} --include NorthwindModel* -v ";
        //Act
        var tuble = await RunCommand(a);
        var output = tuble.Item2;
        //Assert
        Assert.That(output, Does.Contain("public partial class Supplier"));
        Assert.That(output, Does.Contain("public partial class Customer"));

    }
    [Test]
    [TestCaseSource(typeof(TestSample), nameof(TestSample.FileCases))]
    public async Task Model_filter_multi_values_for_v3_v4_test(string url, string _, int __)
    {
        //Arrange          
        var a = $"-r {url} --include *product_*  *customer_* -v ";
        //Act
        var tuble = await RunCommand(a);
        var output = tuble.Item2;

        //Assert
        Assert.That(output, Does.Contain("public partial class Current_Product_List"));
        Assert.That(output, Does.Contain("public partial class Customer_and_Suppliers_by_City"));
        Assert.That(output, Does.Contain("public partial class Product_Sales_for_1997"));
    }

    [Test]
    public async Task Model_filter_case_insensetive_test()
    {
        //Arrange
        string url = TestSample.NorthWindV4;
        var a = $"-r {url} --include *PROduCT*  -v ";
        //Act
        var tuble = await RunCommand(a);
        var output = tuble.Item2;
        //Assert

        Assert.That(output, Does.Contain("public partial class Current_Product_List"));
        Assert.That(output, Does.Contain("public partial class Product_Sales_for_1997"));

    }

    //tests for issue #29, considering class dependency
    [Test]
    public async Task Model_filter_is_considering_class_dependency_test()
    {
        //Arrange
        string url = TestSample.NorthWindV4;
        var a = $"-r {url} --include product  -v -n";
        var expected = @"
public partial class Product
public partial class Category
public partial class Order_Detail
public partial class Order
public partial class Customer
public partial class CustomerDemographic
public partial class Employee
public partial class Territory
public partial class Region
public partial class Shipper
public partial class Supplier
";
        //Act
        var tuble = await RunCommand(a);
        var output = tuble.Item2;

        //Assert
        output.ToLines().Where(s => s.StartsWith("public partial class"))
            .Should().Contain(expected.ToLines());
        output.Should().NotContain("public partial class Product_Sales_for_1997");
    }

    [Test]
    public async Task Model_filter_using_star_should_include_all_dependency_test()
    {
        //Arrange
        string url = TestSample.TripPin4;
        var a = $"-r {url} --include air*  -v ";
        var expected = @"
public partial class AirportLocation : Location
public partial class Location
public partial class City
public partial class Airline
public partial class Airport
";
        //Act
        var tuble = await RunCommand(a);
        var output = tuble.Item2;

        //Assert
        output.ToLines().Should().Contain(expected.ToLines());
    }

    #endregion

    #region readonly

    [Test]
    public async Task Model_readonly_test()
    {
        //Arrange
        string url = TestSample.TripPin4;
        var a = $"-r {url}  -v ";
        //Act
        var tuble = await RunCommand(a);
        var output = tuble.Item2;
        //Assert
        var list = new List<string>
        {
            "public int TripId {get;} //PrimaryKey not null ReadOnly" ,
            "public int PlanItemId {get;} //PrimaryKey not null ReadOnly",
            "public string AirlineCode {get;} //PrimaryKey not null ReadOnly"
        };
        output.Should().ContainAll(list);
    }

    [Test]
    //feature #41
    public async Task Model_readonly_but_ignored_by_setting_readwrite_test()
    {
        //Arrange
        string url = TestSample.TripPin4;
        var a = $"-r {url}  -v --read-write";
        //Act
        var tuble = await RunCommand(a);
        var output = tuble.Item2;
        //Assert
        var list = new List<string>
        {
            "public int TripId {get;set;} //PrimaryKey not null ReadOnly" ,
            "public int PlanItemId {get;set;} //PrimaryKey not null ReadOnly",
            "public string AirlineCode {get;set;} //PrimaryKey not null ReadOnly"
        };
        foreach (var s in list)
        {
            Assert.That(output, Does.Contain(s));
        }
    }
    #endregion

    [Test]
    [Category("record")]
    public async Task Init_only_property_test()
    {
        //Arrange
        string url = TestSample.TripPin4;
        var a = $"-r {url}  -v --init-only";
        //Act
        var tuble = await RunCommand(a);
        var output = tuble.Item2;
        //Assert
        var expected = @"
public partial class Airline
{
    public string AirlineCode {get;} //PrimaryKey not null ReadOnly    
    public string Name {get;init;} // not null    
}".ToLines();

        output.Should().ContainAll(expected);
    }
    [Test]
    [Category("record")]
    public async Task Record_type_cs9_generation_test()
    {
        //Arrange
        string url = TestSample.TripPin4;
        var a = $"-r {url}  -v --init-only -G record";
        //Act
        var tuble = await RunCommand(a);
        var output = tuble.Item2;
        //Assert
        var expected = @"
public partial record Airline
{
    public string AirlineCode {get;} //PrimaryKey not null ReadOnly    
    public string Name {get;init;} // not null    
}".ToLines();

        output.Should().ContainAll(expected);
    }

    [Test]
    [Category("record")]
    public async Task Record_type_cs9_with_navigation_has_no_virtual_members_test()
    {
        //Arrange
        string url = TestSample.TripPin4;
        var a = $"-r {url}  -v --init-only -G record -n";
        //Act
        var tuble = await RunCommand(a);
        var output = tuble.Item2;

        //Assert
        var expected = @"
public partial record Flight : PublicTransportation
	{
	    public string FlightNumber {get;init;} // not null
	    public Airport From {get;init;} 
	    public Airport To {get;init;} 
	    public Airline Airline {get;init;} 
	} 
}".ToLines();

        output.Should().ContainAll(expected);
    }
#if OPENAPI
        [Test]
        [Category("openapi")]
        public async Task OpenApi_test()
        {
            //Arrange
#if NETCOREAPP
            var fname = "test_core.yml";
#else
      var fname = "test.yml";
#endif
            string url = TestSample.TripPin4;
            var a = $"-r {url}  -v -O {fname}";
            //Act
            var tuble = await RunCommand(a);
            var output = tuble.Item2;
            //Assert
            output.Should().Contain($"Saving OpenApi Specs to file : {fname}");
            var text = File.ReadAllText(fname);
            text.Should().Contain("openapi: 3.0.1");
        }
#endif

    #region att-defs test v6.0
    [Test]
    public async Task Att_def_test()
    {
        //Arrange
        var text = """
#class attributes definitions

# applied to all classes
[map]
Scope=class
Format= [AdaptTo("[name]Dto"]

#property attribute
    [dm3]
 Format= [DataMember]
 # Filter is c# expression evaluated to boolean value. If true, the attribute is added to the property
 Filter= ClassName.In("City")

 [json33]
 # applied to all properties 
 Format= [JsonPropertyName([{{PropName.ToCamelCase().Quote()}}])]

""";
        var path = NewTempFile(text);
        string url = TestSample.UrlTripPinService;
        var a = $"-r {url} -v -a  map dm3 json33 --att-defs {path}";
        var expected = """
            // Complex Entity
            [AdaptTo("[name]Dto"]
            public partial class City
            {
                [DataMember]
                [JsonPropertyName(["name"])]
                public string Name {get;set;} 

                [DataMember]
                [JsonPropertyName(["countryRegion"])]
                public string CountryRegion {get;set;} 

                [DataMember]
                [JsonPropertyName(["region"])]
                public string Region {get;set;} 

            }

            // Complex Entity
            [AdaptTo("[name]Dto"]
            public partial class AirportLocation : Location
            {
                [JsonPropertyName(["loc"])]
                public GeographyPoint Loc {get;set;} 

            }
            """;
        //Act
        var tuple = await RunCommand(a);
        var output = tuple.Item2;
        //Assert
        output.Should().ContainAll(expected.ToLines());
    }
    #endregion

    [Test]
    public async Task MetaData_encoded_gzip_should_success_test()
    {
        //Arrange
        string url = OdataService.Trippin;
        var a = $"-r {url} -v";
        //Act
        var tuple = await RunCommand(a);
        var output = tuple.Item2;
        //Assert
        output.Should().Contain("public partial class City");
    }

    [Test]
    public async Task Show_help_test()
    {
        //Arrange       
        var a = $"--help";
        //Act
        var tuple = await RunCommand(a);
        var output = tuple.Item2;
        //Assert
        output.Should().Contain("-r, --url");
    }

}