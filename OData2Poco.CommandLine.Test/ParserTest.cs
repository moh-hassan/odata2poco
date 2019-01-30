using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CommandLine;
using NUnit.Framework;
using OData2Poco.Extension;

namespace OData2Poco.CommandLine.Test
{
    [Category("parser")]
    public class ParserTest : BaseTest
    {
        private static string WorkingDirectory = ".";
        private const string Url = TestSample.NorthWindV4;
       
        [OneTimeSetUp]
        public void SetupOneTime()
        {
            Environment.CurrentDirectory = TestContext.CurrentContext.TestDirectory;
            WorkingDirectory = Environment.CurrentDirectory;
            Program.Logger.Silent = true;
        }

        [Test]
        public async Task Arg_contains_version_Test()
        {
            //Arrange
            var args = new[] { "--version" };
            //Act
            await Program.RunOptionsAsync(args);
            var help = Program.HelpWriter.ToString();
            var result = Regex.Split(help, "\r\n|\r|\n");
            //Assert
            Assert.That(result.Length, Is.EqualTo(1));

        }

        [Test]
        public async Task Arg_contains_help_Test()
        {
            //Arrange
            var args = new string[] { "--help" };
            //Act
            await Program.RunOptionsAsync(args);
            var help = Program.HelpWriter.ToString();
            var result = Regex.Split(help, "\r\n|\r|\n");
            //Assert
            Assert.That(result.Length, Is.GreaterThan(1));
            Assert.That(help, Does.Contain("-r, --url"));

        }

        [Test]
        public async Task Arg_contains_repeated_options_Test()
        {
            var args = $"-r {Url} -v -v".SplitArgs();
           var retCode=await Program.RunOptionsAsync(args);
            var help = Program.HelpWriter.ToString();
            var result = Regex.Split(help, "\r\n|\r|\n");
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
            var retCode = await Program.RunOptionsAsync(args);
            var help = Program.HelpWriter.ToString();
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
            await Program.RunOptionsAsync(args);
            var help = Program.HelpWriter.ToString();
            var result = Regex.Split(help, "\r\n|\r|\n");
            Assert.That(result.Length, Is.GreaterThan(1));
            Assert.That(help, Does.Contain("-r, --url"));
            Assert.That(help, Does.Contain("ERROR(S)"));
            Assert.That(help, Does.Contain("Required option 'r, url' is missing"));

        }

        [Test]
        [TestCase("--version")]
        [TestCase("--help")]
        public void GetParserResult_verion_or_help_return_true_test(string arg)
        {
            var args = new[] { arg };
            ParserResult<Options> result = Program.GetParserResult(args);
            NotParsed<Options> notParsed = (NotParsed<Options>)result;
            IEnumerable<Error> errors = notParsed.Errors;
            var helpOrVersionRequested = errors.Any(x => x.Tag == ErrorType.HelpRequestedError
                                                         || x.Tag == ErrorType.VersionRequestedError);

            Assert.That(result.Tag, Is.EqualTo(ParserResultType.NotParsed));
            Assert.That(helpOrVersionRequested, Is.True);


        }
    }
}
