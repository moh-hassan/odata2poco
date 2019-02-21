using System;
using NUnit.Framework;
using OData2Poco.Extensions;

namespace OData2Poco.CommandLine.Test
{
    class OptionsTest
    {
        [Test]
        [TestCase("pas")]
        [TestCase("PAS")]
        [TestCase("cam")]
        [TestCase("camel")]
        [TestCase("none")]

        public void NameCase_valid_Test(string name)
        {
            var options = new Options
            {
                Lang = "cs",
                NameCase = name
            };
            var ret = options.Validate();
            Assert.That(ret, Is.EqualTo(0));
            Assert.That(options.Errors, Is.Empty);
        }
        [Test]
        [TestCase("zz")]
        [TestCase("_")]
        [TestCase("")]
        public void NameCase_invalid_Test(string name)
        {
            var options = new Options
            {
                Lang = "cs",
                NameCase = name
            };
            var ret = options.Validate();
            Assert.That(ret, Is.EqualTo(0));
            Assert.That(options.Errors, Is.Not.Empty);

        }
        [Test]
        public void Read_paramfile_test()
        {
            var options = new Options
            {
                ParamFile = TestSample.Param1,
            };
            
            var connString=options.GetOdataConnectionString();
            Console.WriteLine(options.ParamFile);
            Console.WriteLine($"connString.ParamFile {connString.ParamFile}");
            var dict=connString.EnvironmentVariables;
            Assert.That(dict.Count, Is.GreaterThan(0));
            Assert.That(dict["url"], Is.EqualTo("http://localhost"));
            Console.WriteLine(dict.Dump());
        }
        [Test]
        public void Read_paramfile_test2()
        {
            var options = new Options
            {
                Url = "{{url}}",
                ParamFile = TestSample.Param1,
            };

            var connString = options.GetOdataConnectionString();
            //Console.WriteLine(options.ParamFile);
            //Console.WriteLine($"connString.ParamFile {connString.ParamFile}");
            //var dict = connString.EnvironmentVariables;
            //Assert.That(dict.Count, Is.GreaterThan(0));
            //Assert.That(dict["url"], Is.EqualTo("http://localhost"));
            Console.WriteLine($"connString.ServiceUrl= {connString.ServiceUrl}");
        }
    }
}
