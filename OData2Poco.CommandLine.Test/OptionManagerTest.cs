// Copyright (c) Mohamed Hassan & Contributors. All rights reserved. See License.md in the project root for license information.

using System.Net.Sockets;
using FluentAssertions;
using NUnit.Framework;
using OData2Poco.Extensions;
using OData2Poco.Http;


namespace OData2Poco.CommandLine.Test;

public class OptionManagerTest
{
    [Test]
    [TestCase("pas")]
    [TestCase("PAS")]
    [TestCase("camel")]
    [TestCase("none")]
    public void NameCase_valid_Test(string nameCase)
    {
        Enum.TryParse<CaseEnum>(nameCase, out var nameCase2);
        var options = new Options
        {
            Lang = Language.CS,
            NameCase = nameCase2
        };
        _ = new OptionManager(options);
        Assert.That(options.Errors, Is.Empty);

    }

    [Test]
    public void Destruct_OptionManager_test()
    {
        //Arrange
        var options = new Options
        {
            ServiceUrl = "http://localhost",
            UserName = "user1",
            Password = "secret",
            Lang = Language.TS,
            NameCase = CaseEnum.Camel,
            Attributes = new[] { "key", "json" },
        };
        //Act
        var (cs, ps) = new OptionManager(options);
        //Assert
       // cs.Password.Credential.Password.Should().HaveLength(6);
        //cs.Password.Credential.Password.Should().BeEquivalentTo(options.Password.Credential.Password);
        cs.ServiceUrl.Should().Be(options.ServiceUrl);
        cs.UserName.Should().Be(options.UserName);
        cs.Password.Should().BeEquivalentTo(options.Password);
        ps.Lang.Should().Be(options.Lang);
        ps.NameCase.Should().Be(options.NameCase);
        ps.Attributes.Should().BeEquivalentTo(options.Attributes);
    }
    [Test]
    public void Option_with_password_should_be_serialized_deserialized_correctly_test()
    {
        //Arrange
        var options = new Options
        {
            ServiceUrl = "http://localhost",
            UserName = "user1",
            Password = "secret",
        };
        //Act
        var (cs, _) = new OptionManager(options);
        //Assert
        Console.WriteLine($"cs user: {cs.UserName} | pw: {cs.Domain}");
        cs.Password.Should().BeEquivalentTo(options.Password);
        cs.UserName.Should().Be(options.UserName);
        
        //set user, domain
       // cs.ToCredential().Should().BeEquivalentTo(cs.Password.Credential);
        cs.Password.GetBasicAuth(cs.UserName).Should().Be("dXNlcjE6c2VjcmV0");
        //cs.UserName.Should().Be(cs.GetCredential().UserName);
        //cs.Password.Credential.Password.Should().HaveLength(6)
        //    .And.BeEquivalentTo(options.Password.Credential.Password);
        //cs.Password.GetSecurePassword().Should().BeEquivalentTo(options.Password.GetSecurePassword());
    }

    [Test]
    [TestCase(AuthenticationType.Basic, "Basic dXNlcjE6c2VjcmV0")]
    [TestCase(AuthenticationType.Token, "Bearer secret")]
    public async Task Option_with_auth_method_should_success_test(
        AuthenticationType auth, string expected)
    {
        //test flow option->optionManager->CustomHttpClient
        //Arrange
        var options = new Options
        {
            ServiceUrl = "http://localhost",
            UserName = "user1",
            Password = "secret",
            Authenticate = auth,
        };
        //Act
        var (cs, _) = new OptionManager(options);
        var dh = new AuthHandler();
        var client = cs.ToCustomHttpClient(dh);
        await client.GetAsync("http://localhost2");
        //Assert
        Console.WriteLine($"scheme: {dh.Scheme} parameter: {dh.Parameter}");
        dh.AuthHeader.Should().NotBeNull();
        dh.AuthHeader.ToString().Should().BeEquivalentTo(expected);
    }
}