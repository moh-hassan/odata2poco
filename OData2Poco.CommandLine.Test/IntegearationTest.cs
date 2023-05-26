// Copyright (c) Mohamed Hassan & Contributors. All rights reserved. See License.md in the project root for license information.
#if !DEBUG
//release and windows only
using System.Collections;
using System.Runtime.InteropServices;
using System.Text;
using CliWrap;
using FluentAssertions;
using NUnit.Framework;
using OData2Poco.Fake;

namespace OData2Poco.CommandLine.Test;
internal class IntegrationTest
{
    //release
    private string _workingDirectory;
    private string _o2Pgen;

    [OneTimeSetUp]
    public void Setup()
    {
        _workingDirectory = Path.GetFullPath(Path.Combine(TestSample.BaseDirectory));
        _o2Pgen = Path.GetFullPath(Path.Combine(TestSample.BaseDirectory, "o2pgen.exe"));

        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) return;
        _workingDirectory = Path.GetFullPath(Path.Combine(TestSample.BaseDirectory,
            "..", "..", "..", "..", "build"));
        _o2Pgen = Path.GetFullPath(Path.Combine(TestSample.BaseDirectory,
            "..", "..", "..", "..", "build", "o2pgen.exe"));
    }

    [Test]
    [Category("integration")]
    [TestCaseSource(nameof(TestCases))]
    public async Task Integration_test_using_executable_exe_file(string options, int exitCode, string expected)
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            Assert.Ignore("ignore test o2pgen.exe in non windows OS");
            return;
        }
        Console.WriteLine($"BaseDirectory: {SystemConst.BaseDirectory}");
        var stdOutBuffer = new StringBuilder();
        var result = await Cli.Wrap(_o2Pgen)
            .WithArguments(options)
            .WithStandardOutputPipe(PipeTarget.ToStringBuilder(stdOutBuffer))
            .WithValidation(CommandResultValidation.None)
            .WithWorkingDirectory(_workingDirectory)
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
            yield return new TestCaseData($"-r {url} -v -Y", 1, "ERROR(S)");
            yield return new TestCaseData($"-r {url} -v -G record -I", 0, "public partial record Airline");
        }
    }
}

#endif
