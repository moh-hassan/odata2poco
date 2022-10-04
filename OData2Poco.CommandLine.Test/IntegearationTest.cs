using NUnit.Framework;
using System;
using CliWrap;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using FluentAssertions;
using System.Collections;

namespace OData2Poco.CommandLine.Test
{
    internal class IntegearationTest
    {
        //release
        string WorkingDirectory;
        string o2pgen;        

        [OneTimeSetUp]
        public void Setup()
        {
#if DEBUG
            WorkingDirectory = Path.GetFullPath(Path.Combine(TestSample.BaseDirectory));
            o2pgen = Path.GetFullPath(Path.Combine(TestSample.BaseDirectory, "o2pgen.exe"));
#else
            //release
            WorkingDirectory = Path.GetFullPath(Path.Combine(TestSample.BaseDirectory,
                "..", "..", "..", "..", "build"));
            o2pgen = Path.GetFullPath(Path.Combine(TestSample.BaseDirectory,
                "..", "..", "..", "..", "build", "o2pgen.exe"));
            
#endif

            //o2pgen.exe is only FullFramework
#if !DEBUG && NETCOREAPP
 Assert.Ignore("ignore test in NETCOREAPP on RELEASE");
#endif
        }

        [Test]
        [Category("integeration")]
        [TestCaseSource(nameof(TestCases))]
        public async Task Integeration_test_using_executable_exe_file(string options, int exitCode, string expected)
        {
            var stdOutBuffer = new StringBuilder();             
            var result = await Cli.Wrap(o2pgen)
                .WithArguments(options)
                .WithStandardOutputPipe(PipeTarget.ToStringBuilder(stdOutBuffer))
                .WithValidation(CommandResultValidation.None)
                .WithWorkingDirectory(WorkingDirectory)
                .ExecuteAsync();
          
            stdOutBuffer.ToString().Should().Contain(expected);
            result.ExitCode.Should().Be(exitCode);

        }
        
        public static IEnumerable TestCases
        {
            get
            {
                var url = TestSample.UrlTripPinService;
                yield return new TestCaseData($"-r {url} -v", 0, "public partial class Airline");
                yield return new TestCaseData($"-r {url} -v -Y", -1,"ERROR(S)");
                yield return new TestCaseData($"-r {url} -v -G record -I", 0, "public partial record Airline");
            }
        }
    }
}
