// Copyright (c) Mohamed Hassan & Contributors. All rights reserved. See License.md in the project root for license information.

namespace OData2Poco.Tests.TypeScript;

using OData2Poco.TypeScript;

[Category("typescript")]
public class TsPropertyGeneratorTest : BaseTest
{
    [Test]
    // [Category("typescript")]
    [TestCase("string", "public Property1: string;")]
    [TestCase("int", "public Property1: number;")]
    [TestCase("float", "public Property1: number;")]
    public void Property_data_type_test(string type, string expected)
    {
        //Arrange
        PropertyTemplate p = new()
        {
            PropName = "Property1",
            PropType = type,
            IsNullable = true
        };
        ClassTemplate ct = new(1)
        {
            Name = "A",
            NameSpace = "N",
            Properties = [p]
        };
        PocoSetting setting = new()
        {
            // AddNavigation = true,
            Lang = Language.TS
        };
        var builder = new TsClassBuilder(ct, setting);
        //Act
        string sut = new TsPropertyBuilder(p, builder);
        //Assert
        sut.Should().Be(expected);
    }

    [Test]
    [TestCase(CaseEnum.Camel, "public userName: string;")]
    [TestCase(CaseEnum.Pas, "public UserName: string;")]
    [TestCase(CaseEnum.Kebab, "public user-name: string;")]
    [TestCase(CaseEnum.Snake, "public user_name: string;")]
    [TestCase(CaseEnum.None, "public UserName: string;")]
    public void Property_change_case_test(CaseEnum caseValue, string expected)
    {
        //Arrange
        PropertyTemplate p = new()
        {
            PropName = "UserName",
            PropType = "string",
            IsNullable = true
        };
        PocoSetting setting = new()
        {
            NameCase = caseValue
        };
        ClassTemplate ct = new(1)
        {
            Name = "A",
            NameSpace = "N",
            Properties = [p]
        };
        var builder = new TsClassBuilder(ct, setting);
        //Act
        string sut = new TsPropertyBuilder(p, builder);
        //Assert
        sut.Should().Be(expected);
    }

    [Test]
    [TestCase(GeneratorType.Class, "public ProductName: string;")]
    [TestCase(GeneratorType.Interface, "ProductName: string;")]
    public void Generate_property_for_interface_or_class_test(GeneratorType type,
        string expected)
    {
        //Arrange
        PropertyTemplate p = new()
        {
            PropName = "ProductName",
            PropType = "string",
            IsNullable = true
        };
        PocoSetting setting = new()
        {
            GeneratorType = type
        };
        ClassTemplate ct = new(1)
        {
            Name = "A",
            Properties = [p]
        };
        var builder = new TsClassBuilder(ct, setting);
        //Act
        string sut = new TsPropertyBuilder(p, builder);
        //Assert
        sut.Should().Be(expected);
    }

    [Test]
    [TestCase("string", "public Property1?: string;")]
    [TestCase("int", "public Property1?: number;")]
    [TestCase("float", "public Property1?: number;")]
    public void Generate_property_test_null(string type, string expected)
    {
        //Arrange
        PropertyTemplate p = new()
        {
            PropName = "Property1",
            PropType = type,
            IsNullable = true
        };
        PocoSetting setting = new()
        {
            AddNullableDataType = true
        };
        ClassTemplate ct = new(1)
        {
            Name = "A",
            Properties = [p]
        };
        var builder = new TsClassBuilder(ct, setting);
        //Act
        string sut = new TsPropertyBuilder(p, builder);
        //Assert
        sut.Should().Be(expected);
    }

    [Test]
    [TestCase(true, "string", "// public Product: string; //navigator")]
    [TestCase(false, "string", "public Product: string;")]
    public void Generate_property_navigate_false_test(bool isNavigate, string type, string expected)
    {
        //Arrange
        PropertyTemplate p = new()
        {
            PropName = "Product",
            PropType = type,
            IsNavigate = isNavigate,
            IsNullable = true
        };
        PocoSetting setting = new()
        {
            AddNavigation = false
        };
        ClassTemplate ct = new(1)
        {
            Name = "A",
            Properties = [p]
        };
        var builder = new TsClassBuilder(ct, setting);
        //Act
        string sut = new TsPropertyBuilder(p, builder);
        //Assert
        sut.Should().Be(expected);
    }

    [Test]
    [TestCase(true, "string", "public Product: string; //navigator")]
    [TestCase(false, "string", "public Product: string;")]
    public void Generate_property_navigate_true_test(bool isNavigate, string type, string expected)
    {
        //Arrange
        PropertyTemplate p = new()
        {
            PropName = "Product",
            PropType = type,
            IsNavigate = isNavigate,
            IsNullable = true
        };
        PocoSetting setting = new()
        {
            AddNavigation = true
        };
        ClassTemplate ct = new(1)
        {
            Name = "A",
            NameSpace = "N",
            Properties = [p]
        };
        var builder = new TsClassBuilder(ct, setting);
        //Act
        string sut = new TsPropertyBuilder(p, builder);
        //Assert
        sut.Should().Be(expected);
    }
}
