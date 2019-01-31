//#define DEBUG

using System;
using System.Collections.Generic;
using NUnit.Framework;
using OData2Poco.CustAttributes;
using OData2Poco.Extensions;

namespace OData2Poco.Tests
{

    [TestFixture]
    class PropertyGeneratorTest
    {
        private readonly AttributeFactory _attributeManager = AttributeFactory.Default;
      
        [Test]
        public void DefaultPropertyDeclaration()
        {
            PropertyTemplate property = new PropertyTemplate
            {
                PropName = "CategoryID",
                PropType = "int",
                IsKey = true
            };
            var pg = new PropertyGenerator(property, new PocoSetting());
         
            Assert.IsTrue(pg.Declaration.Contains("public int CategoryID {get;set;}"));
        }
        [Test]
        public void AllAttributesPropertyDeclaration()
        {
            // Arrange 
            var setting = new PocoSetting
            {
                AddKeyAttribute = true,
                AddRequiredAttribute = true,
                AddJsonAttribute = true
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
            Assert.IsTrue(sut.ToString().Contains("[Key]") &&
                sut.ToString().Contains("[Required]") &&
                sut.ToString().Contains("[JsonProperty(PropertyName = \"CategoryID\")]"));
        }

        [Test]
        public void CamelCasePropertyDeclaration()
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
            Assert.IsTrue(sut.Declaration.Contains("public int categoryID {get;set;}"));
        }
        [Test]
        public void PascalCasePropertyDeclaration()
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
            Assert.IsTrue(sut.Declaration.Contains("public int CategoryID {get;set;}"));
        }
        [Test]
        public void NoneCasePropertyDeclaration()
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
            Assert.IsTrue(sut.Declaration.Contains("public int Category_ID {get;set;}"));
        }
        [Test]
        public void JsonAttributePropertyDeclaration()
        {
            // Arrange 
            var setting = new PocoSetting
            {
                AddJsonAttribute = true
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

            Assert.IsTrue(sut.ToString().Contains(expected));
        }

        [Test]
        public void JsonAttributeWithCamelCasePropertyDeclaration()
        {
            // Arrange 
            var setting = new PocoSetting
            {
                AddJsonAttribute = true,
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
            var expected = "[JsonProperty(PropertyName = \"CategoryID\")] " +Environment.NewLine+
                           "public int categoryID {get;set;} ";

       
            Assert.AreEqual(sut.ToString().TrimAllSpace(), expected.TrimAllSpace());
        }
        [Test]
        public void KeyAttributePropertyDeclaration()
        {
            // Arrange 
            var setting = new PocoSetting
            {
                AddKeyAttribute = true
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
                AddKeyAttribute = true
            });
            // Assert 
            var expected = @"
[Key]
public int CategoryID {get;set;} ";
        
            Assert.AreEqual(sut.ToString().TrimAllSpace(), expected.TrimAllSpace());
        }

        [Test]
        public void RequiredAttributePropertyDeclaration()
        {
            // Arrange 
            var setting = new PocoSetting
            {
                AddRequiredAttribute = true
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
           
            Assert.AreEqual(sut.ToString().TrimAllSpace(), expected.TrimAllSpace());
        }

        [Test]
        //Description property is null
        public void IsNullablePropertyDeclaration()
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
           
            Assert.IsTrue(sut.Declaration.Contains("public int? dummy1 {get;set;}"));
        }


        [Test]
        //products
        public void EagerVirtualPropertyDeclaration()
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
            Assert.IsTrue(sut.Declaration.Contains("public List<Product> Products {get;set;}"));
        }

        [Test]
        public void LazyirtualPropertyDeclaration()
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
            Assert.IsTrue(sut.Declaration.Contains("public virtual List<Product> Products {get;set;}"));
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
        public void test1(string att,string expected)
        {
            // Arrange 
            var setting = new PocoSetting
            {
                Attributes = new List<string> {att } 
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
            var sut = new PropertyGenerator(property, setting);
            // Assert 
             Assert.IsTrue(sut.ToString().Contains(expected));
          
           
        }

    }
}
