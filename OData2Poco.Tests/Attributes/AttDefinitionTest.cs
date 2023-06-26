// Copyright (c) Mohamed Hassan & Contributors. All rights reserved. See License.md in the project root for license information.

using FluentAssertions;
using NUnit.Framework;
using OData2Poco.CustAttributes;
using OData2Poco.CustAttributes.UserAttributes;

namespace OData2Poco.Tests.Attributes;
using static Fake.TestSample;


[Category("UserAttributes")]
public class AttDefinitionTest : BaseTest
{
    private readonly string _attFilePath = Path.Combine(FakeFolder, "attributes.txt");
    private string _text;

    private readonly Dictionary<string, string> _attributes = new()
    {
        ["_json_"] = """[JsonProperty(PropertyName = "ProductId")]""",
        ["_json3_"] = """[JsonPropertyName("ProductId")]""",
        ["_key_"] = """[Key]""",
        ["_Required_"] = """[Required]""",
        ["_dm_"] = """[DataMember]""",
        ["_dm2_"] = """[DataContract]""",    //class
        ["_tab_"] = """[Table("Products")]""", //class
    };

    [OneTimeSetUp]
    public void Setup()
    {
        _text = File.ReadAllText(_attFilePath);
    }

    [Test]
    [TestCase("book", "[DataMember]")]
    [TestCase("product", null)]
    public void AttDefinition_filter_with_in_className_test(string className, string expected)
    {
        var ad = new AttDefinition
        {
            Name = "dm3",
            Format = "[DataMember]",
            Filter = """ PropName.In("id", "Isbn") && ClassName == "book"  """
        };
        var p = new PropertyTemplate
        {
            PropName = "isbn",
            PropType = "int",
            Serial = 1,
            ClassName = className
        };
        var att = ad.ToAttribute(p);
        att.FirstOrDefault().Should().Be(expected);
    }

    [TestCase("book", "[DataMember]")]
    [TestCase("product", null)]
    [TestCase("customer", "[DataMember]")]
    [TestCase("city", "[DataMember]")]
    public void AttDefinition_filter_with_like_className_test(string className, string expected)
    {
        var ad = new AttDefinition
        {
            Name = "dm3",
            Format = "[DataMember]",
            Filter = """ PropName.In("id", "Isbn") && ClassName.Like("cust*","ci?y", "b*")  """
        };
        var p = new PropertyTemplate
        {
            PropName = "isbn",
            PropType = "int",
            Serial = 1,
            ClassName = className
        };
        var att = ad.ToAttribute(p);
        att.FirstOrDefault().Should().Be(expected);
    }
    [Test]
    public void AttDefinition_import_text_test()
    {
        var prop = new PropertyTemplate
        {
            PropName = "ProductId",
            PropType = "int",
            IsKey = true,
            Serial = 1,
        };
        var ct = new ClassTemplate(1)
        {
            Name = "Product",
            EntitySetName = "Products",
        };
        var attDefinition = AttDefinition.Import(_text);
        attDefinition.Should().NotBeEmpty();
        foreach (var att in attDefinition)
        {
            object obj = att.Scope == "property" ? prop : ct;
            att.ToAttribute(obj).Should().NotBeEmpty();
            att.ToAttribute(obj).FirstOrDefault().Should().BeEquivalentTo(_attributes[att.Name]);
        }
    }

    private void AssertAttributes(AttributeFactory af, PropertyTemplate pp, ClassTemplate cc)
    {
        var afAttributes = af.GetAttributeList()
            .Where(a => a.IsUserDefined)
            .Select(a => (a.Name, F(a)))
            .ToDictionary(a => a.Name, a => a.Item2);
        afAttributes.Should().NotBeEmpty();
        afAttributes.Should().BeEquivalentTo(_attributes);

        //local function
        string F(INamedAttribute ina) => ina.Scope switch
        {
            "property" => ina.GetAttributes(pp).FirstOrDefault(),
            "class" => ina.GetAttributes(cc).FirstOrDefault(),
            _ => null
        };
    }

    [Test]
    public void AttributeFactory_load_attDefinitions_from_pocoSetting_properly()
    {
        //Arrange
        AttributeFactory af = AttributeFactory.Default;
        var ps = new PocoSetting { AtributeDefs = _attFilePath };
        //initialize to load attributes from json
        af.Init(ps);
        var prop = new PropertyTemplate
        {
            PropName = "ProductId",
            PropType = "int",
            IsKey = true,
            Serial = 1,
        };
        var ct = new ClassTemplate(1)
        {
            Name = "Product",
            EntitySetName = "Products",
        };
        //Act
        var attDefinition = AttDefinition.Import(_text);
        //Assert
        attDefinition.Select(x => af.IsExists(x.Name)).Should().NotContain(false);
        AssertAttributes(af, prop, ct);
    }

    [Test]
    public void Json_attribute_test()
    {
        //Arrange
        var ad = new AttDefinition
        {
            Name = "json2",
            Scope = "property",
            Format = "[JsonPropertyName({{PropName.ToCamelCase().Quote()}})]",
        };
        var p = new PropertyTemplate
        {
            PropName = "FirstName",
            PropType = "string",
            Serial = 1,
        };
        var expected = """[JsonPropertyName("firstName")]""";
        //Act
        var atts = ad.ToAttribute(p);
        //Assert
        Assert.AreEqual(expected, atts.FirstOrDefault());
    }

    [Test]
    [TestCase(true, "[Key]")]
    [TestCase(false, null)]
    public void Key_attribute_test(bool isKey, string expected)
    {
        //Arrange
        var ad = new AttDefinition
        {
            Name = "Key2",
            Scope = "property",
            Format = "[Key]",
            Filter = "IsKey"
        };
        var p = new PropertyTemplate
        {
            PropName = "ProductId",
            PropType = "int",
            IsKey = isKey,
            Serial = 1,
        };
        //Act
        var atts = ad.ToAttribute(p);
        //Assert
        Assert.AreEqual(expected, atts.FirstOrDefault());
    }

    [Test]
    [TestCase(false, "[Required]")]
    [TestCase(true, null)]
    public void Required_attribute_test(bool isNull, string expected)
    {
        //Arrange
        var ad = new AttDefinition
        {
            Name = "required2",
            Scope = "property",
            Format = "[Required]",
            Filter = "!IsNullable"
        };
        var p = new PropertyTemplate
        {
            PropName = "ProductId",
            PropType = "int",
            IsKey = false,
            IsNullable = isNull,
            Serial = 1,
        };

        //Act
        var atts = ad.ToAttribute(p);
        //Assert
        Assert.AreEqual(expected, atts.FirstOrDefault());
    }

    [Test]
    public void Class_attribute_test()
    {
        //Arrange
        var ad = new AttDefinition
        {
            Name = "_tab_",
            Scope = "class",
            Format = "[Table({{EntitySetName.Quote()}})]"
        };
        var p = new ClassTemplate(1)
        {
            Name = "Product",
            EntitySetName = "Products",

        };
        var expected = """[Table("Products")]""";
        //Act
        var atts = ad.ToAttribute(p);
        //Assert
        Assert.AreEqual(expected, atts.FirstOrDefault());
    }

    [Test]
    public void Export_test()
    {
        //Arrange
        var ad = new AttDefinition
        {
            Name = "json2",
            Scope = "property",
            Format = "[DataMember]",
        };
        var expected = @"[json2]
Scope=property
Format=[DataMember]
";
        //Act
        var sb = ad.Export();
        //Assert
        Assert.AreEqual(expected, sb.ToString());

    }
}