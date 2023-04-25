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

    [SetUp]
    public void SetUp()
    {
        _fileSystem = new NullFileSystem();
        cfg = new OptionConfiguration(_fileSystem);
        var content = @"
# -r mysite.com
-r localhost

-v  #verbose
   
";
        Fakes.Mock(config, content);
        Fakes.Mock("o2pgen.txt", content);
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
        //Act
        var flag = cfg.TryGetConfigurationFile(args, out var cli);
        //Assert
        flag.Should().BeTrue();
        cfg.Errors.Should().HaveCount(1);
        cli.Should().BeEquivalentTo(expected);
    }
    [Test]
    public void Load_args_from_not_existing_configuration_file_test()
    {
        //Arrange
        var args = new[] { "@config2.txt" };
        //Act
        var flag = cfg.TryGetConfigurationFile(args, out var cli);
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
        //Act
        var flag = cfg.TryGetConfigurationFile(args, out var cli);
        //Assert
        flag.Should().BeTrue();
        cfg.Errors.Should().HaveCount(1);
        cli.Should().BeEquivalentTo(expected);
    }

    [Test]
    public void More_args_without_configuration_has_no_default_config_test()
    {
        //Arrange
        var args = new[] { "-r", "localhost" };
        //Act
        var flag = cfg.TryGetConfigurationFile(args, out var cli);
        //Assert
        flag.Should().BeFalse();
        
    }
    [Test]
    public void Empty_args_with_existing_default_config_o2pgen_txt_test()
    {
        //Arrange
        var args = Array.Empty<string>();
        //Act
        //read default configuration file o2pgen.txt
        var flag = cfg.TryGetConfigurationFile(args, out var cli);
       
        //Assert
        flag.Should().BeTrue();
        cli.Should().NotBeEmpty();
    }
    [Test]
    public void Empty_args_without_existing_default_config_o2pgen_txt_test()
    {
        //Arrange
        var args = Array.Empty<string>();
        _fileSystem.Rename("o2pgen.txt","_");
        //Act
        //read default configuration file o2pgen.txt
        var flag = cfg.TryGetConfigurationFile(args, out var cli);

        //Assert
        flag.Should().BeFalse();
        cli.Should().BeEmpty();

        _fileSystem.Rename("","o2pgen.txt");
    }
}