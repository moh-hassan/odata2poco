// Copyright (c) Mohamed Hassan & Contributors. All rights reserved. See License.md in the project root for license information.

namespace OData2Poco.CommandLine.Test;

using System.Collections;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Security.Policy;
using System.Text;
using CliWrap;
using static OData2Poco.Fake.TestCaseSources;
public class IntegrationTest
{
    private string Config => Configure();
    private string Os => CheckOs();

#if !DEBUG && !NETCOREAPP
    //Windows release only
    [Test]
    [Category("integration")]
    [TestCaseSource(typeof(TestCaseSources), nameof(TestCases2))]
    public async Task O2pgen_exe_can_run_in_windows_and_linux_with_mono_installed(
        string options,
        int expectedExitCode,
        string expected)
    {
        //Arrange
        var fw = "net472";
        //Act
        var (exitCode, output) = await RunAsync(options, fw, Config, true).ConfigureAwait(false);
        //Assert
        output.ToString().Should().Contain(expected);
        exitCode.Should().Be(expectedExitCode);
    }
#endif

    //test in release and debug
    [Test]
    [Category("integration")]
#if NET
    [TestCase("net8.0")]
#else
    [TestCase("net472")]
#endif
    public async Task O2pgen_exe_release_is_not_merged_test(string fw)
    {
        //Arrange
        var args = $"-r {OdataService.Trippin} -v";
        //Act
        var (exitCode, output) = await RunAsync(args, fw, Config).ConfigureAwait(false);
        //Assert
        exitCode.Should().Be(0);
        output.ToString().Should().Contain("public partial class Airline");
    }
    //#endif

#if NETFRAMEWORK && !DEBUG
    [Test]
    [TestCase(true)]
    public async Task O2pgen_exe_release_is_merged_test(bool isMerged)
    {
        //Arrange
        var fw = "net472";
        var args = $"-r {OdataService.Trippin} -v";
        //Act
        var (exitCode, output) = await RunAsync(args, fw, Config, isMerged).ConfigureAwait(false);
        //Assert
        exitCode.Should().Be(0);
        output.ToString().Should().Contain("public partial class Airline");
    }
#endif

    private string Configure()
    {
#if DEBUG
        return "Debug";
#else
        return "Release";
#endif
    }

    private (string workDir, string exe) GetBinFolder(string project,
        string fw,
        string configuration = "Release",
        bool isMerged = false)
    {
        var wd = Path.GetFullPath(Path.Combine(TestSample.SolutionFolder, project, "Bin", configuration, fw));
        if (isMerged && configuration == "Release" && fw.StartsWith("net4"))
        {
            wd = Path.GetFullPath(Path.Combine(TestSample.SolutionFolder, "build"));
        }

        if (!Directory.Exists(wd))
            throw new DirectoryNotFoundException($"Directory not found: {wd}");
        var program = fw.StartsWith("net4") ? "o2pgen.exe" : "o2pgen.dll";
        program = Path.Combine(wd, program);
        return (wd, program);
    }

    private async Task<(int exitCode, StringBuilder output)> RunAsync(string args,
        string fw = "net472",
        string configuration = "Release",
        bool isMerged = false)
    {
        var project = fw.StartsWith("net4")
            ? "OData2Poco.CommandLine"
            : "OData2Poco.dotnet.o2pgen";
        var (workDir, exe) = GetBinFolder(project, fw, configuration, isMerged);
        var program = exe;
        if (exe.EndsWith("dll"))
        {
            program = "dotnet";
            args = $"o2pgen.dll {args}";
        }

        var output = new StringBuilder();
        var result = await Cli.Wrap(program)
            .WithArguments(args)
            .WithWorkingDirectory(workDir)
            .WithStandardOutputPipe(PipeTarget.ToStringBuilder(output))
            .WithValidation(CommandResultValidation.None)
            .ExecuteAsync();
        return (result.ExitCode, output);
    }

    private string CheckOs()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            return "Windows";

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            return "Linux";

        return RuntimeInformation.IsOSPlatform(OSPlatform.OSX) ? "MacOS" : "Unknown";
    }
}
