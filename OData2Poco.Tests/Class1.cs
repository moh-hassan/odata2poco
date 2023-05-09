// Copyright (c) Mohamed Hassan & Contributors. All rights reserved. See License.md in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;

namespace OData2Poco.Tests;

public  class Class1
{
    [Test]
    public void Test1()
    {
        var s = "abc123";
        // here is also another way to convert between SecureString and String.

        // 1.String to SecureString

        SecureString theSecureString = new NetworkCredential("", s).SecurePassword;
        //  2.SecureString to String

        string theString = new NetworkCredential("", theSecureString).Password;
        Console.WriteLine(theString);
        //--------------
        var secret = s.ConvertToSecureString();
        var non_secret = secret.ToUnsecureString();
        Console.WriteLine(non_secret);
    }

    [Test]
    public void Test2()
    {
        var s = "abc123";
        var user = "user1";
        var cred = s.ToNetworkCredential(user);
        Console.WriteLine($"cred.Password {cred.Password}");
        Console.WriteLine($"cred.user {cred.UserName}");

        var secret = cred.SecurePassword;
        var cred2 = secret.ToNetworkCredential(user);
        Console.WriteLine($"cred2.Password {cred2.Password}");
        Console.WriteLine($"cred.user {cred2.UserName}");
    }

    [Test]
    public void Test3()
    {
        var s = "abc123";
        var user = "user1";
        var secret = s.ConvertToSecureString();
        var s2 = secret.ToUnsecureString();
        Console.WriteLine($"ToUnsecureString {s2}");
        s.Should().Be(s2);
    }
}