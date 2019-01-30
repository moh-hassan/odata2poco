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

        [Test]
        public async Task Arg_contains_version_Test()
        {
            var args = new[] { "--version" };
            await Program.RunOptionsAsync(args);
            var help = Program.HelpWriter.ToString();
            var result = Regex.Split(help, "\r\n|\r|\n");
            Assert.That(result.Length, Is.EqualTo(1));

        }

        [Test]
        public async Task Arg_contains_help_Test()
        {
            var args = new string[] { "--help" };
            await Program.RunOptionsAsync(args);
            var help = Program.HelpWriter.ToString();
            var result = Regex.Split(help, "\r\n|\r|\n");
            Assert.That(result.Length, Is.GreaterThan(1));
            Assert.That(help, Does.Contain("-r, --url"));


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
            var args = new[] { arg};
            ParserResult<Options> result = Program.GetParserResult(args);
            NotParsed<Options> notParsed = (NotParsed<Options>)result;
            IEnumerable<Error> errors = notParsed.Errors;
           var helpOrVersionRequested = errors.Any(x => x.Tag == ErrorType.HelpRequestedError 
                                                        || x.Tag == ErrorType.VersionRequestedError);
          
            Assert.That(result.Tag, Is.EqualTo(ParserResultType.NotParsed));
            Assert.That(helpOrVersionRequested, Is.True);
            

        }
    }
    internal class VbConversionTest : BaseTest
    {
        public string WorkingDirectory { get; set; }
        [OneTimeSetUp]
        public void SetupOneTime()
        {
            Environment.CurrentDirectory = TestContext.CurrentContext.TestDirectory;
            WorkingDirectory = Environment.CurrentDirectory;
        }

      

        [Test]
        public async Task vb_convert_Test()
        {
            var source = @"public class MyClas {}";
            var vbCode = await CodeConvertorRestService.CodeConvert(source);
            Console.WriteLine(vbCode);
            Assert.That(vbCode, Does.Contain("Public Class MyClas"));
            Assert.That(vbCode, Does.Contain("End Class"));

        }

        [Test]
        public async Task vb_convert_Test2()
        {
            var source = @"
[Table]
public class MyClas 
{
     [Key]
    public int Id {get;set;}
  public string Name {get;set;}
}";
            var vbCode = await CodeConvertorRestService.CodeConvert(source);
            Console.WriteLine(vbCode);
            /*
           <Table>
           Public Class MyClas
               <Key>
               Public Property Id As Integer
               Public Property Name As String
           End Class
            */

            Assert.That(vbCode, Does.Contain("<Table>"));
            Assert.That(vbCode, Does.Contain("Public Class MyClas"));
            Assert.That(vbCode, Does.Contain(" Public Property Name As String"));
            Assert.That(vbCode, Does.Contain("End Class"));

        }
        
        [Test]
        public async Task vb_code_generation_Test()
        {
            var url = TestSample.NorthWindV4;
            var args = $"-r {url}  -v -a key req --lang vb".Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            await Program.RunOptionsAsync(args);

            var output = Program.Logger.Output.ToString();
            Assert.That(output, Does.Contain("Public Class Product"));
            Assert.That(output, Does.Contain("<Key>"));
            Assert.That(output, Does.Contain("<Required>"));
            Assert.That(output, Does.Contain(" Public Property ID As Integer"));
        }

        [Test]
        public async Task Lang_invalid_Test()
        {
            var url = TestSample.NorthWindV4;
            var args = $"-r {url}  -v --lang zz".Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            await Program.RunOptionsAsync(args);

            var output = Program.Logger.Output.ToString();
            Console.WriteLine(output.Dump());

            Assert.That(output, Does.Contain("Invalid Language Option 'zz'. It's set to 'cs'."));

        }
        [Test]
        public async Task Attribute_invalid_Test()
        {
            var url = TestSample.NorthWindV4;
            var args = $"-r {url}  -v -a zz".Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            await Program.RunOptionsAsync(args);

            var output = Program.Logger.Output.ToString();
            Console.WriteLine(output.Dump());
            Assert.That(output, Does.Contain("Attribute 'zz' isn't valid. It will be  droped"));
             
        }
    }
}
