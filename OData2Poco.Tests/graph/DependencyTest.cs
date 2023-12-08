// Copyright (c) Mohamed Hassan & Contributors. All rights reserved. See License.md in the project root for license information.

using OData2Poco.graph;

namespace OData2Poco.Tests.graph;

[Category("dependency")]
internal class DependencyTest : BaseTest
{
    [Test]
    [TestCase("City", "")]
    [TestCase("Location", "City")]
    [TestCase("EventLocation", "Location,City")]
    [TestCase("AirportLocation", "Location,City")]
    [TestCase("Photo", "")]
    [TestCase("Person", "Location,City,PersonGender,Trip,Photo,PlanItem")]
    [TestCase("Airport", "AirportLocation,Location,City")]
    [TestCase("Flight", "Airport,AirportLocation,Location,City,Airline,PublicTransportation,PlanItem")]
    [TestCase("Event", "EventLocation,Location,City,PlanItem")]
    [TestCase("Trip", "Photo,PlanItem")]
    public void Class_dependency_test(string name, string expectedDep)
    {
        //Arrange
        var ct = GetClassTemplateSample(name);
        var expected = StringToArray(expectedDep);
        //Act            
        var sut = Dependency.Search(ClassList, ct);
        //Assert
        sut.Select(c => c.Name).Should()
            .BeEquivalentTo(expected);
    }
    [Test]
    public void ClassTemplate_equality_test()
    {
        var airLine = GetClassTemplateSample("airline");
        var airLine2 = GetClassTemplateSample("airline");
        var airPort = GetClassTemplateSample("airport");
        airLine.Should().BeOfType<ClassTemplate>();
        airLine2.Should().BeOfType<ClassTemplate>();
        airLine.Should().BeEquivalentTo(airLine2);
        airLine.Should().NotBeSameAs(airPort);
    }
}