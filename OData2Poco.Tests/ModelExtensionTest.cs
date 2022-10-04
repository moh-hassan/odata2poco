using FluentAssertions;
using FluentAssertions.Execution;
using NUnit.Framework;
using OData2Poco.graph;
using OData2Poco.Test.TypeScript;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace OData2Poco.Tests
{
    public class ModelExtensionTest: BaseTest
    {
        [Test]
        [TestCase("string", "string")]
        [TestCase("List<string>", "string")]
        [TestCase("List<Microsoft.OData.SampleService.Models.TripPin.Location>", "Microsoft.OData.SampleService.Models.TripPin.Location")]
        [TestCase("Microsoft.OData.SampleService.Models.TripPin.PersonGender",
            "Microsoft.OData.SampleService.Models.TripPin.PersonGender")]
        [TestCase("long", "long")]
        public void Property_generic_basetype_test(string type, string expected)
        {
            //Arrange
            //Act
            var sut = type.GetGenericBaseType();
            //Assert
            sut.Should().Be(expected);
        } 

        [Test]
        [TestCase("AirportLocation", "Location")]
        [TestCase("Event", "PlanItem")]
        [TestCase("EventLocation", "Location")]
        [TestCase("PublicTransportation", "PlanItem")]
        [TestCase("Flight", "PublicTransportation")]
        public void Class_base_type_test(string name, string expected)
        {
            //Arrange
            var ct = GetClassTemplateSample(name);            
            //Act
            var sut = ct.BaseType.ToClassTemplate(ClassList);
            //Assert
            sut.Name.Should().BeEquivalentTo(expected);

        }
    }
}
