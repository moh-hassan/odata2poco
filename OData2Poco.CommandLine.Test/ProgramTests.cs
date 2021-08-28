
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using NUnit.Framework;
using OData2Poco.TestUtility;


namespace OData2Poco.CommandLine.Test
{

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

        //[Test]

        private async Task Parameter_file_Test()
        {
            //Arrange
            var a = "--env-file graph_param.txt -r {{url}} --token-endpoint {{token_endpoint}}  --token-params client_id={{client_id}}&client_secret={{client_secret}}&grant_type={{grant_type}}&resource={{resource}} ";
            //Act
            var tuble = await RunCommand(a);
            var output = tuble.Item2;
            Console.WriteLine(output);
            //Assert
            Assert.AreEqual(0, tuble.Item1);

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
            Assert.AreEqual(0, tuble.Item1);
            Assert.IsTrue(output.Contains("public partial class Product"));
            Assert.IsFalse(output.Contains("System.ComponentModel.DataAnnotations")); //-a key /or -k not set
            Assert.IsFalse(output.Contains("System.ComponentModel.DataAnnotations.Schema")); //a- tab / or -t not set
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
            Assert.AreEqual(0, tuple.Item1);

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
            Assert.IsTrue(output.Contains("public partial class PublicTransportation : PlanItem"));
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

            Assert.IsTrue(output.Contains($"public partial class PublicTransportation : {myBaseClass}"));

            var lines = output.Split('\n');
            var occurneces = lines.Count(l => l.Contains("public int PlanItemId"));
            //Assert
            Assert.IsTrue(occurneces > 1);
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
            Assert.IsTrue(occurneces > 1); // If not using model inheritance, check that the PlanItemId property is duplicated in derived classes

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
            Assert.AreEqual(0, tuble.Item1);
            Assert.IsTrue(output.Contains("public partial class Product"));
            Assert.IsTrue(output.Contains("public virtual Supplier Supplier {get;set;}")); //-n
            Assert.IsTrue(output.Contains("int?"));  //-b

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
            Assert.AreEqual(0, tuble.Item1); //exit code
            Assert.IsTrue(output.Contains("public partial class Category"));
            Assert.IsTrue(output.Contains("[JsonProperty(PropertyName = \"CategoryID\")]"));
            Assert.IsTrue(output.Contains("Category"));
            Assert.IsTrue(output.Contains(" [JsonProperty(PropertyName = \"CategoryName\")]"));
            Assert.IsTrue(output.Contains("CategoryName"));
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
            Assert.AreEqual(0, tuble.Item1); //exit code
            Assert.IsTrue(output.Contains("public partial class Category"));
            Assert.IsTrue(output.Contains("[JsonPropertyName(\"CategoryID\")]"));
            Assert.IsTrue(output.Contains("Category"));
            Assert.IsTrue(output.Contains(" [JsonPropertyName(\"CategoryName\")]"));
            Assert.IsTrue(output.Contains("CategoryName"));
        }
        [Test]
        [TestCaseSource(typeof(TestSample), nameof(TestSample.FileCases))]
        public async Task PocoSettingWithJsonAttributeAndCamelCaseTest(string url, string version, int n)
        {
            //Arrange
            var a = $"-r {url}  -j -c camel -v";
            //Act
            var tuble = await RunCommand(a);
            var output = tuble.Item2;
            //Assert
            Assert.IsTrue(output.Contains("public partial class Category"));
            Assert.IsTrue(output.Contains("[JsonProperty(PropertyName = \"CategoryName\")]"), "itshould be CategoryName");
            Assert.IsTrue(output.Contains("categoryName"));
            Assert.IsTrue(output.Contains("[JsonProperty(PropertyName = \"CategoryID\")]"));
            Assert.IsTrue(output.Contains("category"));
        }

        [Test]
        [TestCaseSource(typeof(TestSample), nameof(TestSample.FileCases))]
        public async Task PocoSettingWithJsonAttributePasCaseTest(string url, string version, int n)
        {
            //Arrange
            var a = $"-r {url}  -jc PAS -v";
            //Act
            var tuble = await RunCommand(a);
            var output = tuble.Item2;
            //Assert
            Assert.AreEqual(0, tuble.Item1);
            Assert.IsTrue(output.Contains("[JsonProperty(PropertyName = \"CategoryID\")]"));
            Assert.IsTrue(output.Contains("Category"));
            Assert.IsTrue(output.Contains(" [JsonProperty(PropertyName = \"CategoryName\")]"));
            Assert.IsTrue(output.Contains("CategoryName"));

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
            Assert.AreEqual(0, tuble.Item1);
            //Assert
            Assert.IsTrue(output.Contains("public partial class Product")); //-v
            Assert.IsTrue(output.Contains("public Supplier Supplier {get;set;}")); //-e

        }

        [Test]
        [TestCaseSource(typeof(TestSample), nameof(TestSample.FileCases))]
        public async Task PocoSettingInheritTest(string url, string version, int n)
        {
            //Arrange
            var a = string.Format("-r {0} -v -i {1}", url, "MyBaseClass,MyInterface");
            //Act
            var tuble = await RunCommand(a);
            var output = tuble.Item2;
            //Assert
            Assert.AreEqual(0, tuble.Item1);
            Assert.IsTrue(output.Contains("public partial class Product : MyBaseClass,MyInterface")); //-i, -v

        }

        [Test]
        [TestCaseSource(typeof(TestSample), nameof(TestSample.FileCases))]
        public async Task PocoSettingNamespaceTest(string url, string version, int n)
        {
            var a = $"-r {url} -v -m {"MyNamespace1.MyNamespace2"}";
            //Act
            var tuble = await RunCommand(a);
            var output = tuble.Item2;
            //Assert
            Assert.AreEqual(0, tuble.Item1);
            Assert.IsTrue(output.Contains("MyNamespace1.MyNamespace2.")); //-m, -v

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
            Assert.AreEqual(0, tuble.Item1);
            Assert.IsTrue(output.Contains("public partial class Product"));
            Assert.IsTrue(output.Contains("[Table")); //-t
            Assert.IsTrue(output.Contains("[Key]")); //-k
            Assert.IsTrue(output.Contains("[Required]"));  //-q
            Assert.IsTrue(output.Contains("virtual")); //-n
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
            Assert.AreEqual(0, tuble.Item1);
            Assert.IsTrue(output.Contains("public partial class Product")); //-v

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
        [TestCaseSource(typeof(TestSample), nameof(TestSample.UrlNorthwindCases))]
        //[TestCaseSource(typeof(TestSample), nameof(TestSample.UrlOdatadCases))]
        public async Task Url_test(string url, string version, int n)
        {
            //Arrange

            var a = $"-r {url} -v ";
            //Act
            var tuble = await RunCommand(a);
            var output = tuble.Item2;
            //Assert
            Assert.AreEqual(0, tuble.Item1);
            Assert.IsTrue(output.Contains("public partial class Product")); //-v

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
            Assert.IsTrue(output.Contains(line));

            Assert.IsTrue(output.Contains("//     Parameters:"));
            Assert.IsTrue(output.Contains("// 	-v Verbose= True"));           
        }

        #region Name Case



        [Test]
        public async Task Entity_case_camel_change_test()
        {
            //Arrange
            string url = TestSample.TripPin4RW;
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
                Assert.IsTrue(output.Contains(s));
            }
        }

        [Test]
        [TestCase("--entity-case none")]
        [TestCase("--entity-case pas")]
        [TestCase("")]
        public async Task Entity_case_pas_change_test(string caseOption)
        {
            //Arrange
            string url = TestSample.TripPin4RW;
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
                Assert.IsTrue(output.Contains(s));
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
            Assert.IsTrue(output.Contains("public partial class Product"));
            Assert.IsTrue(output.Contains("public partial class Customer"));

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
            Assert.IsTrue(output.Contains("public partial class Supplier"));
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
            Assert.IsTrue(output.Contains("public partial class Supplier"));
            Assert.IsTrue(output.Contains("public partial class Customer"));

        }
        [Test]
        public async Task Model_filter_multi_values_test()
        {
            //Arrange
            string url = TestSample.NorthWindV4;
            var a = $"-r {url} --include *product_*  *customer_* -v ";
            //Act
            var tuble = await RunCommand(a);
            var output = tuble.Item2;
            //Assert
            Assert.IsTrue(output.Contains("public partial class Current_Product_List"));
            Assert.IsTrue(output.Contains("public partial class Customer_and_Suppliers_by_City"));
            Assert.IsTrue(output.Contains("public partial class Product_Sales_for_1997"));
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

            Assert.IsTrue(output.Contains("public partial class Current_Product_List"));
            Assert.IsTrue(output.Contains("public partial class Product_Sales_for_1997"));

        }

        [Test]
        public async Task Model_filter_is_auto_prefixed_by_star_test()
        {
            //Arrange
            string url = TestSample.NorthWindV4;
            var a = $"-r {url} --include product  -v "; //like *product
            //Act
            var tuble = await RunCommand(a);
            var output = tuble.Item2;
            //Assert

            Assert.IsTrue(output.Contains("public partial class Product"));
            Assert.IsTrue(!output.Contains("public partial class Product_Sales_for_1997"));
        }
        [Test]
        public async Task Model_filter_is_auto_prefixed_by_star_online_test()
        {
            //Arrange
            string url = TestSample.UrlTripPinService;
            // string url = TestSample.UrlNorthWindV4; 
            var a = $"-r {url} --include air*  -v "; //like *product
            //Act
            var tuble = await RunCommand(a);
            var output = tuble.Item2;
            //Assert
            Assert.IsTrue(output.Contains("public partial class AirportLocation"));
            Assert.IsTrue(output.Contains("public partial class Airline"));
            Assert.IsTrue(output.Contains("public partial class Airport"));
            Assert.IsTrue(!output.Contains("public partial class City"));
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
            foreach (var s in list)
            {
                Assert.IsTrue(output.Contains(s));
            }
        }

        #endregion
    }

}

