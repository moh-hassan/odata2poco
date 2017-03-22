//#define DEBUG

using System;
using System.Diagnostics;
using NUnit.Framework;
using OData2Poco.Shared;

/*
The tests applied for property are:
-c NameCase: camel /pascal/none
-j AddJsonAttribute: True/false
-b AddNullableDataType: True
-e Eager: False/true

-k Key: False/true
-n Navigation: True
-q Required: False
 * note: no need to test these services in other location. Remove it
 * */

namespace OData2Poco.Tests
{

    [TestFixture]
    class PropertyGeneratorTest
    {
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
           // Debug.WriteLine(pg);
            Assert.IsTrue(pg.Declaration.Contains("public int CategoryID {get;set;}"));
        }
        [Test]
        public void AllAttributesPropertyDeclaration()
        {
            PropertyTemplate property = new PropertyTemplate
            {
                PropName = "CategoryID",
                PropType = "int",
                IsKey = true
            };
            var pg = new PropertyGenerator(property, new PocoSetting
            {
                AddKeyAttribute = true,
                AddRequiredAttribute = true,
                AddJsonAttribute = true
            });
            //Debug.WriteLine(pg);

            Assert.IsTrue(pg.ToString().Contains("[Key]") &&
                pg.ToString().Contains("[Required]") &&
                pg.ToString().Contains("[JsonProperty(PropertyName = \"CategoryID\")]"));
        }

        [Test]
        public void CamelCasePropertyDeclaration()
        {
            var property = new PropertyTemplate
            {
                PropName = "CategoryID",
                PropType = "int",
                IsKey = true
            };
            var pg = new PropertyGenerator(property, new PocoSetting
            {
                NameCase = CaseEnum.Camel
            });
           // Debug.WriteLine(pg);
            Assert.IsTrue(pg.Declaration.Contains("public int categoryID {get;set;}"));
        }
        [Test]
        public void PascalCasePropertyDeclaration()
        {
            var property = new PropertyTemplate
            {
                PropName = "Category_ID",
                PropType = "int",
                IsKey = true
            };
            var pg = new PropertyGenerator(property, new PocoSetting
            {
                NameCase = CaseEnum.Pas
            });
            //Debug.WriteLine(pg);
            Assert.IsTrue(pg.Declaration.Contains("public int CategoryID {get;set;}"));
        }
        [Test]
        public void NoneCasePropertyDeclaration()
        {
            var property = new PropertyTemplate
            {
                PropName = "Category_ID",
                PropType = "int",
                IsKey = true
            };
            var pg = new PropertyGenerator(property, new PocoSetting
            {
                NameCase = CaseEnum.None
            });
            //Debug.WriteLine(pg);
            Assert.IsTrue(pg.Declaration.Contains("public int Category_ID {get;set;}"));
        }
        [Test]
        public void JsonAttributePropertyDeclaration()
        {
            //public int CategoryID {get;set;} //PrimaryKey not null
            PropertyTemplate property = new PropertyTemplate
            {
                PropName = "CategoryID",
                PropType = "int",
                IsKey = true
            };
            var pg = new PropertyGenerator(property, new PocoSetting
            {
                AddJsonAttribute = true
            });
          //  Debug.WriteLine(pg);
            var expected = "[JsonProperty(PropertyName = \"CategoryID\")]";

            Assert.IsTrue(pg.ToString().Contains(expected));
        }

        [Test]
        public void JsonAttributeWithCamelCasePropertyDeclaration()
        {
            //public int CategoryID {get;set;} //PrimaryKey not null
            PropertyTemplate property = new PropertyTemplate
            {
                PropName = "CategoryID",
                PropType = "int",
                IsKey = true
            };
            var pg = new PropertyGenerator(property, new PocoSetting
            {
                AddJsonAttribute = true,
                NameCase = CaseEnum.Camel
            });
          //  Debug.WriteLine(pg);
            var expected = "[JsonProperty(PropertyName = \"CategoryID\")] " +Environment.NewLine+
                           "public int categoryID {get;set;} ";

         //   Debug.WriteLine("Expected: " + expected);
            //Assert.IsTrue(Helper.CompareStringIgnoringSpaceCr(pg.ToString(), expected));
            Assert.AreEqual(pg.ToString().TrimAllSpace(), expected.TrimAllSpace());
        }
        [Test]
        public void KeyAttributePropertyDeclaration()
        {
            PropertyTemplate property = new PropertyTemplate
            {
                PropName = "CategoryID",
                PropType = "int",
                IsKey = true
            };
            var pg = new PropertyGenerator(property, new PocoSetting
            {
                AddKeyAttribute = true
            });
          //  Debug.WriteLine(pg);
            var expected = @"
[Key]
public int CategoryID {get;set;} ";
            //Assert.IsTrue(CompareStringIgnoringSpaceCr(pg.ToString(), expected));
            Assert.AreEqual(pg.ToString().TrimAllSpace(), expected.TrimAllSpace());
        }

        [Test]
        public void RequiredAttributePropertyDeclaration()
        {
            //public int CategoryID {get;set;} //PrimaryKey not null
            PropertyTemplate property = new PropertyTemplate
            {
                PropName = "CategoryID",
                PropType = "int",
                IsKey = true
            };
            var pg = new PropertyGenerator(property, new PocoSetting
            {
                AddRequiredAttribute = true
            });
            //Debug.WriteLine(pg);
            var expected = @"
[Required]
public int CategoryID {get;set;} ";
            //Assert.IsTrue(Helper.CompareStringIgnoringSpaceCr(pg.ToString(), expected));
            Assert.AreEqual(pg.ToString().TrimAllSpace(), expected.TrimAllSpace());
        }
        [Test]
        //Description property is null
        public void IsNullablePropertyDeclaration()
        {
            PropertyTemplate property = new PropertyTemplate
            {
                IsKey = false,
                IsNullable = true,
                PropName = "dummy1",
                PropType = "int"
            };

            PropertyGenerator pg = new PropertyGenerator(property, new PocoSetting
            {
                AddNullableDataType = true
            });
            //Debug.WriteLine(pg);
            Assert.IsTrue(pg.Declaration.Contains("public int? dummy1 {get;set;}"));
        }


        [Test]
        //products
        public void EagerVirtualPropertyDeclaration()
        {

            PropertyTemplate property = new PropertyTemplate
            {
                PropName = "Products",
                PropType = "List<Product>",
                PropComment = "// not null",
                IsNavigate = true
            };

            PropertyGenerator pg = new PropertyGenerator(property, new PocoSetting
            {
                AddEager = true,
                AddNavigation = true
            });
         //   Debug.WriteLine(pg);
            Assert.IsTrue(pg.Declaration.Contains("public List<Product> Products {get;set;}"));
        }

        [Test]
        public void LazyirtualPropertyDeclaration()
        {
            var property = new PropertyTemplate
            {
                PropName = "Products",
                PropType = "List<Product>",
                PropComment = "// not null",
                IsNavigate = true
            };
            PropertyGenerator pg = new PropertyGenerator(property, new PocoSetting
            {
                AddEager = false,
                AddNavigation = true
            });
         //   Debug.WriteLine(pg);
            Assert.IsTrue(pg.Declaration.Contains("virtual public List<Product> Products {get;set;}"));
        }

       

    }
}
