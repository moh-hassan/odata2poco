using System.Collections.Generic;
using System.Threading.Tasks;
using NUnit.Framework;
using OData2Poco.Api;
using OData2Poco.TestUtility;

namespace OData2Poco.Tests
{
    public class MultiSchemaTest
    {
        [Test]
        public async Task web_api2_v3_mult_namespace_Test()
        {
            var conn = new OdataConnectionString
            {
                ServiceUrl = TestSample.MultiSchemaV3,
            };
            var setting = new PocoSetting
            {
                Lang = Language.CS,
                NameCase = CaseEnum.None,
            };
            var o2P = new O2P(setting);
            var code = await o2P.GenerateAsync(conn);
            Assert.That(code, Does.Contain("namespace ODataDemo"));
            Assert.That(code, Does.Contain("namespace ODataDemo2"));
            Assert.That(code, Does.Contain("namespace ODataDemo3"));
        }
        [Test]
        public async Task web_api2_v3_mult_namespace_class_and_enum_Test()
        {
            var conn = new OdataConnectionString
            {
                ServiceUrl = TestSample.MultiSchemaV3,
            };
            var setting = new PocoSetting
            {
                Lang = Language.CS,
                NameCase = CaseEnum.None,
            };
            var o2P = new O2P(setting);
            var code = await o2P.GenerateAsync(conn);
            var expected = @"
namespace ODataDemo
{
	public partial class Customer
	{
	    public int ID {get;set;} //PrimaryKey not null
	    public string CustomerName {get;set;} 
	    public string Address {get;set;} 
	}
	public partial class Location
	{
	    public string Address {get;set;} 
	    public string City {get;set;}
	}
}
namespace ODataDemo2
{
	public partial class Customer
	{
    public int ID {get;set;} //PrimaryKey not null
	    public string CustomerName {get;set;} 
	    public string Address {get;set;} 
	}
	public partial class Customer2
	{
	    public int ID {get;set;} //PrimaryKey not null
	    public string CustomerName {get;set;} 
	    public string Address {get;set;} 
	}
	public enum Feature
	 {
 		Feature1=0,
 		Feature2=1,
 		Feature3=2,
 		Feature4=3 
	}
}
";
            //Assert
            Assert.That(code, Does.Match(expected.GetRegexPattern()));

        }
        [Test]
        public async Task Web_api2_v4_classes_and_enum_test()
        {
            var conn = new OdataConnectionString
            {
                ServiceUrl = TestSample.SampleWebApiV4,

            };
            var setting = new PocoSetting
            {
                Lang = Language.CS,
                NameCase = CaseEnum.None,
            };
            var o2P = new O2P(setting);
            var code = await o2P.GenerateAsync(conn);
            var expected = @"
namespace BookStore
{
	public partial class Window
	{
	    public int Id {get;set;} //PrimaryKey not null
	    public string Title {get;set;} 
	    public Shape Shape {get;set;} 
	}
	public abstract partial class Shape
	{
	    public bool HasBorder {get;set;} // not null
	    public Color Color {get;set;} // not null
	}
	public partial class Circle : Shape
	{
	    public Point Center {get;set;} 
	    public int Radius {get;set;} // not null
	}
	public partial class Point
	{
	    public int X {get;set;} // not null
	    public int Y {get;set;} // not null
	}
	public partial class Rectangle : Shape
	{
	    public Point LeftTop {get;set;} 
	    public int Height {get;set;} // not null
	    public int Weight {get;set;} // not null
	}
	public partial class RoundRectangle : Rectangle
	{
	    public double Round {get;set;} // not null
	}
	public partial class Triangle : Shape
	{
	    public Point P1 {get;set;} 
	    public Point P2 {get;set;} 
	    public Point P3 {get;set;} 
	}
	public enum Color
	 {
 		Red=0,
 		Blue=1,
 		Green=2,
 		Yellow=3 
	}
}
";
            //Assert
            Assert.That(code, Does.Match(expected.GetRegexPattern()));

        }
        [Test]
        [TestCase("key")]
        [TestCase("req")]
        [TestCase("tab")]
        [TestCase("dm")]
        [TestCase("proto")]
        public async Task Web_api2_v4_attributes_test(string att)
        {
            var conn = new OdataConnectionString
            {
                ServiceUrl = TestSample.SampleWebApiV4,

            };
            var setting = new PocoSetting
            {
                Lang = Language.CS,
                NameCase = CaseEnum.None,
                Attributes = new List<string> { att }
            };
            var o2P = new O2P(setting);
            var code = await o2P.GenerateAsync(conn);
            switch (att)
            {
                case "key":
                    Assert.That(code, Does.Contain("[Key]"));
                    break;
                case "req":
                    Assert.That(code, Does.Contain("[Required]"));
                    break;
                case "tab":
                    Assert.That(code, Does.Contain("[Table(\"Windows\")]"));
                    break;
                case "dm":
                    Assert.That(code, Does.Contain("	[DataContract]"));
                    Assert.That(code, Does.Contain("  [DataMember]"));
                    break;
                case "proto":
                    Assert.That(code, Does.Contain("	[ProtoContract]"));
                    Assert.That(code, Does.Contain("   [ProtoMember(1)"));
                    break;
            }
        }
        [Test]
        public async Task Web_api2_v4_abstract_class_test()
        {
            var conn = new OdataConnectionString
            {
                ServiceUrl = TestSample.SampleWebApiV4,

            };
            var setting = new PocoSetting
            {
                Lang = Language.CS,
                NameCase = CaseEnum.None,

            };
            var o2P = new O2P(setting);
            var code = await o2P.GenerateAsync(conn);
            var expected = "public abstract partial class Shape";
            Assert.That(code, Does.Contain(expected));

        }
        [Test]
        public async Task Web_api2_v4_base_class_test()
        {
            var conn = new OdataConnectionString
            {
                ServiceUrl = TestSample.SampleWebApiV4,

            };
            var setting = new PocoSetting
            {
                Lang = Language.CS,
                NameCase = CaseEnum.None,

            };
            var o2P = new O2P(setting);
            var code = await o2P.GenerateAsync(conn);
            Assert.That(code, Does.Contain("public partial class Circle : Shape"));
            Assert.That(code, Does.Contain("public partial class Rectangle : Shape"));

        }
    }
}
