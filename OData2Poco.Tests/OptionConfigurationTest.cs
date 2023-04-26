// Copyright (c) Mohamed Hassan & Contributors. All rights reserved. See License.md in the project root for license information.

using FluentAssertions;
using NUnit.Framework;
using OData2Poco.InfraStructure.FileSystem;

namespace OData2Poco.Tests;
public class OptionConfigurationTest
{
    private IPocoFileSystem _fileSystem;
    private OptionConfiguration cfg;
    readonly string config = "config.txt";
    string content = @"
# -r mysite.com
-r localhost

-v  #verbose
   
";
    [SetUp]
    public void SetUp()
    {
        _fileSystem = new NullFileSystem();
        cfg = new OptionConfiguration(_fileSystem);
        Fakes.Mock(config, content);
    }

    [Test]
    public void Read_existing_configuration_file_test()
    {
        //Arrange
        string[] expected = { "-r", "localhost", "-v" };
        //Act
        var sut = cfg.ReadConfig(config);
        //Assert
        sut.Should().BeEquivalentTo(expected);
    }

    [Test]
    public void Load_args_from_existing_configuration_file_test()
    {
        //Arrange
        var args = new[] { "@config.txt" };
        string[] expected = { "-r", "localhost", "-v" };
        Fakes.Mock("o2pgen.txt", content);
        //Act
        var flag = cfg.TryGetConfigurationFile(args, out var cli, out var error,
            out string fileName);
        //Assert
        flag.Should().BeTrue();
        fileName.Should().Be("config.txt");
        error.Should().BeNull();
        cli.Should().BeEquivalentTo(expected);
    }

    [Test]
    public void Load_args_from_not_existing_configuration_file_test()
    {
        //Arrange
        var args = new[] { "@config2.txt" };
        //Act
        var flag = cfg.TryGetConfigurationFile(args, out var cli, out var error,
            out string _);
        //Assert
        flag.Should().BeFalse();
        cli.Should().BeEquivalentTo(args);
    }

    [Test]
    public void Load_args_from_default_configuration_file_test()
    {
        //Arrange
        string[] expected = { "-r", "localhost", "-v" };
        var args = Array.Empty<string>();
        Fakes.Mock("o2pgen.txt", content);
        //Act
        var flag = cfg.TryGetConfigurationFile(args, out var cli, out var error,
            out string fileName);
        Fakes.Remove("o2pgen.txt");

        //Assert
        flag.Should().BeTrue();
        fileName.Should().Be("o2pgen.txt");
        error.Should().BeNull();
        cli.Should().BeEquivalentTo(expected);
    }


    [Test]
    public void More_args_without_configuration_has_no_default_config_test()
    {
        //Arrange
        var args = new[] { "-r", "localhost" };
        //Act
        var flag = cfg.TryGetConfigurationFile(args, out var cli, out var error, out string _);
        //Assert
        flag.Should().BeFalse();
        cli.Should().BeEquivalentTo(args);
    }

    [Test]
    public void Empty_args_without_existing_default_config_o2pgen_txt_test()
    {
        //Arrange
        var args = Array.Empty<string>();
        //Act
        var flag = cfg.TryGetConfigurationFile(args, out var cli, out var error, out string _);

        //Assert
        flag.Should().BeFalse();
        cli.Should().BeEmpty();
    }

}