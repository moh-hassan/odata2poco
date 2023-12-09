// Copyright (c) Mohamed Hassan & Contributors. All rights reserved. See License.md in the project root for license information.

namespace OData2Poco.Tests.ModelFilter;

[Category("filter")]
internal class ModelFilterTest : BaseTest
{
    [Test]
    public void Filter_by_name_Test()
    {
        // Arrange
        var ct = new List<ClassTemplate>
        {
            new(1) {Name = "entity1"},
            new(2) {Name = "entity2"},
            new(3) {Name = "entity3"},
        };
        var filter = new List<string> { "entity1" };
        //Act
        var sut = ct.FilterList(filter).Select(x => x.Name);
        //Assert
        var expected = new[] { "entity1" };
        Assert.That(sut, Is.EquivalentTo(expected));
    }

    [Test]
    public void Filter_by_star_test()
    {
        //Arrange
        var ct = new List<ClassTemplate>
        {
            new(1) {Name = "entity1"},
            new(2) {Name = "entity2"},
            new(3) {Name = "entity_3"},
        };
        //Act
        var filter = new List<string> { "entity*" };
        var result = ct.FilterList(filter).Select(x => x.Name);
        //Assert
        var expected = new[] { "entity1", "entity2", "entity_3" };
        Assert.That(result, Is.EquivalentTo(expected));
    }

    [Test]
    public void Filter_by_multi_values_Test()
    {
        //Arrange
        var ct = new List<ClassTemplate>
        {
            new(1) { Name = "entity1"},
            new(2) {Name = "entity2"},
            new(3){Name = "entityzz3"},
        };
        //Act
        var filter = new List<string> { "entity?" };
        var result = ct.FilterList(filter).Select(x => x.Name);
        //Assert
        var expected = new[] { "entity1", "entity2" };
        Assert.That(result, Is.EquivalentTo(expected));
    }

    [Test]
    public void Filter_by_name_space_Test()
    {
        //Arrange
        var ct = new List<ClassTemplate>
        {
            new (1) {Name = "entity1",NameSpace = "NameSpace1"},
            new (2) {Name = "entity2",NameSpace = "NameSpace1"},
            new (3) {Name = "entityzz3",NameSpace = "NameSpace2"},
        };
        //Act
        var filter = new List<string> { "NameSpace1.*" };
        var result = ct.FilterList(filter).Select(x => x.Name);
        //Assert
        var expected = new[] { "entity1", "entity2" };
        Assert.That(result, Is.EquivalentTo(expected));
    }

    [Test]
    public void Filter_by_namespace_and_one_char_Test()
    {
        //Arrange
        var ct = new List<ClassTemplate>
        {
            new (1) {Name = "entity1",NameSpace = "NameSpace1"},
            new (2) {Name = "entity2",NameSpace = "NameSpace1"},
            new (3) {Name = "entityzz3",NameSpace = "NameSpace2"},
        };
        //Act
        var filter = new List<string> { "NameSpace1.entity?" };
        var result = ct.FilterList(filter).Select(x => x.Name);
        //Assert
        var expected = new[] { "entity1", "entity2" };
        Assert.That(result, Is.EquivalentTo(expected));
    }
    //"Airline", "Airport", "AirportLocation", "Location", "City"
    [Test]
    [TestCase("Airport", "Airport,AirportLocation,Location,City")]
    [TestCase("Air*", "AirportLocation,Airline,Airport,Location,City")]
    [TestCase("Airline,Airport", "Airline,Airport,AirportLocation,Location,City")]
    [TestCase("Flight", "Flight,Airport,AirportLocation,Location,City,Airline,PublicTransportation,PlanItem")]
    [TestCase("Photo", "Photo")]
    [TestCase("*Trans*", "PublicTransportation,PlanItem")]
    [TestCase("abc", "")]
    [TestCase("", "City,Location,EventLocation,AirportLocation,Photo,Person,Airline,Airport,PlanItem,PublicTransportation,Flight,Event,Trip,PersonGender")]
    public void Filter_should_handle_dependency_test(string keyword,
        string expectedClasses)
    {
        //Arrange
        List<string> list = expectedClasses == ""
            ? [] : expectedClasses.Split(',').ToList();
        var filter = keyword.Split(',').ToList();
        //Act
        var sut = ClassList.FilterList(filter).Select(x => x.Name);

        //Assert
        sut.Should().BeEquivalentTo(list);

    }


    [Test]
    [TestCase("air*")]
    public void Filter_using_star_should_handle_dependency_test(string keyword)
    {
        //Arrange           
        var filter = keyword.Split(',').ToList();
        var expected = new[] { "AirportLocation", "Airline", "Airport", "Location", "City" };
        //Act
        var sut = ClassList.FilterList(filter);
        sut.Select(a => a.Name).Should().BeEquivalentTo(expected);
    }

    [Test]
    public void Issue_29()
    {
        // Arrange
        List<PropertyTemplate> props = [new PropertyTemplate { PropName = "ownerid", PropType = "ns.principal" }];
        var classList = new List<ClassTemplate>
        {
            new (1) {Name = "account",NameSpace="ns",BaseType="ns.crmbaseentity", Properties=props, },
            new (2) {NameSpace="ns",Name = "crmbaseentity"},
            new (3) {NameSpace="ns",Name = "principal"},
            new (4) {NameSpace="ns",Name = "AAA"},
            new (5) {NameSpace="ns",Name = "BBB"},
        };
        var filter = new List<string> { "account" };
        //Act
        var sut = classList.FilterList(filter);

        //Assert
        var expected = new[] { "account", "principal", "crmbaseentity" };
        sut.Select(x => x.Name).Should().BeEquivalentTo(expected);
    }
}