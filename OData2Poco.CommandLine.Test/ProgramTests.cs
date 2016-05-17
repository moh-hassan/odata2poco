using System;
using System.Collections.Generic;
using System.IO;
using Medallion.Shell;
using NUnit.Framework;

// The remote server returned an error: (401) Unauthorized.
// The remote server returned an error: (404) Not Found.

namespace OData2Poco.CommandLine.Test
{

    [TestFixture]
    public class ProgramTests
    {
        private const double Timeout = 3*60; //sec
        const string UrlV4 = "http://services.odata.org/V4/Northwind/Northwind.svc";
        const string UrlV3 = "http://services.odata.org/V3/Northwind/Northwind.svc";
        
        static Func<string, Command> TestCommand = (s => Command.Run("o2pgen", s.Split(' '),
             options => options.Timeout(TimeSpan.FromSeconds(Timeout))));

        /// <summary>
        /// exitcode = tuble.item1  , output= tuble.item2
        /// </summary>
        private Func<string, Tuple<int, string>> RunCommand = (s =>
      {
          var command = Command.Run("o2pgen", s.Split(' '),
           options => options.Timeout(TimeSpan.FromSeconds(Timeout)));
          //var command = TestCommand(s);
          // in an async method, we could use var result = await command.Task;
          var outText = command.Result.StandardOutput;
          var errText = command.Result.StandardError;
          var exitCode = command.Result.ExitCode;
          //Console.WriteLine(outText);
          //   Assert.AreEqual(0, exitCode);
          Tuple<int, string> tuple = new Tuple<int, string>(exitCode, outText + "\n" + errText);
          //Console.WriteLine(tuple.Item2);
          return tuple;
      });

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
        [TestCase(UrlV4)]
        [TestCase(UrlV3)]
        [TestCase(@"data\northwindv4.xml")]
        [TestCase(@"data\northwindv3.xml")]
        public void AllArgumentTest(string url)
        {
            var a = string.Format("-r {0} -d -l -v -m meta.xml -f north.cs ", url);
            var tuble = RunCommand(a);
            var output = tuble.Item2;
            Assert.AreEqual(0, tuble.Item1);
            Console.WriteLine(tuble.Item2);
            Assert.IsTrue(output.Contains("Saving generated code to file : north.cs")); //-f -r
            Assert.IsTrue(output.Contains("HTTP Header")); //-d
            Assert.IsTrue(output.Contains("POCO classes")); //-l
            Assert.IsTrue(output.Contains("Saving Metadata to file : meta.xml")); //-m
            Assert.IsTrue(output.Contains("public class Product")); //-v

        }
        [Test]
        [TestCase(UrlV4)]
        [TestCase(UrlV3)]
        //-r
        public void StartWith_r_ArgumentTest(string url)
        {
            var a = string.Format("-r {0}", url);
            var tuble = RunCommand(a);
            var output = tuble.Item2;
            Assert.AreEqual(0, tuble.Item1);

            Assert.IsTrue(output.Contains("Saving generated code to file : poco.cs"));
        }

        [Test]
        [TestCase(UrlV4)]
        [TestCase(UrlV3)]
        //-r
        public void StartWith_r_u_p_ArgumentTest(string url)
        {
            var a = string.Format("-r {0} -ux -py", url);
            var tuble = RunCommand(a);
            var output = tuble.Item2;
            Assert.AreEqual(0, tuble.Item1);
            Assert.IsTrue(output.Contains("Saving generated code to file : poco.cs"));
        }



        [Test]
        [TestCase(@"http://invalid-url.com")]
        [TestCase(@"http://www.google.com")] //not odata service
        public void InvalidUrlTest(string url)
        {
            var a = String.Format("-r {0}", url);
            var tuble = RunCommand(a);
            var output = tuble.Item2;
            Assert.AreNotEqual(0, tuble.Item1);
            Assert.IsTrue(output.Contains("Error in executing the command"));

        }


        [Test]
        [TestCase(UrlV4)]
        public void InValidArgumentTest(string url)
        {
            var a = string.Format("-r {0} -z ", url); //-z invalid argument
            var result = RunCommand(a);
            Assert.AreEqual(0, result.Item1);
            Assert.IsTrue(result.Item2.Contains("-r, --url"));
        }

        [Test]
        [TestCase(@"data\northwindv4.xml", 0)]
        [TestCase(@"data\northwindv3.xml", 0)]
        [TestCase(@"data\invalidxml.xml", -1)]
        public void FileReadingTest(string url, int exitCode)
        {
            var a = string.Format("-r {0} -d -l -v -m meta.xml -f north.cs  ", url);
            var tuble = RunCommand(a);
            var output = tuble.Item2;
            Assert.AreEqual(exitCode, tuble.Item1);

        }

        [Test]
        [TestCase(UrlV4)]
        [TestCase(UrlV3)]
        [TestCase("http://localhost/odata2/api/northwind")] //not authorized
        [TestCase("http://localhost/odata20/api/northwind")] //not found
        public void NotHangingInCaseNoConnectionTest(string url)
        {
            var expected = new List<int> { 0, -1 }; //-1 is valid exitcode if there's no connection to internet
            var actual = 5;
            var a = string.Format("-r {0} -dlv -m meta.xml -fnorth.cs  ", url);
            var result = RunCommand(a);

            CollectionAssert.Contains(expected, result.Item1);

        }
       

    }//
}//
