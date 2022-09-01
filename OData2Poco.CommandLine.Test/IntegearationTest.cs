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
        string Mode;

        [OneTimeSetUp]
        public void Setup()
        {
#if DEBUG
            WorkingDirectory = Path.GetFullPath(Path.Combine(TestSample.BaseDirectory));
            Mode = "DEBUG";            
            
            o2pgen = Path.GetFullPath(Path.Combine(TestSample.BaseDirectory, "o2pgen.exe"));
#else
            //release
            WorkingDirectory = Path.GetFullPath(Path.Combine(TestSample.BaseDirectory,
                "..", "..", "..", "..", "build"));
            o2pgen = Path.GetFullPath(Path.Combine(TestSample.BaseDirectory,
                "..", "..", "..", "..", "build", "o2pgen.exe"));
            Mode = "RELEASE";
#endif

            //o2pgen.exe is only FullFramework
#if !DEBUG && NETCOREAPP
 Assert.Ignore("ignore test in NETCOREAPP on RELEASE");
#endif
        }

        [Test]
        [Category("integeration")]
        [TestCaseSource(nameof(TestCases))]
        public async Task Test1Async(string options, int exitCode, string expected)
        {
            var stdOutBuffer = new StringBuilder();
            Console.WriteLine($"path= '{o2pgen}'");
            Console.WriteLine($"Mode= {Mode}");
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
                yield return new TestCaseData($"-r {url} -v -RI", 0, "public partial record Airline");
            }
        }
    }
}
