using System;
using NUnit.Framework;
using OData2Poco.TextTransform;

namespace OData2Poco.Tests.TextTransform
{
   
    public class TemplateClassTest
    {
        [Test]
        public void class_default_Test()
        {
            FluentCsTextTemplate ft = new FluentCsTextTemplate();
            string result = ft.StartClass("Circle");
            var expected = "\tpublic partial class Circle\r\n\t{\r\n";
            

            Assert.That(result, Is.EqualTo(expected));
        }
        [Test]
        public void class_inherit_Test()
        {
            FluentCsTextTemplate ft = new FluentCsTextTemplate();
            string result = ft.StartClass("Circle", inherit: "Shape");
            var expected = "\tpublic partial class Circle : Shape\r\n\t{\r\n";
         

            Assert.That(result, Is.EqualTo(expected));
        }
        [Test]
        public void class_abstract_Test()
        {
            FluentCsTextTemplate ft = new FluentCsTextTemplate();
            string result = ft.StartClass("Circle", abstractClass: true);
            var expected = "\tpublic abstract partial class Circle\r\n\t{\r\n";
            Assert.That(result, Is.EqualTo(expected));
        }
        [Test]
        public void class_inherit_abstarct_Test()
        {
            FluentCsTextTemplate ft = new FluentCsTextTemplate();
            string result = ft.StartClass("Circle", inherit: "Shape", abstractClass: true);
            var expected = "\tpublic abstract partial class Circle : Shape\r\n\t{\r\n";
            Assert.That(result, Is.EqualTo(expected));
        }
        [Test]
        public void template_class_declare_Test()
        {
            //Arrange
            var ct = new ClassTemplate
            {
                Name = "Circle",
                IsAbstrct = true,
                BaseType = "Shape"
            };
            //Act
            FluentCsTextTemplate ft = new FluentCsTextTemplate();
            string result = ft.StartClass(ct);
            //Assert
            var expected = "\tpublic abstract partial class Circle : Shape\r\n\t{\r\n";
            Assert.That(result, Is.EqualTo(expected));
        }
    }
}
