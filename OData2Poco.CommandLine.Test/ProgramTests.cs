//#define v3
using System;
using System.Collections.Generic;
using System.IO;
using Medallion.Shell;
using NUnit.Framework;

namespace OData2Poco.CommandLine.Test
{

    [TestFixture]
    public class ProgramTests
    {
        private const double Timeout = 20; //sec
#if v3
        private string Url = "http://services.odata.org/V3/OData/OData.svc";
#else
        private string Url = "http://services.odata.org/V4/OData/OData.svc";
        //private string Url = "http://localhost/odata2/api/northwind";  
#endif

        // The remote server returned an error: (401) Unauthorized.
        // The remote server returned an error: (404) Not Found.

        static Func<string, Command> TestCommand = (s => Command.Run("o2pgen", s.Split(' '),
             options => options.Timeout(TimeSpan.FromSeconds(Timeout))));

        /// <summary>
        /// exitcode = tuble.item1  , output= tuble.item2
        /// </summary>
          private Func<string, Tuple<int, string>> RunCommand = (s =>
        {
            var command =  Command.Run("o2pgen", s.Split(' '),
             options => options.Timeout(TimeSpan.FromSeconds(Timeout)));
            //var command = TestCommand(s);
            // in an async method, we could use var result = await command.Task;
            var outText = command.Result.StandardOutput;
            var errText = command.Result.StandardError;
            var exitCode = command.Result.ExitCode;
            //Console.WriteLine(outText);
         //   Assert.AreEqual(0, exitCode);
            Tuple<int, string> tuple = new Tuple<int, string>(exitCode, outText + "\n" + errText);
            Console.WriteLine(tuple.Item2);
            return tuple;
        });

        [Test]
        public void ShowHelpIfNoArguments()
        {
            var a = "";
            var tuble = RunCommand(a);
            var output = tuble.Item2;
            Assert.AreEqual(0, tuble.Item1);
            var help = File.ReadAllText("help.txt");
            Assert.IsTrue(output.Contains(help.Trim()));
        }
        [Test]
        public void ShowAllArguments()
        {
            var a = string.Format("-r {0} -dlv -m meta.xml -fnorth.cs ", Url);
            var tuble = RunCommand(a);
            var output = tuble.Item2;
            Assert.AreEqual(0, tuble.Item1);

            Assert.AreEqual(0, tuble.Item1);
            Assert.IsTrue(output.Contains("Saving generated code to file : north.cs"));
            Assert.IsTrue(output.Contains("HTTP Header"));
            Assert.IsTrue(output.Contains("POCO classes"));
            Assert.IsTrue(output.Contains("Saving Metadata to file : meta.xml"));
            Assert.IsTrue(output.Contains("public class Product"));

        }
        [Test]
        //-r
        public void StartWith_r_Argument()
        {
            var a = string.Format("-r {0}", Url);
            var tuble = RunCommand(a);
            var output = tuble.Item2;
            Assert.AreEqual(0, tuble.Item1);

            Assert.IsTrue(output.Contains("Saving generated code to file : poco.cs"));
        }

        [Test]
        //-r
        public void StartWith_r_u_p_Argument()
        {
            var a = string.Format("-r {0} -ux -py", Url);
            var tuble = RunCommand(a);
            var output = tuble.Item2;
            Assert.AreEqual(0, tuble.Item1);
            Assert.IsTrue(output.Contains("Saving generated code to file : poco.cs"));
        }

        [Test]
        //-f
        public void StartWith_f_Argument()
        {
            var a = string.Format("-r {0} -f north.cs", Url);
            var tuble = RunCommand(a);
            var output = tuble.Item2;
            Assert.AreEqual(0, tuble.Item1);
            Assert.IsTrue(output.Contains("Saving generated code to file : north.cs"));
        }

        [Test]
        public void StartWith_d_Argument()
        {
            var a = string.Format("-r {0} -d", Url);
            var tuble = RunCommand(a);
            var output = tuble.Item2;
            Assert.AreEqual(0, tuble.Item1);
            Assert.IsTrue(output.Contains("HTTP Header"));
        }

        [Test]
        public void StartWith_m_Argument()
        {
            var a = string.Format("-r {0} -m meta.xml", Url);
            var tuble = RunCommand(a);
            var output = tuble.Item2;
            Assert.AreEqual(0, tuble.Item1);
            Assert.IsTrue(output.Contains("Saving Metadata to file : meta.xml"));
        }

        [Test]
        public void StartWith_l_Argument()
        {
            var a = string.Format("-r {0} -l ", Url);
            var tuble = RunCommand(a);
            var output = tuble.Item2;
            Assert.AreEqual(0, tuble.Item1);
            Assert.IsTrue(output.Contains("POCO classes"));
        }

        [Test]
        public void StartWith_v_Argument()
        {
            var a = string.Format("-r {0} -v", Url);
            var tuble = RunCommand(a);
            var output = tuble.Item2;
            Assert.AreEqual(0, tuble.Item1);
            Assert.IsTrue(output.Contains("public class Product"));
        }

        [Test]
        public void StartWith_error_Argument()
        {
            var a = @"-r www.google.com";
            var tuble = RunCommand(a);
            var output = tuble.Item2;
            Assert.AreNotEqual(0, tuble.Item1);
            Assert.IsTrue(output.Contains("Error in executing the command"));

        }


        [Test]
        public void StartWith_Argument2()
        {
            var a = string.Format("-r {0} -dlv -m meta.xml -fnorth.cs ", Url);
            var result = RunCommand(a);
            //Console.WriteLine(result.Item2);
            Assert.AreEqual(0, result.Item1);
        }

        [Test]
        [TestCase(@"data\northwindv4.xml", 0)]
        [TestCase(@"data\northwindv3.xml", 0)]
        [TestCase(@"data\invalidxml.xml", -1)]
        public void FileReadingTest(string url, int exitCode)
        {
            var a = string.Format("-r {0} -dlv -m meta.xml -fnorth.cs  ", url);
            var tuble = RunCommand(a);
            var output = tuble.Item2;
            Assert.AreEqual(exitCode, tuble.Item1);

        }
        
        [Test]
        [TestCase("http://services.odata.org/V4/OData/OData.svc")]
        [TestCase("http://services.odata.org/V3/OData/OData.svc")]
        [TestCase("http://localhost/odata2/api/northwind")] //not authorized
        [TestCase("http://localhost/odata20/api/northwind")] //not found
        public void CollectionContains(string url)
        {
            var expected = new List<int> { 0, -1 }; //-1 is valid exitcode if there's no connection to internet
            var actual = 5;
            var a = string.Format("-r {0} -dlv -m meta.xml -fnorth.cs  ", url);
            var result = RunCommand(a);
           
            CollectionAssert.Contains(expected, result.Item1);
            //Assert.That(expected, Contains.Item(actual));
        }

   
    }//
}//
