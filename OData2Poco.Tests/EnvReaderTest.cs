// Copyright (c) Mohamed Hassan & Contributors. All rights reserved. See License.md in the project root for license information.

using FluentAssertions;
using NUnit.Framework;

namespace OData2Poco.Tests;

public class EnvReaderTest : BaseTest
{
    #region Setup
    [OneTimeSetUp]
    public void SetUp()
    {
        CreateEnv("aa", "abc123");
        CreateEnv("myToken", "abc123");
        CreateEnv("key1", "abc");
        CreateEnv("user", "user1");
        CreateEnv("password", "secret");
        CreateEnv("zone", "north");
        CreateEnv("client_id", "a-123-456");
        CreateEnv("client_secret", "abc-456-xyz");
    }
    [OneTimeTearDown]
    public void TearDown()
    {
        DelEnv("aa", "myToken", "key1", "user", "password", "zone", "client_id", "client_secret");
    }
    #endregion

    [Test]
    [TestCase("client_secret")]
    [TestCase("myToken")]
    [TestCase("client_id")]
    public void Arg_with_existing_env_test(string arg)
    {
        //Arrange
        //Act
        var flag = new EnvReader(_fileSystem)
            .TryReadEnv(arg, out var value);
        //Assert
        flag.Should().BeTrue();
        value.Should().NotBeEmpty();

    }

    [Test]
#if IsWindows
    [TestCase("%client_secret%")]
    [TestCase("%myToken%")]
    [TestCase("%client_id%")]
#else
    [TestCase("$client_secret")]
    [TestCase("$myToken")]
    [TestCase("$client_id")]
#endif
    public void ResolveEnv_with_existing_env_test(string arg)
    {
        //Arrange
        //Act
        var text = new EnvReader(_fileSystem)
            .ResolveEnv(arg, out var errors);
        //Assert
        text.Should().NotBeEmpty();
        errors.Count.Should().Be(0);
    }

    [Test]
    public void Args_without_env_vars()
    {
        //Arrange
        string[] args = { "-a", "aa", "-b" };
        //Act
        var Args = new EnvReader(_fileSystem)
            .ResolveArgEnv(args, out var errors);
        //Assert
        Args.Length.Should().Be(3);
        Args.Should().BeEquivalentTo(args);
        errors.Should().BeEmpty();
    }

    [Test]
    [TestCase("%aa%")]
    [TestCase("$aa")]
    public void Args_wit_env_exist_test(string envName)
    {
        //Arrange
        string[] args = { "-a", envName, "-b" };
        //Act
        var sut = new EnvReader(_fileSystem)
            .ResolveArgEnv(args, out var errors);

        //Assert
        sut.Length.Should().Be(3);
        sut.Should().BeEquivalentTo("-a", "abc123", "-b");
        errors.Count.Should().Be(0);
    }

    [Test]
    [TestCase("%aa%")]
    [TestCase("$aa")]
    public void Args_wit_expression_have_env_var_exist_test(string envName)
    {
        //Arrange
        string[] args = { "-a", $"Agent {envName}", "-b" };
        //Act
        var sut = new EnvReader(_fileSystem)
            .ResolveArgEnv(args, out var errors);
        //Assert
        sut.Length.Should().Be(3);
        sut.Should().BeEquivalentTo("-a", "Agent abc123", "-b");
        errors.Count.Should().Be(0);
    }


    [Test]
    public void Args_with_env_var_not_exist_test()
    {
        //Arrange
        string[] args = { "-a", "%aa2%", "-b" };
        //Act
        var sut = new EnvReader(_fileSystem)
            .ResolveArgEnv(args, out var errors);
        //Assert
        sut.Length.Should().Be(3);
        sut.Should().BeEquivalentTo(args);
        errors.Count.Should().Be(1);
    }

    [Test]
    public void Arg_as_at_at_file_exist_test()
    {
        //Arrange
        string text = "abc.123.xyz";
        Fakes.Mock("myFile.txt", text);
        string arg = "@@myFile.txt";
        //Act
        var flag = new EnvReader(_fileSystem)
            .TryResolveFile(arg, out var value, out var error);
        Fakes.Remove("myFile.txt");
        //Assert
        flag.Should().BeTrue();
        value.Should().Be("abc.123.xyz");
        error.Should().BeNull();
    }
    [Test]
    public void Args_wit_at_at_file_exist_including_env_var_test()
    {
        //Arrange
        string text = "abc.123.xyz";
        Fakes.Mock("myFile.txt", text);
        string[] args = { "-a", "@@myFile.txt", "-p", "%password%" };
        //Act
        var sut = new EnvReader(_fileSystem)
            .ResolveArgEnv(args, out var errors);
        Fakes.Remove("myFile.txt");
        //Assert
        sut.Length.Should().Be(4);
        sut.Should().BeEquivalentTo("-a", "abc.123.xyz", "-p", "secret");
        errors.Count.Should().Be(0);
    }

    [Test]
    public void Args_wit_at_at_file_not_exist_test()
    {
        //Arrange
        string[] args = { "-a", "@@file.txt", "-b" };
        //Act
        var sut = new EnvReader(_fileSystem)
            .ResolveArgEnv(args, out var errors);
        //Assert
        sut.Length.Should().Be(3);
        sut.Should().BeEquivalentTo(args);
        errors.Count.Should().Be(1);
    }


    [Test]
    [TestCase("%myToken%")]
    [TestCase("$myToken")]
    public void Resolve_arg_expression_with_exist_env_var_test(string envVar)
    {
        //Arrange
        var input = $"Authorization=Bearer {envVar}";
        var expected = "Authorization=Bearer abc123";
        //Act
        var sut = new EnvReader(_fileSystem);
        var text = sut.ResolveEnv(input, out var errors);
        //Assert
        text.Should().Be(expected);
        errors.Count.Should().Be(0);
    }

    [Test]
    [TestCase("%myToken2%")]
    [TestCase("$myToken2")]
    public void Arg_expression_with_not_exist_env_var_test(string envVar)
    {
        //Arrange
        var input = $"Authorization=Bearer {envVar}";
        //Act
        var sut = new EnvReader(_fileSystem);
        var text = sut.ResolveEnv(input, out var errors);
        //Assert
        text.Should().Be(input);
        errors.Count.Should().Be(1);

    }

    [Test]
    public void Arg_has_dollar_sign_should_be_kept_if_no_env_var()
    {
        //Arrange
        var input = "Authorization=Bearer $key1:$key2";
        //Act
        var sut = new EnvReader(_fileSystem);
        var text = sut.ResolveEnv(input, out var errors);
        //Assert
        text.Should().Be("Authorization=Bearer abc:$key2");
        errors.Count.Should().Be(1);
    }


    [Test]
    [TestCase("Authorization=Basic %user%:%password%")]
    [TestCase("Authorization=Basic $user:$password")]
    public void Arg_with_multiple_env_var_existing_test(string input)
    {
        //Arrange
        //Act
        var sut = new EnvReader(_fileSystem);
        var text = sut.ResolveEnv(input, out var errors);
        //Assert
        text.Should().Be("Authorization=Basic user1:secret");
        errors.Count.Should().Be(0);
    }

    [Test]
    [TestCase("Authorization=Basic %user2%:%password2%")]
    [TestCase("Authorization=Basic $user2:$password2")]
    public void Arg_with_multiple_env_var_not_existing_test(string input)
    {
        //Arrange
        var sut = new EnvReader(_fileSystem);
        //Act
        var text = sut.ResolveEnv(input, out var errors);

        //Assert
        text.Should().Be(input);
        errors.Count.Should().Be(2);
    }

    [Test]
    public void Resolve_env_var_include_newline_test()
    {
        //Arrange
        var input = @"
-r http://myservice/$zone/v1
-auth token
-p $myToken
-u $user1
";
        var expected = @"
-r http://myservice/north/v1
-auth token
-p abc123
-u $user1
";
        //Act
        var sut = new EnvReader(_fileSystem);
        var text = sut.ResolveEnv(input, out var errors);
        //Assert
        text.Trim().Should().Be(expected.Trim());
        errors.Count.Should().Be(1);
    }

    [Test]
    public void Configuration_file_including_env_var_test()
    {
        //Arrange
        var cmd = """
        -r http://localhost/odata
        --auth oauth2
        -u %client_id%
        -p %client_secret%
        --token-endpoint https://dev-identity.com/oauth/token
        --token-params  audience=https://www.todo.com
        -v
        """;
        var expected = new[]
        {
            "-r", "http://localhost/odata", "--auth", "oauth2", "-u", "a-123-456", "-p", "abc-456-xyz",
            "--token-endpoint", "https://dev-identity.com/oauth/token", "--token-params",
            "audience=https://www.todo.com", "-v"
        };

        Fakes.Mock("myFile.txt", cmd);

        //Act
        var cfg = new OptionConfiguration(_fileSystem);
        var args = new[] { "@myFile.txt" };
        var flag = cfg.TryGetConfigurationFile(args, out var args2, out var error, out var fileName);

        var newArgs2 = new EnvReader(_fileSystem)
            .ResolveArgEnv(args2, out var errors);
        Fakes.Remove("myFile.txt");
        //Assert
        flag.Should().BeTrue();
        newArgs2.Should().BeEquivalentTo(expected);
        errors.Should().BeEmpty();

    }
}