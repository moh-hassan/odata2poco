using System;
using NUnit.Framework;
using OData2Poco.Extensions;
using OData2Poco.TextTransform;

namespace OData2Poco.Tests.TextTransform
{
   
    public class TemplateClassTest
    {
        [Test]
        public void class_declare_default_Test()
        {
            FluentCsTextTemplate ft = new FluentCsTextTemplate();
            string result = ft.StartClass("Circle");
            var expected = @"
    public partial class Circle
    {";
            Assert.That(result.TrimAllSpace(),Does.Contain(expected.TrimAllSpace()));
        }
        [Test]
        public void class_inherit_Test()
        {
            FluentCsTextTemplate ft = new FluentCsTextTemplate();
            string result = ft.StartClass("Circle", inherit: "Shape");
            var expected = @"
    public partial class Circle : Shape
    {
";
         

            Assert.That(result.TrimAllSpace(), Does.Contain(expected.TrimAllSpace()));
        }
        [Test]
        public void class_abstract_Test()
        {
            FluentCsTextTemplate ft = new FluentCsTextTemplate();
            string result = ft.StartClass("Circle", abstractClass: true);
            var expected = @"
    public abstract partial class Circle
    {
";
            Assert.That(result.TrimAllSpace(), Does.Contain(expected.TrimAllSpace()));
        }
        [Test]
        public void class_inherit_abstarct_Test()
        {
            FluentCsTextTemplate ft = new FluentCsTextTemplate();
            string result = ft.StartClass("Circle", inherit: "Shape", abstractClass: true);
            var expected = @"
public abstract partial class Circle : Shape
    {
";
            Assert.That(result.TrimAllSpace(),Does.Contain(expected.TrimAllSpace()));
        }
       
    }
}
