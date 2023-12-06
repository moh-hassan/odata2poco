// Copyright (c) Mohamed Hassan & Contributors. All rights reserved. See License.md in the project root for license information.

using FluentAssertions;
using NUnit.Framework;
using OData2Poco.CustAttributes;


namespace OData2Poco.Tests;

[TestFixture]
internal class NamedAttributeTest
{

    //common data
    private readonly List<string> _attributeList = new() { "dm", "db", "display", "json", "key", "req", "tab", "proto" };

    #region Property Attributes

    //attribute key   
    [Test]
    public void PropertyTemplate_with_key_true_Test()
    {
        // Arrange 
        PropertyTemplate p = new PropertyTemplate
        {
            PropName = "FirstName",
            PropType = "string",
            IsKey = true,
            Serial = 1,
        };

        //Act
        var sut = AttributeFactory.Default.GetAttributes(p, "key");
        var att = string.Join(" ", sut);
        //Assert        
        Assert.That(att, Does.Contain("[Key"));
    }

    [Test]
    public void PropertyTemplate_with_key_false_Test()
    {
        // Arrange 
        PropertyTemplate p = new PropertyTemplate
        {
            PropName = "FirstName",
            PropType = "string",
            IsKey = false,
            Serial = 1,
        };

        //Act
        var sut = AttributeFactory.Default.GetAttributes(p, "key");
        var att = string.Join(" ", sut);
        //Assert       
        Assert.That(att, Is.Empty);
    }

    //attribute req
    [Test]
    public void PropertyTemplate_with_Required_true_Test()
    {
        // Arrange 
        PropertyTemplate p = new PropertyTemplate
        {
            PropName = "FirstName",
            PropType = "string",
            //IsKey = false,
            IsNullable = false,
            Serial = 1,
        };

        //Act
        var sut = AttributeFactory.Default.GetAttributes(p, "req");
        var att = string.Join(" ", sut);
        //Assert      
        Assert.That(att, Does.Contain("[Required]"));
    }

    [Test]
    public void PropertyTemplate_with_Required_false_Test()
    {
        // Arrange 
        PropertyTemplate p = new PropertyTemplate
        {
            PropName = "FirstName",
            PropType = "string",
            IsNullable = true,
            Serial = 1,
        };

        //Act
        var sut = AttributeFactory.Default.GetAttributes(p, "req");
        var att = string.Join(" ", sut);
        //Assert       
        Assert.That(att, Is.Empty);
    }

    [Test]
    public void PropertyTemplate_with_json_Test()
    {
        // Arrange 
        PropertyTemplate p = new PropertyTemplate
        {
            PropName = "FirstName",
            PropType = "string",
            //IsNullable = true,
            //Serial = 1,
        };

        //Act
        var sut = AttributeFactory.Default.GetAttributes(p, "json");
        var att = string.Join(" ", sut);
        //Assert
        var expected = "[JsonProperty(PropertyName = \"FirstName\")]";
        Assert.That(att, Does.Contain(expected));
    }
    [Test]
    public void PropertyTemplate_with_datamember_Test()
    {
        // Arrange 
        PropertyTemplate p = new PropertyTemplate
        {
            PropName = "FirstName",
            PropType = "string",
            //IsNullable = true,
            //Serial = 1,
        };

        //Act
        var sut = AttributeFactory.Default.GetAttributes(p, "dm");
        var att = string.Join(" ", sut);
        //Assert
        var expected = "[DataMember]";
        Assert.That(att, Does.Contain(expected));
    }
    [Test]
    public void PropertyTemplate_with_proto_Test()
    {
        // Arrange 
        PropertyTemplate p = new PropertyTemplate
        {
            PropName = "FirstName",
            PropType = "string",
            //IsNullable = true,
            Serial = 3,
        };

        //Act
        var sut = AttributeFactory.Default.GetAttributes(p, "proto");
        var att = string.Join(" ", sut);
        //Assert
        var expected = "[ProtoMember(3)]";
        Assert.That(att, Does.Contain(expected));
    }
    [Test]
    public void PropertyTemplate_with_display_Test()
    {
        // Arrange 
        PropertyTemplate p = new PropertyTemplate
        {
            PropName = "FirstName",
            PropType = "string",
            //IsNullable = true,
            Serial = 3,
        };

        //Act
        var sut = AttributeFactory.Default.GetAttributes(p, "display");
        var att = string.Join(" ", sut);
        //Assert
        var expected = "[Display(Name = \"First Name\")]";
        Assert.That(att, Does.Contain(expected));
    }
    [Test]
    public void PropertyTemplate_with_db_Test()
    {
        // Arrange 
        PropertyTemplate p = new PropertyTemplate
        {
            PropName = "FirstName",
            PropType = "string",
            //IsNullable = false, //IsNullable is false by default
            IsKey = true,
        };

        //Act
        var sut = AttributeFactory.Default.GetAttributes(p, "db");
        var att = string.Join(" ", sut);
        //Assert
        var expected = "[Key] [Required]";
        Assert.That(att, Does.Contain(expected));
    }
    [Test]
    public void PropertyTemplate_with_table_Test()
    {
        // Arrange 
        PropertyTemplate p = new PropertyTemplate
        {
            PropName = "FirstName",
            PropType = "string",
            //IsNullable = false, //IsNullable is false by default
            IsKey = true,
        };

        //Act
        var sut = AttributeFactory.Default.GetAttributes(p, "tab");
        var att = string.Join(" ", sut);
        //Assert       
        Assert.That(att, Is.Empty);
    }


    [Test]
    public void PropertyTemplate_with_setting_key_Test()
    {
        // Arrange 
        PropertyTemplate p = new PropertyTemplate
        {
            PropName = "FirstName",
            PropType = "string",
            IsKey = true,
        };

        //Act
        var sut = AttributeFactory.Default
            .Init()
            .GetAllAttributes(p);
        var att = string.Join(" ", sut);
        //Assert      
        Assert.That(att, Is.Empty);
    }


    #endregion

    [Test]
    [TestCase("key", "[Key]")]
    [TestCase("req", "[Required]")]
    [TestCase("dm", "[DataMember]")]
    [TestCase("display", "[Display(Name = \"First Name\")]")]
    [TestCase("json", "[JsonProperty(PropertyName = \"FirstName\")]")]
    [TestCase("tab", "")]
    [TestCase("db", "[Key] [Required]")]
    [TestCase("proto", "[ProtoMember(1)]")]
    public void PropertyTemplate_with_seeting_Attribute_Test(string name, string expected)
    {
        // Arrange 
        PropertyTemplate p = new PropertyTemplate
        {
            PropName = "FirstName",
            PropType = "string",
            IsKey = true,
            Serial = 1,
        };

        //Act
        var setting = new PocoSetting
        {
            Attributes = new List<string> { name }
        };

        var sut = AttributeFactory.Default
            .Init(setting)
            .GetAllAttributes(p);
        var att = string.Join(" ", sut);
        //Assert
        expected.Should().Be(att);
    }

    [Test]
    [TestCase("key", "")]
    [TestCase("req", "")]
    [TestCase("dm", "[DataContract]")]
    [TestCase("display", "")]
    [TestCase("json", "")]
    [TestCase("tab", "[Table(\"productDetail\")]")]
    [TestCase("db", "[Table(\"productDetail\")]")]
    [TestCase("proto", "[ProtoContract]")]
    public void ClasTemplatewith_seeting_Attribute_Test(string name, string value)
    {
        // Arrange 
        var p = new ClassTemplate(1)
        {
            Name = "ProductDetail",
            EntitySetName = "productDetail"
        };

        //Act
        var setting = new PocoSetting
        {
            Attributes = new List<string> { name }
        };
        var sut = AttributeFactory.Default
            .Init(setting)
            .GetAllAttributes(p);
        var att = string.Join(" ", sut);
        //Assert
        value.Should().Be(att);
    }

    private bool ListCheck<T>(IEnumerable<T> l1, IEnumerable<T> l2)
    {
        if (l1.Intersect(l2).Any())
        {
            return true;
        }

        return false;
    }
    [Test]
    public void PropertyTemplate_All_Attributes_Test()
    {
        // Arrange 
        PropertyTemplate p = new PropertyTemplate
        {
            PropName = "FirstName",
            PropType = "string",
            IsKey = true,
            Serial = 1,
        };


        var atts = AttributeFactory.Default.GetAttributes(p, _attributeList);

        var match = ListCheck(atts, new List<string>
        {
            "[ProtoMember(1)]",
            "[DataMember]" ,
            " [Key]",
            "[Required]",
            "[Display(Name = \"First Name\")]",
            "[JsonProperty(PropertyName = \"FirstName\")]",

        });
        Assert.That(match, Is.True);
    }

    [Test]
    public void ClassTemplate_All_Attributes_Test()
    {
        // Arrange 
        var p = new ClassTemplate(1)
        {
            Name = "ProductDetail",
            EntitySetName = "productDetail"
        };


        var atts = AttributeFactory.Default.GetAttributes(p, _attributeList);

        var match = ListCheck(atts, new List<string>
        {
            "[DataContract]",
            "[Table(\"productDetail\")]",
            "[ProtoContract]",
        });
        Assert.That(match, Is.True);
    }
    //init factory test
    [Test]
    public void PropertyTemplate_All_Attributes_With_SettingTest()
    {
        // Arrange 
        PropertyTemplate p = new PropertyTemplate
        {
            PropName = "FirstName",
            PropType = "string",
            IsKey = true,
            Serial = 1,
        };

        var setting = new PocoSetting()
        {
            Attributes = new List<string>(_attributeList),
        };

        var atts = AttributeFactory.Default
            .Init(setting)
            .GetAllAttributes(p);

        var match = ListCheck(atts, new List<string>
        {
            "[ProtoMember(1)]",
            "[DataMember]" ,
            " [Key]",
            "[Required]",
            "[Display(Name = \"First Name\")]",
            "[JsonProperty(PropertyName = \"FirstName\")]",

        });
        Assert.That(match, Is.True);

    }

    [Test]
    public void ClassTemplate_All_Attributes_With_SettingTest()
    {
        // Arrange 
        var p = new ClassTemplate(1)
        {
            Name = "ProductDetail",
            EntitySetName = "productDetail"
        };

        var setting = new PocoSetting()
        {
            Attributes = new List<string>(_attributeList),
        };

        var atts = AttributeFactory.Default
            .Init(setting)
            .GetAllAttributes(p);

        var match = ListCheck(atts, new List<string>
        {
            "[DataContract]",
            "[Table(\"productDetail\")]",
            "[ProtoContract]",

        });
        Assert.That(match, Is.True);


    }
    [Test]
    public void PocoAttributesList_supported_attributes_test()
    {
        var atts = new PocoAttributesList().SupportedAttributes();
        Assert.That(atts, Has.Count.GreaterThan(0));
    }

}