// Copyright (c) Mohamed Hassan & Contributors. All rights reserved. See License.md in the project root for license information.

using FluentAssertions;
using NUnit.Framework;

namespace OData2Poco.Tests;

public class SecuredPasswordTest
{
    [Test]
    [TestCase("secret")]
    [TestCase("")]
    [TestCase(null)]
    public void Credential_securedString_test(string password)
    {
        //Arrange
        var secret = SecuredContainer.ToSecureString(password);
        //Act
        var sp = new SecuredContainer(secret);

        //Assert
        sp.Credential.SecurePassword.Length.Should().Be(secret.Length);
        sp.Credential.SecurePassword.Should().BeEquivalentTo(secret);
    }
    [Test]
    [TestCase("secret", "secret")]
    [TestCase("", "")]
    [TestCase(null, "")]
    public void Credential_password_test(string password, string expected)
    {
        //Arrange
        //Act
        var sp = new SecuredContainer(password);
        //Assert
        sp.Credential.Password.Should().Be(expected);
        sp.Credential.SecurePassword.Length.Should().Be(expected.Length);
    }

    [Test]
    [TestCase("pass123")]
    public void Round_trip_password(string password)
    {
        //Arrange
        var secret = SecuredContainer.ToSecureString(password);
        //Act
        var sp = new SecuredContainer(secret);
        //Assert
        sp.Credential.Password.Should().Be(password);
    }

}