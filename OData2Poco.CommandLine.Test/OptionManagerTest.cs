// Copyright (c) Mohamed Hassan & Contributors. All rights reserved. See License.md in the project root for license information.

using FluentAssertions;
using NUnit.Framework;


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
        cs.ServiceUrl.Should().Be(options.ServiceUrl);
        cs.UserName.Should().Be(options.UserName);
        Console.Write(cs.Password);
        // cs.Password.Should().Be(options.Password);
        cs.Password.Should().BeEquivalentTo(options.Password);
        ps.Lang.Should().Be(options.Lang);
        ps.NameCase.Should().Be(options.NameCase);
        ps.Attributes.Should().BeEquivalentTo(options.Attributes);
    }

}