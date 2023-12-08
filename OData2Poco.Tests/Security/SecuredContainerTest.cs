// Copyright (c) Mohamed Hassan & Contributors. All rights reserved. See License.md in the project root for license information.

namespace OData2Poco.Tests;

[TestFixture]
public class SecuredContainerTest : BaseTest
{
    [Test]
    [TestCase("pass@123")]
    public void Round_trip_string_password(string password)
    {
        //Arrange
        var sut = new SecurityContainer(password);
        //Act
        var password2 = sut.GetRawPassword();
        //Assert
        sut.IsSecuredString.Should().BeFalse();
        sut.Credential.Password.Should().NotBeEmpty();
        password2.Should().Be(password);
    }

    [Test]
    [TestCase("pass@123")]
    public void Round_trip_string_password_with_assignment(string password)
    {
        //Arrange
        SecurityContainer sut = password;
        //Act
        var password2 = sut.GetRawPassword();
        //Assert
        sut.IsSecuredString.Should().BeFalse();
        sut.Credential.Password.Should().NotBeEmpty();
        password2.Should().Be(password);
    }
    [Test]
    [TestCase("pass@123")]
    public void Round_trip_secured_string_password(string password)
    {
        //Arrange
        var secret = password.ToSecureString();
        var sut = new SecurityContainer(secret);
        //Act
        var password2 = sut.GetRawPassword();
        //Assert
        sut.IsSecuredString.Should().BeTrue();
        password2.Should().Be(password);
    }

    [Test]
    [TestCase("pass@123")]
    public void Round_trip_secured_string_password_with_assignment(string password)
    {
        //Arrange
        var secret = password.ToSecureString();
        SecurityContainer sut = secret;
        //Act
        var password2 = sut.GetRawPassword();
        //Assert
        sut.IsSecuredString.Should().BeTrue();
        password2.Should().Be(password);
    }
    [Test]
    public void Get_tokens_test()
    {
        //Arrange
        string password = "pass@123";
        string user = "user1";
        var sut = new SecurityContainer(password);
        //Act
        var basicAuth = sut.GetBasicAuth(user);
        var token = sut.GetToken();
        //Assert
        sut.Credential.Password.Should().NotBeEmpty();
        basicAuth.Should().Be("dXNlcjE6cGFzc0AxMjM=");
        basicAuth.FromBase64().Should().Be($"{user}:{sut.GetRawPassword()}");
        token.Should().Be(password);
    }
}
