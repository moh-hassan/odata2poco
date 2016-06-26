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
<<<<<<< HEAD
        [TestCase(UrlV4)]
        [TestCase(UrlV3)]
        [TestCase(@"data\northwindv4.xml")]
        [TestCase(@"data\northwindv3.xml")]
        public void AllArgumentTest(string url)
=======
        [TestCase(@"data\northwindv4.xml")]
        [TestCase(@"data\northwindv3.xml")]
        public void AllArgumentFileTest(string url)
>>>>>>> develop
        {
            var a = string.Format("-r {0} -d -l -v -m meta.xml -f north.cs ", url);
            var tuble = RunCommand(a);
            var output = tuble.Item2;
            Assert.AreEqual(0, tuble.Item1);
            Console.WriteLine(tuble.Item2);
            Assert.IsTrue(output.Contains("Saving generated code to file : north.cs")); //-f -r
<<<<<<< HEAD
            Assert.IsTrue(output.Contains("HTTP Header")); //-d
=======
            Assert.IsFalse(output.Contains("HTTP Header")); //-d for http only
>>>>>>> develop
            Assert.IsTrue(output.Contains("POCO classes")); //-l
            Assert.IsTrue(output.Contains("Saving Metadata to file : meta.xml")); //-m
            Assert.IsTrue(output.Contains("public class Product")); //-v

        }
<<<<<<< HEAD
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

=======

        [Test]
        [TestCase(UrlV4)]
        [TestCase(UrlV3)]
        [TestCase(@"data\northwindv4.xml")]
        [TestCase(@"data\northwindv3.xml")]
        public void UrlArgumentTest(string url)
        {
            var a = string.Format("-r {0} -v", url);
            var tuble = RunCommand(a);
            var output = tuble.Item2;
            Assert.AreEqual(0, tuble.Item1);
            Assert.IsTrue(output.Contains("public class Product")); //-v

        }

      

>>>>>>> develop
        [Test]
        [TestCase(UrlV4)]
        [TestCase(UrlV3)]
        //-r
        public void StartWith_r_u_p_ArgumentTest(string url)
        {
<<<<<<< HEAD
            var a = string.Format("-r {0} -ux -py", url);
=======
            var a = string.Format("-r {0} -v -ux -py", url);
>>>>>>> develop
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
<<<<<<< HEAD
            var a = String.Format("-r {0}", url);
            var tuble = RunCommand(a);
            var output = tuble.Item2;
=======
            var a = String.Format("-r {0} -vdl", url);
            var tuble = RunCommand(a);
            var output = tuble.Item2;
            Console.WriteLine(output);
>>>>>>> develop
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
<<<<<<< HEAD
        [TestCase(@"data\northwindv4.xml", 0)]
        [TestCase(@"data\northwindv3.xml", 0)]
        [TestCase(@"data\invalidxml.xml", -1)]
        public void FileReadingTest(string url, int exitCode)
        {
            var a = string.Format("-r {0} -d -l -v -m meta.xml -f north.cs  ", url);
            var tuble = RunCommand(a);
            var output = tuble.Item2;
=======
      [TestCase(@"data\invalidxml.xml", -1)]
        public void FileReadingTest(string url, int exitCode)
        {
         //   var a = string.Format("-r {0} -d -l -v -m meta.xml -f north.cs  ", url);
            var a = string.Format("-r {0} -vld -m meta.xml -f north.cs  ", url);
            var tuble = RunCommand(a);
            var output = tuble.Item2;
            Console.WriteLine(output);
>>>>>>> develop
            Assert.AreEqual(exitCode, tuble.Item1);

        }

        [Test]
<<<<<<< HEAD
        [TestCase(UrlV4)]
        [TestCase(UrlV3)]
        [TestCase("http://localhost/odata2/api/northwind")] //not authorized
        [TestCase("http://localhost/odata20/api/northwind")] //not found
=======
        [TestCase(@"data\not_exist_file.xml", -1)]
        public void FileNotExistReadingTest(string url, int exitCode)
        {
            //   var a = string.Format("-r {0} -d -l -v -m meta.xml -f north.cs  ", url);
            var a = string.Format("-r {0} -vld -m meta.xml -f north.cs  ", url);
            var tuble = RunCommand(a);
            var output = tuble.Item2;
            Console.WriteLine(output);
            Assert.AreEqual(exitCode, tuble.Item1);

        }
     
        //disable network card for this test
        [Test]
        [TestCase(UrlV4)]
        [TestCase(UrlV3)]
>>>>>>> develop
        public void NotHangingInCaseNoConnectionTest(string url)
        {
            var expected = new List<int> { 0, -1 }; //-1 is valid exitcode if there's no connection to internet
            var actual = 5;
            var a = string.Format("-r {0} -dlv -m meta.xml -fnorth.cs  ", url);
            var result = RunCommand(a);

            CollectionAssert.Contains(expected, result.Item1);

        }
<<<<<<< HEAD
       
=======
        //v1.5
        [Test]
        [TestCase(UrlV4)]
        [TestCase(UrlV3)]
        public void NewFeaturesV1_5_options_kt(string url)
        {
            var a = string.Format("-r {0} -v -k -t -q -n", url);
            var tuble = RunCommand(a);
            var output = tuble.Item2;
            Assert.AreEqual(0, tuble.Item1);
            Console.WriteLine(tuble.Item2);
            /*
            [Table("Products")]
     public class Product
     {
         [Key]
         [Required]
         public int ID  {get;set;} //PrimaryKey not null
         public string Name  {get;set;} 
         public string Description  {get;set;} 
         [Required]
         public DateTimeOffset ReleaseDate  {get;set;} // not null
         public DateTimeOffset DiscontinuedDate  {get;set;} 
         [Required]
         public short Rating  {get;set;} // not null
         [Required]
         public double Price  {get;set;} // not null
         virtual public List<Category> Categories  {get;set;} 
         virtual public Supplier Supplier  {get;set;} 
         virtual public ProductDetail ProductDetail  {get;set;} 
     }	 
             * */

            Assert.IsTrue(output.Contains("public class Product"));
            Assert.IsTrue(output.Contains("[Table(\"Products\")]")); //-t
            Assert.IsTrue(output.Contains("[Key]")); //-k
            Assert.IsTrue(output.Contains("[Required]"));  //-q
            Assert.IsTrue(output.Contains("virtual public Supplier Supplier  {get;set;}")); //-n
        }
>>>>>>> develop

    }//
}//
