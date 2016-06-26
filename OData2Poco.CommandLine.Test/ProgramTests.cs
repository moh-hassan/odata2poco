using System;
using System.Diagnostics;
using Medallion.Shell;
using NUnit.Framework;

namespace OData2Poco.CommandLine.Test
{

    [TestFixture]
    public class ProgramTests
    {
        private const double Timeout = 3 * 60; //sec

        static Func<string, Command> TestCommand = (s => Command.Run("o2pgen", s.Split(' '),
             options => options.Timeout(TimeSpan.FromSeconds(Timeout))));

        /// <summary>
        /// exitcode = tuble.item1  , output= tuble.item2
        /// </summary>
        private Func<string, Tuple<int, string>> RunCommand = (s =>
      {
          var command = Command.Run("o2pgen", s.Split(' '),
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
            var a = string.Format("-r {0}  ", url);
            var tuble = RunCommand(a);
            var output = tuble.Item2;
            Debug.WriteLine(output);
            Assert.AreEqual(0, tuble.Item1);
            //Console.WriteLine(tuble.Item2);
            Assert.IsTrue(output.Contains("public class Product"));
        }

        [Test]
        [TestCaseSource(typeof(TestSample), "UrlCases")]
        public void PocoSettingTest(string url, string version, int n)
        {
            var a = string.Format("-r {0} -v -k -t -q -n -b", url);
            var tuble = RunCommand(a);
            var output = tuble.Item2;
            Assert.AreEqual(0, tuble.Item1);
            Console.WriteLine(tuble.Item2);

            Assert.IsTrue(output.Contains("public class Product"));
            Assert.IsTrue(output.Contains("[Table(\"Products\")]")); //-t
            Assert.IsTrue(output.Contains("[Key]")); //-k
            Assert.IsTrue(output.Contains("[Required]"));  //-q
            Assert.IsTrue(output.Contains("virtual public Supplier Supplier  {get;set;}")); //-n
            Assert.IsTrue(output.Contains("int?"));  //-b
        }

        [Test]
        [TestCaseSource(typeof(TestSample), "UrlCases")]
        public void FileDefaultSettingTest(string url, string version, int n)
        {
            var a = string.Format("-r {0} -v ", url);
            var tuble = RunCommand(a);
            var output = tuble.Item2;
            //Debug.WriteLine(output);
            Assert.AreEqual(0, tuble.Item1);
            //Console.WriteLine(tuble.Item2);
            Assert.IsTrue(output.Contains("public class Product"));
        }

        [Test]
        [TestCaseSource(typeof(TestSample), "FileCases")]
        public void FileWithSettingTest(string url, string version, int n)
        {
            var a = string.Format("-r {0} -v -k -t -q -n", url);
            var tuble = RunCommand(a);
            var output = tuble.Item2;
            Assert.AreEqual(0, tuble.Item1);
            //Console.WriteLine(tuble.Item2);

            Assert.IsTrue(output.Contains("public class Product"));
            Assert.IsTrue(output.Contains("[Table")); //-t
            Assert.IsTrue(output.Contains("[Key]")); //-k
            Assert.IsTrue(output.Contains("[Required]"));  //-q
            Assert.IsTrue(output.Contains("virtual public")); //-n
        }

        [Test]
        public void ShowHelpIfNoArgumentsTest()
        {
            var a = "";
            var tuble = RunCommand(a);
            var output = tuble.Item2;
            Assert.AreEqual(0, tuble.Item1);
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
            Assert.AreEqual(0, result.Item1);
            Assert.IsTrue(result.Item2.Contains("-r, --url"));
        }



        [Test]
        [TestCase(@"data\not_exist_file.xml", -1)]
        public void FileNotExistReadingTest(string url, int exitCode)
        {
            //   var a = string.Format("-r {0} -d -l -v -m meta.xml -f north.cs  ", url);
            var a = string.Format("-r {0} -vld -m meta.xml -f north.cs  ", url);
            var tuble = RunCommand(a);
            var output = tuble.Item2;
            //Console.WriteLine(output);
            Assert.AreEqual(exitCode, tuble.Item1);

        }



    }//
}//
