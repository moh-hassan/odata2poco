using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CommandLine;
using NUnit.Framework;
using OData2Poco.CommandLine.InfraStructure.Logging;
using OData2Poco.Extension;

namespace OData2Poco.CommandLine.Test
{
    [Category("parser")]
    public class ParserTest : BaseTest
    {
        private static string WorkingDirectory = ".";
        private static string Url = TestSample.NorthWindV4;
        private ArgumentParser argumentParser;

        public ParserTest()
        {
            argumentParser = new ArgumentParser();
        }
        [OneTimeSetUp]
        public void SetupOneTime()
        {
            Environment.CurrentDirectory = TestContext.CurrentContext.TestDirectory;
            WorkingDirectory = Environment.CurrentDirectory;
        }


        async Task<Tuple<int, string>> RunCommand(string[] args)
        {
            ArgumentParser.Logger.Silent = true;
            ArgumentParser.Logger.Clear();
            var exitCode = await argumentParser.RunOptionsAsync(args);
            var help = ArgumentParser.Help;
            return new Tuple<int, string>(exitCode, help);
        }
        [Test]
        public async Task Arg_contains_version_Test()
        {
            //Arrange
            var args = new[] { "--version" };
            //Act
            var result = await RunCommand(args);
            var help = ArgumentParser.Help;
            //Assert
            Assert.That(result.Item2.Split('\n').Length, Is.EqualTo(1));

        }

        [Test]
        public async Task Arg_contains_help_Test()
        {
            //Arrange
            var args = new string[] { "--help" };
            //Act
            var result = await RunCommand(args);
            var help = result.Item2;
            //Assert
            Assert.That(result.Item2.Split('\n').Length, Is.GreaterThan(1));
            Assert.That(help, Does.Contain("-r, --url"));

        }

        [Test]
        public async Task Arg_contains_repeated_options_Test()
        {
            var args = $"-r {Url} -v -v".SplitArgs();
            var result = await RunCommand(args);
            var help = result.Item2;
            var retCode = result.Item1;

            Assert.That(retCode, Is.EqualTo(-1));
            Assert.That(help, Does.Contain("ERROR(S)"));
            Assert.That(help, Does.Contain("Option 'v, verbose' is defined multiple times"));

        }

        [Test]
        [TestCase("-r")]
        [TestCase("-v")]
        public async Task Arg_contains_url_without_option_Test(string arg)
        {
            var args = arg.SplitArgs();
            var result = await RunCommand(args);
            var help = result.Item2;
            var retCode = result.Item1;
            Assert.That(retCode, Is.EqualTo(-1));
            Assert.That(help, Does.Contain("ERROR(S)"));
            Assert.That(help, Does.Contain("Required option 'r, url' is missing."));

        }

        [Test]
        [TestCase("")]
        [TestCase("-v")]
        //any option exept -r 
        public async Task Arg_is_empty_Test(string options)
        {
            var args = options.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            var result = await RunCommand(args);
            var help = result.Item2;
            var retCode = result.Item1;
            Assert.That(retCode,Is.EqualTo(-1));
            Assert.That(help.Split('\n').Length, Is.GreaterThan(1));
            Assert.That(help, Does.Contain("-r, --url"));
            Assert.That(help, Does.Contain("ERROR(S)"));
            Assert.That(help, Does.Contain("Required option 'r, url' is missing"));

        }

    }
}
