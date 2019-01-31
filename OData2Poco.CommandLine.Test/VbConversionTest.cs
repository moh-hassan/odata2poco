
using System;
using System.Threading.Tasks;
using NUnit.Framework;

namespace OData2Poco.CommandLine.Test
{
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
        public async Task vb_code_generation_end_to_end_Test()
        {
            var url = TestSample.NorthWindV4;
            var args = $"-r {url}  -v -a key req --lang vb".Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            await Program.RunOptionsAsync(args);
            Program.Logger.Silent = true;
            var output = Program.OutPut;
            Assert.That(output, Does.Contain("Public Partial Class Product"));
            Assert.That(output, Does.Contain("<Key>"));
            Assert.That(output, Does.Contain("<Required>"));
            Assert.That(output, Does.Contain(" Public Property ProductID As Integer"));
        }

        [Test]
        public async Task Lang_invalid_Test()
        {
            var url = TestSample.NorthWindV4;
            var args = $"-r {url}  -v --lang zz".Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            await Program.RunOptionsAsync(args);

            var output = Program.Logger.Output.ToString();
            Assert.That(output, Does.Contain("Invalid Language Option 'zz'. It's set to 'cs'."));

        }
        [Test]
        public async Task Attribute_invalid_Test()
        {
            var url = TestSample.NorthWindV4;
            var args = $"-r {url}  -v -a zz".Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            await Program.RunOptionsAsync(args);

            var output = Program.Logger.Output.ToString();
            Assert.That(output, Does.Contain("Attribute 'zz' isn't valid. It will be  droped"));

        }
    }
}
