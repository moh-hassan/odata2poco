// Copyright (c) Mohamed Hassan & Contributors. All rights reserved. See License.md in the project root for license information.

namespace OData2Poco.CommandLine.Test;

[Category("parser")]
public class ParserTest : BaseTest
{
    private static readonly string s_url = TestSample.NorthWindV4;

    private ArgumentParser ArgumentParser { get; } = new();

    [OneTimeSetUp]
    public void SetupOneTime()
    {
        Environment.CurrentDirectory = TestContext.CurrentContext.TestDirectory;
    }

    [Test]
    public async Task Arg_contains_version_Test()
    {
        //Arrange
        string[] args = ["--version"];
        //Act
        var (_, output) = await RunCommand(args).ConfigureAwait(false);
        //Assert
        Assert.That(output.Split('\n'), Has.Length.EqualTo(2)); //commandlinee v2.8 add newline
    }

    [Test]
    public async Task Arg_contains_help_Test()
    {
        //Arrange
        string[] args = ["--help"];
        //Act
        var (_, help) = await RunCommand(args).ConfigureAwait(false);
        //Assert
        Assert.Multiple(() =>
        {
            Assert.That(help.Split('\n'), Has.Length.GreaterThan(1));
            Assert.That(help, Does.Contain("-r, --url"));
        });
    }

    [Test]
    public async Task Arg_contains_repeated_options_Test()
    {
        var args = $"-r {s_url} -v -v".SplitArgs();
        var (retCode, help) = await RunCommand(args).ConfigureAwait(false);
        Assert.Multiple(() =>
        {
            Assert.That(retCode, Is.EqualTo(1));
            Assert.That(help, Does.Contain("ERROR(S)"));
            Assert.That(help, Does.Contain("Option 'v, verbose' is defined multiple times"));
        });
    }

    [Test]
    [TestCase("-r")]
    [TestCase("-v")]
    public async Task Arg_contains_url_without_option_Test(string arg)
    {
        var args = arg.SplitArgs();
        var (retCode, output) = await RunCommand(args).ConfigureAwait(false);
        Assert.Multiple(() =>
        {
            Assert.That(retCode, Is.EqualTo(1));
            Assert.That(output, Does.Contain("ERROR(S)"));
            Assert.That(output, Does.Contain("Required option 'r, url' is missing."));
        });
    }

    [Test]
    [TestCase("")]
    [TestCase("-v")]
    //any option exept -r
    public async Task Arg_is_empty_Test(string options)
    {
        char[] separator = [' '];
        var args = options.Split(separator, StringSplitOptions.RemoveEmptyEntries);
        var (retCode, output) = await RunCommand(args).ConfigureAwait(false);
        Assert.Multiple(() =>
        {
            Assert.That(retCode, Is.EqualTo(1));
            Assert.That(output.Split('\n'), Has.Length.GreaterThan(1));
            Assert.That(output, Does.Contain("-r, --url"));
            Assert.That(output, Does.Contain("ERROR(S)"));
            Assert.That(output, Does.Contain("Required option 'r, url' is missing"));
        });
    }

    private async Task<Tuple<int, string>> RunCommand(string[] args)
    {
        ArgumentParser.ClearLogger();
        ArgumentParser.SetLoggerSilent();
        var exitCode = await ArgumentParser.RunOptionsAsync(args).ConfigureAwait(false);
        var help = ArgumentParser.Help;
        return new Tuple<int, string>(exitCode, help);
    }
}
