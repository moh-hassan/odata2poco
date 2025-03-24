// Copyright (c) Mohamed Hassan & Contributors. All rights reserved. See License.md in the project root for license information.

namespace OData2Poco.CommandLine.Test;

using System.Net;
using Http;

public class OptionManagerTest
{
    [Test]
    [TestCase("pas")]
    [TestCase("Pas")]
    [TestCase("PAS")]
    [TestCase("camel")]
    [TestCase("none")]
    public void NameCase_valid_Test(string nameCase)
    {
        var result = Enum.TryParse<CaseEnum>(nameCase, true, out var nameCase2);
        Assert.That(result, Is.True);

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
            Attributes =
            [
                "key",
                "json"
            ],
        };
        //Act
        var (cs, ps) = new OptionManager(options);
        //Assert
        cs.ServiceUrl.Should().Be(options.ServiceUrl);
        cs.UserName.Should().Be(options.UserName);
        cs.Password.Should().BeEquivalentTo(options.Password);
        ps?.Lang.Should().Be(options.Lang);
        ps?.NameCase.Should().Be(options.NameCase);
        ps?.Attributes.Should().BeEquivalentTo(options.Attributes);
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
        cs.Password.Should().BeEquivalentTo(options.Password);
        cs.UserName.Should().Be(options.UserName);
        cs.Password.GetBasicAuth(cs.UserName).Should().Be("dXNlcjE6c2VjcmV0");
        cs.GetToken().Should().Be(options.Password.GetRawPassword());
    }

    [Test]
    [TestCase(AuthenticationType.Basic)]
    [TestCase(AuthenticationType.Token)]
    public async Task Option_with_auth_method_should_success_test(AuthenticationType auth)
    {
        //Arrange
        var options = new Options
        {
            ServiceUrl = auth == AuthenticationType.Basic
            ? OdataService.TrippinBasic : OdataService.TrippinBearer,
            Authenticate = auth,
        };
        if (auth == AuthenticationType.Basic)
        {
            options.UserName = "user";
            options.Password = "secret";
        }
        else
        {
            options.Password = "secret_token";
        }
        //Act
        var (cs, _) = new OptionManager(options);
        var client = await CustomHttpClient.CreateAsync(cs).ConfigureAwait(false);
        var response = await client.ReadMetaDataAsync()
            .ConfigureAwait(false);
        var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
        Assert.Multiple(() =>
        {
            //Assert
            Assert.That(cs.Authenticate, Is.EqualTo(options.Authenticate));
            Assert.That(cs.ServiceUrl, Is.EqualTo(options.ServiceUrl));
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(content, Does.Contain("Edm"));
        });
    }

    [Test]
    public void Option_with_fileName_should_configure_lastUpdate()
    {
        var options = new Options
        {
            ServiceUrl = "http://localhost",
            CodeFilename = TestSample.DemoCs
        };
        //Act
        var (cs, ps) = new OptionManager(options);
        Assert.Multiple(() =>
        {
            //Assert
            Assert.That(ps.CodeFilename, Is.EqualTo(options.CodeFilename));
            Assert.That(cs.LastUpdated, Is.EqualTo(ps.GetLastUpdate()));
        });
    }

    [Test]
    public void Option_with_fileName_should_configure_lastUpdate2()
    {
        var options = new Options
        {
            ServiceUrl = "http://localhost",
            CodeFilename = "not_exist_file"
        };
        //Act
        var (cs, ps) = new OptionManager(options);
        Assert.Multiple(() =>
        {
            //Assert
            Assert.That(ps.CodeFilename, Is.EqualTo(options.CodeFilename));
            Assert.That(cs.LastUpdated?.Date, Is.EqualTo(ps.GetLastUpdate()?.Date));
        });
    }
}
