using System;
using System.IO;
using System.Linq;
using Medallion.Shell;
using NUnit.Framework;

namespace OData2Poco.CommandLine.Test
{
    /*
     * Note for text contain check for properties of class
     * all properties are declared with one and only one space between words
     * example virtual public Supplier Supplier {get;set;}  //only one space bet all words
     * */
    [TestFixture]
    public class ProgramTests
    {
        private const double Timeout = 3 * 60; //sec
        private const string appCommand = @"o2pgen";
        static Func<string, Medallion.Shell.Command> TestCommand = (s => Medallion.Shell.Command.Run(appCommand, s.Split(' '),
             options => options.Timeout(TimeSpan.FromSeconds(Timeout))));

        /// <summary>
        /// exitcode = tuble.item1  , output= tuble.item2
        /// </summary>
        /// <remarks>
        /// Command is split  simply using space as a delimiter so does not cope 
        /// with spaces within quoted parameter values nor quotes themselves. Works well
        /// enough for unit tests.
        /// </remarks>
        private Func<string, Tuple<int, string>> RunCommand = (s =>
        {
           var command = Medallion.Shell.Command.Run(appCommand, s.Split(' '),
           options => options.Timeout(TimeSpan.FromSeconds(Timeout)));

            var outText = command.Result.StandardOutput;
            var errText = command.Result.StandardError;
            var exitCode = command.Result.ExitCode;
            Tuple<int, string> tuple = new Tuple<int, string>(exitCode, outText + "\n" + errText);
            //Console.WriteLine(tuple.Item2);
            return tuple;
        });

        [Test]
        [TestCaseSource(typeof(TestSample), "UrlCases")]
        public void DefaultSettingTest(string url, string version, int n)
        {
            var a = string.Format("-r {0} -v ", url);
            var tuble = RunCommand(a);
            var output = tuble.Item2;
            //Console.WriteLine(output);
            Assert.AreEqual(0, tuble.Item1);
            //Console.WriteLine(tuble.Item2);
            Assert.IsTrue(output.Contains("public class Product"));
            Assert.IsFalse(output.Contains("System.ComponentModel.DataAnnotations")); //-k not set
            Assert.IsFalse(output.Contains("System.ComponentModel.DataAnnotations.Schema")); //-t not set
        }

        [Test]
        [TestCaseSource(typeof(TestSample), "UrlCases")]
        public void PocoSettingTest(string url, string version, int n)
        {
            var a = string.Format("-r {0} -v -k -t -q -n -b", url);
            var tuble = RunCommand(a);
            var output = tuble.Item2;
            Assert.AreEqual(0, tuble.Item1);
          //  Console.WriteLine(tuble.Item2);
            
            Assert.IsTrue(output.Contains("public class Product"));
            Assert.IsTrue(output.Contains("[Table(\"Products\")]")); //-t
            Assert.IsTrue(output.Contains("System.ComponentModel.DataAnnotations.Schema")); //-t
            Assert.IsTrue(output.Contains("[Key]")); //-k
            Assert.IsTrue(output.Contains("System.ComponentModel.DataAnnotations")); //-k
            Assert.IsTrue(output.Contains("[Required]"));  //-q
            Assert.IsTrue(output.Contains("virtual public Supplier Supplier {get;set;}")); //-n
            Assert.IsTrue(output.Contains("int?"));  //-b
            Assert.IsFalse(output.Contains("public class Product :")); // -i is not set
        }

        [Test]
        public void PocoWithInheritanceTest()
        {
            var url = "http://services.odata.org/V4/TripPinServiceRW/" ;
            var a = $"-r {url} -v";

            var tuble = RunCommand(a);
            var output = tuble.Item2;

            Assert.IsTrue(output.Contains("public class PublicTransportation : PlanItem"));
        }

        [Test(Description = "If model inheritance is used (the default) check that the propterties of a base calsss are not duplicated inderived classes")]
        public void PropertyInheritenceTest()
        {
            var url = "http://services.odata.org/V4/TripPinServiceRW/";
            var a = $"-r {url} -v";

            var tuble = RunCommand(a);
            var output = tuble.Item2;

            var lines = output.Split('\n');
            var occurneces = lines.Count(l => l.Contains("public int PlanItemId"));

            Assert.IsTrue(occurneces == 1); // For inheritance, check that PlanItemId property only occurs in the base class

        }
        [Test]
        public void PocoWithBaseClassTest()
        {
            var url = "http://services.odata.org/V4/TripPinServiceRW/";
            const string myBaseClass = nameof(myBaseClass);

            var a = $"-r {url} -v -i {myBaseClass}";

            var tuble = RunCommand(a);
            var output = tuble.Item2;

            Assert.IsTrue(output.Contains($"public class PublicTransportation : {myBaseClass}"));

            var lines = output.Split('\n');
            var occurneces = lines.Count(l => l.Contains("public int PlanItemId"));

            Assert.IsTrue(occurneces > 1);
        }

        [Test(Description = "If model inheritance is not used, the properties from a base class should by duplicated in the derived classes.")]
        public void PropertyDuplicationTest()
        {
            var url = "http://services.odata.org/V4/TripPinServiceRW/";
            const string myBaseClass = nameof(myBaseClass);

            var a = $"-r {url} -v -i {myBaseClass}";

            var tuble = RunCommand(a);
            var output = tuble.Item2;

            var lines = output.Split('\n');
            var occurneces = lines.Count(l => l.Contains("public int PlanItemId"));

            Assert.IsTrue(occurneces > 1); // If not using model inheritance, check that the PlanItemId property is duplicated in derived classes

        }

        [Test]
        [TestCaseSource(typeof(TestSample), "UrlCases")]
        public void NullableDatatypeTest(string url, string version, int n)
        {
            var a = string.Format("-r {0} -v  -n -b", url); //navigation propertis with complex data type
            var tuble = RunCommand(a);
            var output = tuble.Item2;
            Assert.AreEqual(0, tuble.Item1);
           // Console.WriteLine(tuble.Item2);

            Assert.IsTrue(output.Contains("public class Product"));
            Assert.IsTrue(output.Contains("virtual public Supplier Supplier {get;set;}")); //-n
            Assert.IsTrue(output.Contains("int?"));  //-b
         
        }

        [Test]
        [TestCaseSource(typeof(TestSample), "UrlCases")]
        public void PocoSettingWithJsonAttributeTest(string url, string version, int n)
        {
            var a = string.Format("-r {0}  -j -v", url);
            var tuble = RunCommand(a);
            var output = tuble.Item2;
            //Console.WriteLine(tuble.Item2);
            Assert.AreEqual(0, tuble.Item1); //exit code
            Assert.IsTrue(output.Contains("public class Category"));
            Assert.IsTrue(output.Contains("[JsonProperty(PropertyName = \"CategoryID\")]"));
            Assert.IsTrue(output.Contains("Category"));
            Assert.IsTrue(output.Contains(" [JsonProperty(PropertyName = \"CategoryName\")]"));
            Assert.IsTrue(output.Contains("CategoryName"));

        }

        [Test]
        [TestCaseSource(typeof(TestSample), "UrlCases")]
        public void PocoSettingWithJsonAttributeAndCamelCaseTest(string url, string version, int n)
        {
            var a = string.Format("-r {0}  -j -c cam -v", url);
            var tuble = RunCommand(a);
            var output = tuble.Item2;
            //Assert.AreEqual(0, tuble.Item1);
          //  Console.WriteLine(tuble.Item2);
            Assert.IsTrue(output.Contains("public class Category"));
            Assert.IsTrue(output.Contains("[JsonProperty(PropertyName = \"CategoryName\")]"),"itshould be CategoryName");
            Assert.IsTrue(output.Contains("categoryName"));
            Assert.IsTrue(output.Contains("[JsonProperty(PropertyName = \"CategoryID\")]"));
            Assert.IsTrue(output.Contains("category"));
         

        }

        [Test]
        [TestCaseSource(typeof(TestSample), "UrlCases")]
        public void PocoSettingWithJsonAttributePasCaseTest(string url, string version, int n)
        {
            var a = string.Format("-r {0}  -jc PAS -v", url);
            var tuble = RunCommand(a);
            var output = tuble.Item2;
            Assert.AreEqual(0, tuble.Item1);
            //Console.WriteLine(tuble.Item2);

            Assert.IsTrue(output.Contains("[JsonProperty(PropertyName = \"CategoryID\")]"));
            Assert.IsTrue(output.Contains("Category"));
            Assert.IsTrue(output.Contains(" [JsonProperty(PropertyName = \"CategoryName\")]"));
            Assert.IsTrue(output.Contains("CategoryName"));

        }
       
       
        [Test]
        [TestCaseSource(typeof(TestSample), "UrlCases")]
        public void PocoSettingEagerTest(string url, string version, int n)
        {
            var a = string.Format("-r {0} -v -e", url);
            var tuble = RunCommand(a);
            var output = tuble.Item2;
            Assert.AreEqual(0, tuble.Item1);
          //  Console.WriteLine(tuble.Item2);
            
            Assert.IsTrue(output.Contains("public class Product")); //-v
            Assert.IsTrue(output.Contains("public Supplier Supplier {get;set;}")); //-e
        
        }

        [Test]
        [TestCaseSource(typeof(TestSample), "UrlCases")]
        public void PocoSettingInheritTest(string url, string version, int n)
        {
            var a = string.Format("-r {0} -v -i {1}", url,"MyBaseClass,MyInterface");
            var tuble = RunCommand(a);
            var output = tuble.Item2;
            Assert.AreEqual(0, tuble.Item1);
            //  Console.WriteLine(tuble.Item2);
           
            Assert.IsTrue(output.Contains("public class Product : MyBaseClass,MyInterface")); //-i, -v

        }

        [Test]
        [TestCaseSource(typeof(TestSample), "UrlCases")]
        public void PocoSettingNamespaceTest(string url, string version, int n)
        {
            var a = string.Format("-r {0} -v -m {1}", url, "MyNamespace1.MyNamespace2");
            var tuble = RunCommand(a);
            var output = tuble.Item2;
            Assert.AreEqual(0, tuble.Item1);
            //  Console.WriteLine(tuble.Item2);

            Assert.IsTrue(output.Contains("MyNamespace1.MyNamespace2.")); //-m, -v

        }

        [Test]
        [TestCaseSource(typeof(TestSample), "UrlCases")]
        public void FileDefaultSettingTest(string url, string version, int n)
        {
            var a = string.Format("-r {0} -v ", url);
            var tuble = RunCommand(a);
            var output = tuble.Item2;
           
            Assert.AreEqual(0, tuble.Item1);
            //Console.WriteLine(tuble.Item2);
            Assert.IsTrue(output.Contains("public class Product"));
        }

        [Test]
        [TestCaseSource(typeof(TestSample), nameof(TestSample.FileCases))]
        public void FileWithSettingTest(string url, string version, int n)
        {
            var a = string.Format("-r {0} -v -k -t -q -n", url);
            var tuble = RunCommand(a);
            var output = tuble.Item2;
            Assert.AreEqual(0, tuble.Item1);
            Assert.IsTrue(output.Contains("public class Product"));
            Assert.IsTrue(output.Contains("[Table")); //-t
            Assert.IsTrue(output.Contains("[Key]")); //-k
            Assert.IsTrue(output.Contains("[Required]"));  //-q
            Assert.IsTrue(output.Contains("virtual")); //-n
        }

        [Test]
        public void ShowHelpIfNoArgumentsTest()
        {
            var a = "";
            var tuble = RunCommand(a);
            var output = tuble.Item2;
            var expectedExitCode = (int)ExitCodes.ArgumentsInvalid;
            Assert.AreEqual(expectedExitCode, tuble.Item1);
            //var help = File.ReadAllText("help.txt");
            Assert.IsTrue(output.Contains("-r, --url"));
        }




        [Test]
        [TestCaseSource(typeof(TestSample), "FileCases")]
        public void FolderMaybeNotExistTest(string url, string version, int n)
        {
            var fname = @"xyz.cs";
            var a = string.Format("-r {0} -v -f {1}", url, fname);
            var tuble = RunCommand(a);
            var output = tuble.Item2;
            //Console.WriteLine(output);
            Assert.AreEqual(0, tuble.Item1);
            Assert.IsTrue(output.Contains("public class Product")); //-v

        }


        [Test]
        [TestCase(@"http://invalid-url.com")]
        [TestCase(@"http://www.google.com")] //not odata service
        public void InvalidUrlTest(string url)
        {
            var a = String.Format("-r {0} -vdl", url);
            var tuble = RunCommand(a);
            var output = tuble.Item2;
            //Console.WriteLine(output);
            Assert.AreNotEqual(0, tuble.Item1);
            Assert.IsTrue(output.Contains("Error in executing the command"));

        }


        [Test]
        [TestCaseSource(typeof(TestSample), "UrlCases")]
        public void InValidArgumentTest(string url, string version, int n)
        {
            var a = string.Format("-r {0} -z ", url); //-z invalid argument
            var result = RunCommand(a);
            var expectedExitCode = (int)ExitCodes.ArgumentsInvalid;
            Assert.AreEqual(expectedExitCode, result.Item1);
            Assert.IsTrue(result.Item2.Contains("-r, --url"));
        }

    }
}
