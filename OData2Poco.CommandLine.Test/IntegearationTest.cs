// Copyright (c) Mohamed Hassan & Contributors. All rights reserved. See License.md in the project root for license information.

namespace OData2Poco.CommandLine.Test;

using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Text;
using CliWrap;
using static OData2Poco.Fake.TestCaseSources;

[Category("integration")]
public class IntegrationTest
{
    //private string Config => Configure();
    private string Os => CheckOs();
#if Debug
    private string _config = "Debug";
    private bool isMerged = false;
#else
    private string _config = "Release";
    private bool isMerged = true;
#endif

#if NETCOREAPP
    private string fw = "net8.0";
#else
    private string fw = "net472";
#endif

#if !NETCOREAPP

    [Test]
    public async Task O2pgen_exe_can_run_in_windows_and_linux_with_mono_installed()
    {
        //Arrange
        var options = $"-r {OdataService.Trippin} -v";
        var expectedExitCode = 0;
        var expected = "public partial class Airline";
        //Act
        var (exitCode, output) = await RunAsync(options, fw, _config)
            .ConfigureAwait(false);
        //Assert
        output.ToString().Should().Contain(expected);
        exitCode.Should().Be(expectedExitCode);
    }

    [Test]
    public async Task O2pgen_exe_release_is_not_merged_test()
    {
        //Arrange
        var args = $"-r {OdataService.Trippin} -v";
        //Act
        var (exitCode, output) = await RunAsync(args, fw, _config)
            .ConfigureAwait(false);
        //Assert
        exitCode.Should().Be(0);
        output.ToString().Should().Contain("public partial class Airline");
    }

    [Test]
    public async Task O2pgen_exe_release_is_merged_test()
    {
        //Arrange
        var args = $"-r {OdataService.Trippin} -v";
        //Act
        var (exitCode, output) = await RunAsync(args, fw, _config).ConfigureAwait(false);
        //Assert
        exitCode.Should().Be(0);
        output.ToString().Should().Contain("public partial class Airline");
    }
    private async Task<(int exitCode, StringBuilder output)> RunAsync(string args,
        string fw,
        string configuration)
    {
        var project = fw.StartsWith("net4")
            ? "OData2Poco.CommandLine"
            : "OData2Poco.dotnet.o2pgen";
        var (workDir, exe) = GetBinFolder(project, fw, configuration);
        var program = exe;
        if (exe.EndsWith("dll"))
        {
            program = "dotnet";
            args = $"o2pgen.dll {args}";
        }

        var output = new StringBuilder();
        try
        {
            var result = await Cli.Wrap(program)
                .WithArguments(args)
                .WithWorkingDirectory(workDir)
                .WithStandardOutputPipe(PipeTarget.ToStringBuilder(output))
                .WithValidation(CommandResultValidation.None)
                .ExecuteAsync();
            return (result.ExitCode, output);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            output.AppendLine(ex.Message);
            return (-1, output);
        }
    }

    private (string workDir, string exe) GetBinFolder(string project,
       string fw,
       string configuration
       )
    {
        var wd = Path.GetFullPath(Path.Combine(TestSample.SolutionFolder, project, "Bin", configuration, fw));
        if (isMerged && configuration == "Release" && fw.StartsWith("net4"))
        {
            wd = Path.GetFullPath(Path.Combine(TestSample.SolutionFolder, "build"));
        }

        var program = fw.StartsWith("net4") ? "o2pgen.exe" : "o2pgen.dll";
        program = Path.Combine(wd, program);
        return (wd, program);
    }
#endif

    private string CheckOs()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            return "Windows";

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            return "Linux";

        return RuntimeInformation.IsOSPlatform(OSPlatform.OSX) ? "MacOS" : "Unknown";
    }
}
