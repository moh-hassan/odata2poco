// Copyright (c) Mohamed Hassan & Contributors. All rights reserved. See License.md in the project root for license information.

namespace OData2Poco.Tests;

using Http;

[TestFixture]
public class StringExtensionsTest : BaseTest
{
    [Test]
    [TestCase("userName", "UserName")]
    [TestCase("user_Name", "UserName")]
    [TestCase("user Name", "UserName")]
    [TestCase("user-Name", "UserName")]
    public void ToPascalCaseTest(string name, string pascalName)
    {
        pascalName.Should().Be(name.ToPascalCase());
    }

    [Test]
    [TestCase("UserName", "userName")]
    [TestCase("user_Name", "userName")]
    [TestCase("User Name", "userName")]
    [TestCase("user-Name", "userName")]
    public void ToCamelCaseTest(string name, string pascalName)
    {
        pascalName.Should().Be(name.ToCamelCase());
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
        text.TrimAllSpace().Should().Be(expected);
    }

    [Test]
    public void TrimAllSpaceAndKeepCrLfTest()
    {
        var text = @"this            is  line1


          and this is     line2";
        var expected = "this is line1\nand this is line2\n";
        text.TrimAllSpace(true).Should().Be(expected);
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
    [TestCase("Authorization=Basic {user1:password1}", "Authorization=Basic dXNlcjE6cGFzc3dvcmQx")]
    [TestCase("Authorization=Basic abc123", "Authorization=Basic abc123")]
    public void ReplaceToBase64_test(string header, string expectedHeader)
    {
        var header2 = header.ReplaceToBase64();
        header2.Should().Be(expectedHeader);
    }

    [Test]
    [TestCase("a*", true)]
    [TestCase("a*c", true)]
    [TestCase("a?c", true)]
    [TestCase("a?c*", true)]
    [TestCase("a?c*", true)]
    [TestCase("?a?c", false)]
    [TestCase("a[b]c", true)]
    [TestCase("a[^d]c", true)]
    [TestCase("*c", true)]
    public void Like_test(string value, bool expected)
    {
        var flag = "abc".Like(value);
        flag.Should().Be(expected);
    }

    [Test]
    [TestCase("a*", false)]
    [TestCase("a*c", false)]
    [TestCase("a?c", false)]
    [TestCase("a?c*", false)]
    [TestCase("a?c*", false)]
    [TestCase("?a?c", true)]
    [TestCase("a[b]c", false)]
    [TestCase("a[^d]c", false)]
    [TestCase("*c", false)]
    public void NotLike_test(string value, bool expected)
    {
        var flag = "abc".NotLike(value);
        flag.Should().Be(expected);
    }

    [Test]
    [TestCase("city", true)]
    [TestCase("trip", true)]
    [TestCase("book", true)]
    [TestCase("product", false)]
    public void In_test(string value, bool expected)
    {
        var flag = value.In("city", "trip", "book");
        flag.Should().Be(expected);
    }

    [Test]
    [TestCase("city", false)]
    [TestCase("trip", false)]
    [TestCase("book", false)]
    [TestCase("product", true)]
    public void NotIn_test(string value, bool expected)
    {
        var flag = value.NotIn("city", "trip", "book");
        flag.Should().Be(expected);
    }

    [Test]
    public void String_with_dot_can_be_reduced_to_last()
    {
        //Arrange
        var sut = "a.b.c";
        var expected = "c";
        //Act
        var actual = sut.Reduce();
        //Assert
        actual.Should().Be(expected);
    }

    [Test]
    [TestCase("a=b", "a", "b")]
    [TestCase("a=", "a", "")]
    [TestCase("a", "a", "")]
    [TestCase(null, "", "")]
    public void Key_value_pair_should_split(string sut, string key, string value)
    {
        //Arrange
        //Act
        var (k, v) = sut.SplitKeyValue();
        //Assert
        k.Should().BeEquivalentTo(key);
        v.Should().BeEquivalentTo(value);
    }
}
