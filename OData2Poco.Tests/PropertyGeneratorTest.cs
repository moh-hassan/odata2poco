// Copyright (c) Mohamed Hassan & Contributors. All rights reserved. See License.md in the project root for license information.

using OData2Poco.CustAttributes;
using OData2Poco.TestUtility;

namespace OData2Poco.Tests;

[Category("property_generation")]
[TestFixture]
internal class PropertyGeneratorTest
{
    private AttributeFactory _attributeManager;
    [OneTimeSetUp]
    public void Init()
    {
        _attributeManager = AttributeFactory.Default;
    }
    [Test]
    public void DefaultPropertyDeclaration_test()
    {
        PropertyTemplate property = new PropertyTemplate
        {
            PropName = "CategoryID",
            PropType = "int",
            IsKey = true
        };
        var pg = new PropertyGenerator(property, new PocoSetting());

        Assert.That(pg.Declaration, Does.Contain("public int CategoryID {get;set;}"));
    }
    [Test]
    public void AllAttributesPropertyDeclaration_test()
    {
        // Arrange 
        var setting = new PocoSetting
        {
            Attributes = ["key", "req", "json"],
        };
        _attributeManager.Init(setting);

        PropertyTemplate property = new PropertyTemplate
        {
            PropName = "CategoryID",
            PropType = "int",
            IsKey = true
        };

        // Act 
        var sut = new PropertyGenerator(property, setting);


        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(sut.ToString(), Does.Contain("[Key]"));
            Assert.That(sut.ToString(), Does.Contain("[Required]"));
            Assert.That(sut.ToString(), Does.Contain("[JsonProperty(PropertyName = \"CategoryID\")]"));
        });
    }

    [Test]
    public void CamelCasePropertyDeclaration_test()
    {

        // Arrange 
        var setting = new PocoSetting
        {
            NameCase = CaseEnum.Camel
        };
        _attributeManager.Init(setting);
        var property = new PropertyTemplate
        {
            PropName = "CategoryID",
            PropType = "int",
            IsKey = true
        };

        // Act 
        var sut = new PropertyGenerator(property, setting);

        // Assert 
        Assert.That(sut.Declaration, Does.Contain("public int categoryID {get;set;}"));
    }
    [Test]
    public void PascalCasePropertyDeclarationTest()
    {
        // Arrange 
        var setting = new PocoSetting
        {
            NameCase = CaseEnum.Pas
        };
        _attributeManager.Init(setting);

        var property = new PropertyTemplate
        {
            PropName = "Category_ID",
            PropType = "int",
            IsKey = true
        };
        // Act 
        var sut = new PropertyGenerator(property, setting);

        // Assert 
        Assert.That(sut.Declaration, Does.Contain("public int CategoryID {get;set;}"));
    }
    [Test]
    public void NoneCasePropertyDeclaration_test()
    {
        // Arrange 
        var setting = new PocoSetting
        {
            NameCase = CaseEnum.None
        };
        _attributeManager.Init(setting);
        var property = new PropertyTemplate
        {
            PropName = "Category_ID",
            PropType = "int",
            IsKey = true
        };
        // Act 
        var sut = new PropertyGenerator(property, setting);

        // Assert 
        Assert.That(sut.Declaration, Does.Contain("public int Category_ID {get;set;}"));
    }
    [Test]
    public void JsonAttributePropertyDeclaration_test()
    {
        // Arrange 
        var setting = new PocoSetting
        {
            Attributes = ["json"],
        };
        _attributeManager.Init(setting);
        PropertyTemplate property = new PropertyTemplate
        {
            PropName = "CategoryID",
            PropType = "int",
            IsKey = true
        };
        // Act 
        var sut = new PropertyGenerator(property, setting);

        // Assert 
        var expected = "[JsonProperty(PropertyName = \"CategoryID\")]";
        Assert.That(sut.ToString(), Does.Contain(expected));
    }

    [Test]
    public void JsonAttributeWithCamelCasePropertyDeclaration_test()
    {
        // Arrange 
        var setting = new PocoSetting
        {
            Attributes = ["json"],
            NameCase = CaseEnum.Camel
        };
        _attributeManager.Init(setting);

        PropertyTemplate property = new PropertyTemplate
        {
            PropName = "CategoryID",
            PropType = "int",
            IsKey = true
        };
        // Act 
        var sut = new PropertyGenerator(property, setting);
        // Assert 
        var expected =
            $"[JsonProperty(PropertyName = \"CategoryID\")] {Environment.NewLine}public int categoryID {{get;set;}} ";


        sut.ToString().TrimAllSpace().Should().Be(expected.TrimAllSpace());
    }
    [Test]
    public void KeyAttributePropertyDeclaration_test()
    {
        // Arrange 
        var setting = new PocoSetting
        {
            Attributes = ["key"],
        };
        _attributeManager.Init(setting);
        PropertyTemplate property = new PropertyTemplate
        {
            PropName = "CategoryID",
            PropType = "int",
            IsKey = true
        };
        // Act 
        var sut = new PropertyGenerator(property, new PocoSetting
        {
            Attributes = ["key"],
        });
        // Assert 
        var expected = @"
[Key]
public int CategoryID {get;set;} ";

        sut.ToString().TrimAllSpace().Should().Be(expected.TrimAllSpace());
    }

    [Test]
    public void RequiredAttributePropertyDeclaration_test()
    {
        // Arrange 
        var setting = new PocoSetting
        {
            Attributes = ["req"],
        };
        _attributeManager.Init(setting);

        PropertyTemplate property = new PropertyTemplate
        {
            PropName = "CategoryID",
            PropType = "int",
            IsKey = true
        };
        // Act 
        var sut = new PropertyGenerator(property, setting);
        // Assert 

        var expected = @"
[Required]
public int CategoryID {get;set;} ";

        sut.ToString().TrimAllSpace().Should().Be(expected.TrimAllSpace());
    }

    [Test]
    public void IsNullablePropertyDeclaration_test()
    {
        // Arrange 
        var setting = new PocoSetting
        {
            AddNullableDataType = true
        };
        _attributeManager.Init(setting);
        PropertyTemplate property = new PropertyTemplate
        {
            IsKey = false,
            IsNullable = true,
            PropName = "dummy1",
            PropType = "int"
        };
        // Act 
        var sut = new PropertyGenerator(property, setting);
        // Assert 

        Assert.That(sut.Declaration, Does.Contain("public int? dummy1 {get;set;}"));
    }


    [Test]
    //products
    public void EagerVirtualPropertyDeclaration_test()
    {
        // Arrange 
        var setting = new PocoSetting
        {
            AddEager = true,
            AddNavigation = true
        };
        _attributeManager.Init(setting);
        PropertyTemplate property = new PropertyTemplate
        {
            PropName = "Products",
            PropType = "List<Product>",
            PropComment = "// not null",
            IsNavigate = true
        };
        // Act 
        var sut = new PropertyGenerator(property, setting);
        // Assert 
        Assert.That(sut.Declaration, Does.Contain("public List<Product> Products {get;set;}"));
    }

    [Test]
    public void LazyirtualPropertyDeclaration_test()
    {
        // Arrange 
        var setting = new PocoSetting
        {
            AddEager = false,
            AddNavigation = true
        };
        _attributeManager.Init(setting);
        var property = new PropertyTemplate
        {
            PropName = "Products",
            PropType = "List<Product>",
            PropComment = "// not null",
            IsNavigate = true
        };
        // Act 
        var sut = new PropertyGenerator(property, setting);
        // Assert 
        Assert.That(sut.Declaration, Does.Contain("public virtual List<Product> Products {get;set;}"));
    }
    [Test]
    public void Property_declaration_test()
    {
        // Arrange 
        var setting = new PocoSetting
        {
            AddEager = false,
            AddNavigation = true,
            Attributes = ["key", "json"],
        };
        _attributeManager.Init(setting);
        var property = new PropertyTemplate
        {
            PropName = "ProductId",
            PropType = "int",
            //PropComment = "// not null",
            IsKey = true
        };
        // Act 
        string sut = new PropertyGenerator(property, setting);
        var expected = @"
[Key]
[JsonProperty(PropertyName = ""ProductId"")]
        public virtual int ProductId {get;set;}
";
        // Assert 
        Assert.That(sut, Does.Match(expected.GetRegexPattern()));
    }

    [Test]
    [TestCase("dm", "[DataMember]")]
    [TestCase("proto", "[ProtoMember(1)]")]
    [TestCase("key", "[Key]")]
    [TestCase("req", "[Required]")]
    //[TestCase("tab", "")]
    //[TestCase("notExist", "")]
    [TestCase("[Non_named]", "[Non_named]")]
    [TestCase("display", "[Display(Name = \"Product Id\")]")]
    [TestCase("db", "[Key]")]
    public void Property_attributes_test(string att, string expected)
    {
        // Arrange 
        var setting = new PocoSetting
        {
            Attributes = [att]
        };
        _attributeManager.Init(setting);

        var property = new PropertyTemplate
        {
            PropName = "ProductId",
            PropType = "string",
            Serial = 1,
            IsKey = true,

        };
        // Act 
        string sut = new PropertyGenerator(property, setting);
        // Assert 
        Assert.That(sut, Does.Contain(expected));


    }
    [Test]
    public void Property_has_type_in_other_namespac_test()
    {
        // Arrange 
        var setting = new PocoSetting
        {
            AddEager = false,
        };
        var property = new PropertyTemplate
        {
            PropName = "Table",
            PropType = "SP.SimpleDataTable",
            ClassNameSpace = "SP1",
        };
        var expected = "public SP.SimpleDataTable Table {get;set;}";
        // Act 
        string sut = new PropertyGenerator(property, setting);
        // Assert 
        Assert.That(sut, Does.Contain(expected));

    }
    [Test]
    public void Property_has_type_in_the_same_namespac_test()
    {
        // Arrange 
        var setting = new PocoSetting
        {
            AddEager = false,
        };
        var property = new PropertyTemplate
        {
            PropName = "Table",
            PropType = "SP.SimpleDataTable",
            ClassNameSpace = "SP",
        };
        var expected = "public SimpleDataTable Table {get;set;}";
        // Act 
        string sut = new PropertyGenerator(property, setting);
        // Assert 
        Assert.That(sut, Does.Contain(expected));
    }
    [Test]
    public void Property_has_type_without_prefix_namespac_test()
    {
        // Arrange 
        var setting = new PocoSetting
        {
            AddEager = false,
        };
        var property = new PropertyTemplate
        {
            PropName = "Table",
            PropType = "SimpleDataTable",
            ClassNameSpace = "SP",
        };
        var expected = "public SimpleDataTable Table {get;set;}";
        // Act 
        string sut = new PropertyGenerator(property, setting);
        // Assert 
        Assert.That(sut, Does.Contain(expected));
    }
    [Test]
    public void Property_has_collection_type_in_the_same_namespac_test()
    {
        // Arrange
        var setting = new PocoSetting();
        var property = new PropertyTemplate
        {
            PropName = "Table",
            PropType = "List<SP.SimpleDataTable>",
            ClassNameSpace = "SP",
        };
        var expected = "public List<SimpleDataTable> Table {get;set;}";
        // Act 
        string sut = new PropertyGenerator(property, setting);
        // Assert 
        Assert.That(sut, Does.Contain(expected));
    }
    [Test]
    public void Property_has_collection_type_in_different_namespac_test()
    {
        // Arrange 
        var setting = new PocoSetting();
        var property = new PropertyTemplate
        {
            PropName = "Table",
            PropType = "List<SP.SimpleDataTable>",
            ClassNameSpace = "SP1",
        };
        var expected = "public List<SP.SimpleDataTable> Table {get;set;}";
        // Act 
        string sut = new PropertyGenerator(property, setting);
        // Assert 
        Assert.That(sut, Does.Contain(expected));
    }
    [Test]
    public void Property_has__type_collection_without_prefix_namespac_test()
    {
        // Arrange 
        var setting = new PocoSetting();
        var property = new PropertyTemplate
        {
            PropName = "Table",
            PropType = "List<SimpleDataTable>",
            ClassNameSpace = "SP1",
        };
        var expected = "public List<SimpleDataTable> Table {get;set;}";
        // Act 
        string sut = new PropertyGenerator(property, setting);
        // Assert 
        Assert.That(sut, Does.Contain(expected));
    }

    //feature #43
    [Test]
    [TestCase("Person", true, "public Person? ID {get;set;}")]
    [TestCase("Person", false, "public Person ID {get;set;}")]
    [TestCase("int", true, "public int? ID {get;set;}")]
    [TestCase("int", false, "public int ID {get;set;}")]
    [TestCase("List<Person>", true, "public List<Person>? ID {get;set;}")]
    [TestCase("List<Person>", false, "public List<Person> ID {get;set;}")]
    public void NullableReferenceType(string propType, bool isNullable, string expected)
    {
        // Arrange 
        var setting = new PocoSetting
        {
            EnableNullableReferenceTypes = true,
        };
        _attributeManager.Init(setting);

        var property = new PropertyTemplate
        {
            PropName = "ID",
            PropType = propType,
            IsNullable = isNullable
        };
        // Act 
        var sut = new PropertyGenerator(property, setting);

        // Assert              
        sut.Declaration.Should().Contain(expected);
    }

    [Test]
    public void Property_init_only_test()
    {
        // Arrange 
        var setting = new PocoSetting
        {
            InitOnly = true,
        };

        var property = new PropertyTemplate
        {
            PropName = "ID",
            PropType = "string",

        };
        // Act 
        var sut = new PropertyGenerator(property, setting);
        // Assert 
        sut.Declaration.Trim().Should().Be("public string ID {get;init;}");
    }
}