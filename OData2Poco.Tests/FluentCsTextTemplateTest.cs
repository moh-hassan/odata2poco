using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using OData2Poco.Tests.Helper;
using OData2Poco.TextTransform;

namespace OData2Poco.Tests
{
    [TestFixture]
    class FluentCsTextTemplateTest
    {
        [Test]
        public void GenerateDummyClassTest()
        {
            var code = @"using System;
using System.Text;

namespace Northwind.Data
{
	public class Northwind
	{
	    public int age  {get;set;} 
	    virtual public double Price  {get;set;} //Price of product
	    [key]
	    [Required]
	    public string StartDate  {get;set;} 
	}
}
";
            var template = new FluentCsTextTemplate();
            var name = "TestClass";
            var text = "";
            var result = template
                //using namespace
                .UsingNamespace("System")
                .UsingNamespace("System.Text")
                .NewLine()
                //namespace
                .StartNamespace("Northwind.Data")
                //class name
                .StartClass("Northwind")
                .WriteLineProperty("int", "age")
                .WriteLineProperty("double", "Price", isVirtual: true, comment: "Price of product")
                .WriteLineAttribute("key")
                .WriteLineAttribute("Required")
                .WriteLineProperty("string", "StartDate")
                .EndClass()
                .EndNamespace()
                .ToString();
            Console.WriteLine(result);
            var compRegex = new StringCompIgnoreWhiteSpaceRegex();
          var flag=  compRegex.Equals(code, result);
           // StringAssert.Contains(code.Trim(), result.Trim());
            Assert.IsTrue(flag);
        }
    }
}
