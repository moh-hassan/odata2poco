// Copyright (c) Mohamed Hassan & Contributors. All rights reserved. See License.md in the project root for license information.

using System.Diagnostics;
using FluentAssertions;
using NUnit.Framework;
using OData2Poco.Extensions;
using OData2Poco.Http;

namespace OData2Poco.Tests;

[TestFixture]
internal class StringExtensionsTest : BaseTest
{
    [Test]
    [TestCase("userName", "UserName")]
    [TestCase("user_Name", "UserName")]
    [TestCase("user Name", "UserName")]
    [TestCase("user-Name", "UserName")]
    public void ToPascalCaseTest(string name, string pascalName)
    {
        Assert.AreEqual(pascalName, name.ToPascalCase());
    }

    [Test]
    [TestCase("UserName", "userName")]
    [TestCase("user_Name", "userName")]
    [TestCase("User Name", "userName")]
    [TestCase("user-Name", "userName")]
    public void ToCamelCaseTest(string name, string pascalName)
    {
        Assert.AreEqual(pascalName, name.ToCamelCase());
    }

    [Test]
    [TestCase("UserName", "user-name")]
    [TestCase("user_Name", "user_name")]
    [TestCase("Username", "username")]
    public void ToKebaCaseTest(string name, string kebabName)
    {
        kebabName.Should().Be(name.ToKebabCase());
    }


    [Test]
    public void TrimAllSpaceAndCrLfTest()
    {
        var text = @"this            is  line1


          and this is     line2";

        var expected = "this is line1 and this is line2";
        Assert.AreEqual(text.TrimAllSpace(), expected);
    }

    [Test]
    public void TrimAllSpaceAndKeepCrLfTest()
    {
        var text = @"this            is  line1


          and this is     line2";
        var expected = "this is line1\nand this is line2\n";
        Assert.AreEqual(text.TrimAllSpace(true), expected);
    }
    [Test]
    [TestCase("Apple", "apple")]
    [TestCase("apple", "Apple")]
    [TestCase("", "")]
    [TestCase(null, null)]
    public void ToggleFirstLetterTest(string name, string expected)
    {
        var name2 = name.ToggleFirstLetter();
        Assert.That(name2, Is.EqualTo(expected));
    }

    [Test]
    [TestCase("event", "Event")]
    [TestCase("void", "Void")]
    [TestCase("Void", "Void")]
    [TestCase("char", "Char")]
    [TestCase("Char", "Char")]
    [TestCase("not_reserved", "not_reserved")]
    [TestCase("", "")]
    [TestCase(null, null)]
    public void ChangeReservedWordTest(string name, string expected)
    {
        var name2 = name.ChangeReservedWord();
        Assert.That(name2, Is.EqualTo(expected));
    }

    [Test]
    [TestCase("camel", CaseEnum.Camel)]
    [TestCase("CAMEL", CaseEnum.Camel)]
    [TestCase("pas", CaseEnum.Pas)]
    [TestCase("anyvalue", CaseEnum.None)]
    public void StringToCaseEnumTest(string val, CaseEnum expected)
    {
        var enumValue = val.ToEnum<CaseEnum>();
        //Assert
        Assert.That(enumValue, Is.EqualTo(expected));
    }

    [Test]
    [TestCase("basic", AuthenticationType.Basic)]
    [TestCase("BASIC", AuthenticationType.Basic)]
    [TestCase("undefined", AuthenticationType.None)]
    public void AuthenticationTypeTest(string auth, AuthenticationType expected)
    {
        //Arrange
        //Act
        AuthenticationType? authType = auth.ToEnum<AuthenticationType>();
        //Assert
        Assert.That(authType, Is.EqualTo(expected));
    }

    [Test]
    [TestCase(typeof(AuthenticationType), "None, Basic, Token, Oauth2, Ntlm, Digest")]
    [TestCase(typeof(Language), "None, CS, TS")]
    public void Enum_to_string_values_test(Type t, string expected)
    {
        var sut = t.EnumToString();
        sut.Should().Be(expected);
    }

    [Test]
    [TestCase("Authorization=Basic {user1:password1}", "Authorization=Basic dXNlcjE6cGFzc3dvcmQx",true)]
    [TestCase("Authorization=Basic abc123", "Authorization=Basic abc123", false)]
    public void ReplaceToBase64_test(string header, string expectedHeader, bool expectedFlag)
    {
        var flag = header.TryReplaceToBase64(out var header2);
        flag.Should().Be(expectedFlag);
        header2.Should().Be(expectedHeader);

    }
}