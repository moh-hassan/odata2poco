
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using NUnit.Framework;
using OData2Poco.CustAttributes;


namespace OData2Poco.Tests
{

    [TestFixture]
    class NamedAttributeTest
    {
      
        //common data
        List<string> list = new List<string> { "dm", "db", "display", "json", "key", "req", "tab", "proto" };

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
            AttributeFactory.Default.Init();
            //Act
            var sut = AttributeFactory.Default.GetAttributes(p, "key");
            var att = string.Join(" ", sut);
            //Assert
            Assert.IsTrue(att.Contains("[Key"));
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
            AttributeFactory.Default.Init();
            //Act
            var sut = AttributeFactory.Default.GetAttributes(p, "key");
            var att = string.Join(" ", sut);
            //Assert
            Assert.IsTrue(att.Length == 0);
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
            AttributeFactory.Default.Init();
            //Act
            var sut = AttributeFactory.Default.GetAttributes(p, "req");
            var att = string.Join(" ", sut);
            //Assert
            Assert.IsTrue(att.Contains("[Required]"));
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
            AttributeFactory.Default.Init();
            //Act
            var sut = AttributeFactory.Default.GetAttributes(p, "req");
            var att = string.Join(" ", sut);
            //Assert
            Assert.IsTrue(att.Length == 0);
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
            AttributeFactory.Default.Init();
            //Act
            var sut = AttributeFactory.Default.GetAttributes(p, "json");
            var att = string.Join(" ", sut);
            //Assert
            var expected= "[JsonProperty(PropertyName = \"FirstName\")]";
            Assert.IsTrue(att.Contains(expected));
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
            AttributeFactory.Default.Init();
            //Act
            var sut = AttributeFactory.Default.GetAttributes(p, "dm");
            var att = string.Join(" ", sut);
            //Assert
            var expected = "[DataMember]";
            Assert.IsTrue(att.Contains(expected));
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
            AttributeFactory.Default.Init();
            //Act
            var sut = AttributeFactory.Default.GetAttributes(p, "proto");
            var att = string.Join(" ", sut);
            //Assert
            var expected = "[ProtoMember(3)]";
            Assert.IsTrue(att.Contains(expected));
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
            AttributeFactory.Default.Init();
            //Act
            var sut = AttributeFactory.Default.GetAttributes(p, "display");
            var att = string.Join(" ", sut);
            //Assert
            var expected = "[Display(Name = \"First Name\")]";
           Assert.IsTrue(att.Contains(expected));
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
            AttributeFactory.Default.Init();
            //Act
            var sut = AttributeFactory.Default.GetAttributes(p, "db");
            var att = string.Join(" ", sut);
            //Assert
            var expected = "[Key] [Required]";
            Assert.IsTrue(att.Contains(expected));
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
            AttributeFactory.Default.Init();
            //Act
            var sut = AttributeFactory.Default.GetAttributes(p, "tab");
            var att = string.Join(" ", sut);
            //Assert
            Assert.IsTrue(att.Length == 0);
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
        public void PropertyTemplateAttributeTest(string name, string value)
        {
            // Arrange 
            PropertyTemplate p = new PropertyTemplate
            {
                PropName = "FirstName",
                PropType = "string",
                IsKey = true,
                Serial = 1,
            };
            AttributeFactory.Default.Init();
            //Act
            var sut = AttributeFactory.Default.GetAttributes(p, name);
            var att = string.Join(" ", sut);
            //Assert
            //Console.WriteLine("{0} = {1}",name, string.Join(" ", sut));
            Assert.AreEqual(value, att);
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
        public void ClasTemplateAttributeTest(string name, string value)
        {
            // Arrange 
            var p = new ClassTemplate()
            {
                Name = "ProductDetail",
                EntitySetName = "productDetail"
            };
            AttributeFactory.Default.Init();
            //Act
         
            var sut = AttributeFactory.Default.GetAttributes(p, name);
            var att = string.Join(" ", sut);
            //Assert
            Assert.AreEqual(value, att);
        }

        private bool ListCheck<T>(IEnumerable<T> l1, IEnumerable<T> l2)
        {
            // TODO: Null parm checks
            if (l1.Intersect(l2).Any())
            {
                Console.WriteLine("matched");
                return true;
            }
            else
            {
                Console.WriteLine("not matched");
                return false;
            }
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


            var atts = AttributeFactory.Default.GetAttributes(p, list);
            atts.ForEach(x => Console.WriteLine(x));
            var match = ListCheck(atts, new List<string>
            {
                "[ProtoMember(1)]",
                "[DataMember]" ,
                " [Key]",
                "[Required]",
                "[Display(Name = \"First Name\")]",
                "[JsonProperty(PropertyName = \"FirstName\")]",

            });
            Assert.IsTrue(match);
        }

        [Test]
        public void ClassTemplate_All_Attributes_Test()
        {
            // Arrange 
            var p = new ClassTemplate()
            {
                Name = "ProductDetail",
                EntitySetName = "productDetail"
            };

            //var pa = new PocoAttributesList();
            var atts = AttributeFactory.Default.GetAttributes(p, list );
            Console.WriteLine("----------");
            atts.ForEach(x => Console.WriteLine(x));
            var match = ListCheck(atts, new List<string>
            {
                "[DataContract]",
                "[Table(\"productDetail\")]",
                "[ProtoContract]",
            });
            Assert.IsTrue(match);
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
            //AttributeManager.Default.Init(list.ToList());
            var setting = new PocoSetting()
            {
                Attributes = new List<string>(list),
            };

            var atts = AttributeFactory.Default
                .Init(setting)
                .GetAllAttributes(p);
            atts.ForEach(x => Console.WriteLine(x));
            var match = ListCheck(atts, new List<string>
            {
                "[ProtoMember(1)]",
                "[DataMember]" ,
                " [Key]",
                "[Required]",
                "[Display(Name = \"First Name\")]",
                "[JsonProperty(PropertyName = \"FirstName\")]",

            });
            Assert.IsTrue(match);

        }

        [Test]
        public void ClassTemplate_All_Attributes_With_SettingTest()
        {
            // Arrange 
            var p = new ClassTemplate()
            {
                Name = "ProductDetail",
                EntitySetName = "productDetail"
            };

            var setting = new PocoSetting()
            {
                Attributes = new List<string>(list),
            };

            var atts = AttributeFactory.Default
                .Init(setting)
                .GetAllAttributes(p);
            atts.ForEach(x => Console.WriteLine(x));
            var match = ListCheck(atts, new List<string>
            {
                "[DataContract]",
                "[Table(\"productDetail\")]",
                "[ProtoContract]",

            });
            Assert.IsTrue(match);


        }

    }
}
