// Copyright (c) Mohamed Hassan & Contributors. All rights reserved. See License.md in the project root for license information.

using System.Net;
using System.Security;
using FluentAssertions;
using NUnit.Framework;

namespace OData2Poco.Tests;

public class SecuredPasswordTest
{
    [Test]
    public void NetworkCredential_to_from_securedString()
    {
        var password = "abc123";

        // 1.String to SecureString
        SecureString theSecureString = new NetworkCredential("", password).SecurePassword;

        //  2.SecureString to String
        string theString = new NetworkCredential("", theSecureString).Password;
        theString.Should().Be(password);

        var secret = password.ConvertToSecureString();
        secret.Should().BeEquivalentTo(theSecureString);
        var nonSecret = secret.ToUnsecureString();
        nonSecret.Should().Be(password);
    }

    [Test]
    public void NetworkCredential_with_secured_password()
    {
        var password = "abc123";
        var user = "user1";
        var cred = password.ToNetworkCredential(user);
        var securePassword = cred.SecurePassword;
        var cred2 = securePassword.ToNetworkCredential(user);
        cred2.Should().BeEquivalentTo(cred);
    }

    [Test]
    public void Round_trip_password()
    {
        var password = "abc123";
        var secret = password.ConvertToSecureString();
        var unSecure = secret.ToUnsecureString();
        password.Should().Be(unSecure);
    }
}