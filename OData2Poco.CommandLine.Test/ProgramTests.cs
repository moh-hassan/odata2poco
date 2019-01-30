
using System;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;

namespace OData2Poco.CommandLine.Test
{
    public enum ExitCodes
    {
        Success = 0,
        ArgumentsInvalid = -1,
        HandledException = -2,
        UnhandledException = -99,
    }


    [TestFixture]
    public class ProgramTests : BaseTest
    {
        private static string WorkingDirectory = ".";
        private const double Timeout = 3 * 60; //sec

        private const string appCommand = @"o2pgen";

        [OneTimeSetUp]
        public void SetupOneTime()
        {
            Environment.CurrentDirectory = TestContext.CurrentContext.TestDirectory;
            WorkingDirectory = Environment.CurrentDirectory;
        }

        private async Task<Tuple<int, string>> RunCommand(string s)
        {
            Program.Logger.Silent = true;
            string[] args = s.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            var retcode = await Program.RunOptionsAsync(args);
            string help1 = Program.Logger.Output.ToString();

            var outText = help1;
            var errText = "";
            var exitCode = retcode;
            Tuple<int, string> tuple = new Tuple<int, string>(exitCode, outText + "\n" + errText);

            return tuple;
        }

        [Test]
        [TestCaseSource(typeof(TestSample), "UrlCases")]
       public async Task DefaultSettingTest(string url, string version, int n)
        {
            var a = $"-r {url} -v";
            var tuble = await RunCommand(a);
       
            var output = tuble.Item2;
            Assert.AreEqual(0, tuble.Item1);
            Assert.IsTrue(output.Contains("public class Product"));
            Assert.IsFalse(output.Contains("System.ComponentModel.DataAnnotations")); //-a key /or -k not set
            Assert.IsFalse(output.Contains("System.ComponentModel.DataAnnotations.Schema")); //a- tab / or -t not set
        }

        [Test]
        [TestCaseSource(typeof(TestSample), "UrlCases")]
        public async Task PocoSettingTest(string url, string version, int n)
        {
            var a = $"-r {url} -v -a key tab req -n -b";
            var tuble = await RunCommand(a);
            var output = tuble.Item2;
            //Assert
            Assert.AreEqual(0, tuble.Item1);

            Assert.IsTrue(output.Contains("public class Product"));
            Assert.IsTrue(output.Contains("[Table(\"Products\")]")); //-a tab/ -t
            Assert.IsTrue(output.Contains("System.ComponentModel.DataAnnotations.Schema")); //-a tab /-t
            Assert.IsTrue(output.Contains("[Key]")); //-k
            Assert.IsTrue(output.Contains("System.ComponentModel.DataAnnotations")); //-a key /-k
            Assert.IsTrue(output.Contains("[Required]"));  //-q
            Assert.IsTrue(output.Contains("virtual public Supplier Supplier {get;set;}")); //-n
            Assert.IsTrue(output.Contains("int?"));  //-b
            //Assert.IsFalse(output.Contains("public class Product :")); // -i is not set
        }
 
        [Test]
        public async Task PocoWithInheritanceTest()
        {
            var url = "http://services.odata.org/V4/TripPinServiceRW/";
            var a = $"-r {url} -v";

            var tuble = await RunCommand(a);
            var output = tuble.Item2;

            Assert.IsTrue(output.Contains("public class PublicTransportation : PlanItem"));
        }

        [Test(Description = "If model inheritance is used (the default) check that the propterties of a base calsss are not duplicated inderived classes")]
        public async Task PropertyInheritenceTest()
        {
            var url = "http://services.odata.org/V4/TripPinServiceRW/";
            var a = $"-r {url} -v";

            var tuble = await RunCommand(a);
            var output = tuble.Item2;
          
            var lines = output.Split('\n');
            var occurneces = lines.Count(l => l.Contains("public int PlanItemId"));

           // Assert.IsTrue(occurneces == 1); // For inheritance, check that PlanItemId property only occurs in the base class
            Assert.That(occurneces,Is.EqualTo(1));

        }
        [Test]
        public async Task PocoWithBaseClassTest()
        {
            var url = "http://services.odata.org/V4/TripPinServiceRW/";
            const string myBaseClass = nameof(myBaseClass);

            var a = $"-r {url} -v -i {myBaseClass}";

            var tuble = await RunCommand(a);
            var output = tuble.Item2;

            Assert.IsTrue(output.Contains($"public class PublicTransportation : {myBaseClass}"));

            var lines = output.Split('\n');
            var occurneces = lines.Count(l => l.Contains("public int PlanItemId"));

            Assert.IsTrue(occurneces > 1);
        }

        [Test(Description = "If model inheritance is not used, the properties from a base class should by duplicated in the derived classes.")]
        public async Task PropertyDuplicationTest()
        {
            var url = "http://services.odata.org/V4/TripPinServiceRW/";
            const string myBaseClass = nameof(myBaseClass);

            var a = $"-r {url} -v -i {myBaseClass}";

            var tuble = await RunCommand(a);
            var output = tuble.Item2;
            var lines = output.Split('\n');
            var occurneces = lines.Count(l => l.Contains("public int PlanItemId"));
            Assert.IsTrue(occurneces > 1); // If not using model inheritance, check that the PlanItemId property is duplicated in derived classes

        }

        [Test]
        [TestCaseSource(typeof(TestSample), "UrlCases")]
        public async Task NullableDatatypeTest(string url, string version, int n)
        {
            //Arrange
            var a = $"-r {url} -v  -n -b"; //navigation propertis with complex data type
            //Act
            var tuble = await RunCommand(a);
            //Assert
            var output = tuble.Item2;
            Assert.AreEqual(0, tuble.Item1);
            Assert.IsTrue(output.Contains("public class Product"));
            Assert.IsTrue(output.Contains("virtual public Supplier Supplier {get;set;}")); //-n
            Assert.IsTrue(output.Contains("int?"));  //-b

        }

        [Test]
        [TestCaseSource(typeof(TestSample), "UrlCases")]
        public async Task PocoSettingWithJsonAttributeTest(string url, string version, int n)
        {
            var a = $"-r {url}  -j -v";
            var tuble = await RunCommand(a);
            var output = tuble.Item2;
            Assert.AreEqual(0, tuble.Item1); //exit code
            Assert.IsTrue(output.Contains("public class Category"));
            Assert.IsTrue(output.Contains("[JsonProperty(PropertyName = \"CategoryID\")]"));
            Assert.IsTrue(output.Contains("Category"));
            Assert.IsTrue(output.Contains(" [JsonProperty(PropertyName = \"CategoryName\")]"));
            Assert.IsTrue(output.Contains("CategoryName"));

        }

        [Test]
        [TestCaseSource(typeof(TestSample), "UrlCases")]
        public async Task PocoSettingWithJsonAttributeAndCamelCaseTest(string url, string version, int n)
        {
            var a = $"-r {url}  -j -c cam -v";
            var tuble = await RunCommand(a);
            var output = tuble.Item2;
            Assert.IsTrue(output.Contains("public class Category"));
            Assert.IsTrue(output.Contains("[JsonProperty(PropertyName = \"CategoryName\")]"), "itshould be CategoryName");
            Assert.IsTrue(output.Contains("categoryName"));
            Assert.IsTrue(output.Contains("[JsonProperty(PropertyName = \"CategoryID\")]"));
            Assert.IsTrue(output.Contains("category"));


        }

        [Test]
        [TestCaseSource(typeof(TestSample), "UrlCases")]
        public async Task PocoSettingWithJsonAttributePasCaseTest(string url, string version, int n)
        {
            var a = $"-r {url}  -jc PAS -v";
            var tuble = await RunCommand(a);
            var output = tuble.Item2;
            Assert.AreEqual(0, tuble.Item1);

            Assert.IsTrue(output.Contains("[JsonProperty(PropertyName = \"CategoryID\")]"));
            Assert.IsTrue(output.Contains("Category"));
            Assert.IsTrue(output.Contains(" [JsonProperty(PropertyName = \"CategoryName\")]"));
            Assert.IsTrue(output.Contains("CategoryName"));

        }


        [Test]
        [TestCaseSource(typeof(TestSample), "UrlCases")]
        public async Task PocoSettingEagerTest(string url, string version, int n)
        {
            var a = $"-r {url} -v -e";
            var tuble = await RunCommand(a);
            var output = tuble.Item2;
            Assert.AreEqual(0, tuble.Item1);
            //Assert
            Assert.IsTrue(output.Contains("public class Product")); //-v
            Assert.IsTrue(output.Contains("public Supplier Supplier {get;set;}")); //-e

        }

        [Test]
        [TestCaseSource(typeof(TestSample), "UrlCases")]
        public async Task PocoSettingInheritTest(string url, string version, int n)
        {
            var a = string.Format("-r {0} -v -i {1}", url, "MyBaseClass,MyInterface");
            var tuble = await RunCommand(a);
            var output = tuble.Item2;
            Assert.AreEqual(0, tuble.Item1);
          Assert.IsTrue(output.Contains("public class Product : MyBaseClass,MyInterface")); //-i, -v

        }

        [Test]
        [TestCaseSource(typeof(TestSample), nameof(TestSample.UrlCases))]
        public async Task PocoSettingNamespaceTest(string url, string version, int n)
        {
            var a = $"-r {url} -v -m {"MyNamespace1.MyNamespace2"}";
            var tuble = await RunCommand(a);
            var output = tuble.Item2;
            Assert.AreEqual(0, tuble.Item1);
           Assert.IsTrue(output.Contains("MyNamespace1.MyNamespace2.")); //-m, -v

        }

        [Test]
        [TestCaseSource(typeof(TestSample), "UrlCases")]
        public async Task FileDefaultSettingTest(string url, string version, int n)
        {
            var a = $"-r {url} -v ";
            var tuble = await RunCommand(a);
            var output = tuble.Item2;

            Assert.AreEqual(0, tuble.Item1);
           
            Assert.IsTrue(output.Contains("public class Product"));
        }

        [Test]
        [TestCaseSource(typeof(TestSample), nameof(TestSample.FileCases))]
        public async Task FileWithSettingTest(string url, string version, int n)
        {
           
            var a = $"-r {url} -v -a key tab req -n";
            var tuble = await RunCommand(a);
            var output = tuble.Item2;
            Assert.AreEqual(0, tuble.Item1);
            Assert.IsTrue(output.Contains("public class Product"));
            Assert.IsTrue(output.Contains("[Table")); //-t
            Assert.IsTrue(output.Contains("[Key]")); //-k
            Assert.IsTrue(output.Contains("[Required]"));  //-q
            Assert.IsTrue(output.Contains("virtual")); //-n
        }

      




        [Test]
        [TestCaseSource(typeof(TestSample), "FileCases")]
        public async Task FolderMaybeNotExistTest(string url, string version, int n)
        {
            var fname = @"xyz.cs";
            var a = $"-r {url} -v -f {fname}";
            var tuble = await RunCommand(a);
            var output = tuble.Item2;
           
            Assert.AreEqual(0, tuble.Item1);
            Assert.IsTrue(output.Contains("public class Product")); //-v

        }

    }

}

