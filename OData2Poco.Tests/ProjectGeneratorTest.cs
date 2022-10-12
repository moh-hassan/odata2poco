// Copyright (c) Mohamed Hassan & Contributors. All rights reserved. See License.md in the project root for license information.

using NUnit.Framework;
using OData2Poco.Extensions;
using OData2Poco.Fake;

namespace OData2Poco.Tests;

internal class ProjectGeneratorTest
{

    [Test]
    public void Generate_default_project_Test()
    {
        //Arrange
        var attributes = new List<string> { "" };
        //Act
        var sut = new ProjectGenerator(attributes);
        var text = sut.GetProjectCode().Trim();
        var expected = File.ReadAllText(ProjectTestData.GenerateDefaultProjectTest).Trim();
        //Assert
        Assert.That(text.ToLines(), Is.EquivalentTo(expected.ToLines()));

    }

    [Test]
    public void Generate_project_for_json_Test()
    {
        //Arrange
        var attributes = new List<string> { "json" };
        //Act
        var sut = new ProjectGenerator(attributes);
        var text = sut.GetProjectCode().Trim();
        //Assert
        var expected = File.ReadAllText(ProjectTestData.GenerateProjectForJsonTest).Trim();
        Assert.That(text.ToLines(), Is.EquivalentTo(expected.ToLines()));
    }

    [Test]
    public void Generate_project_for_attributes_Test()
    {
        //Arrange
        var attributes = new List<string> { "key" };
        var sut = new ProjectGenerator(attributes);
        //Act
        var text = sut.GetProjectCode().Trim();
        //Assert
        var expected = File.ReadAllText(ProjectTestData.GenerateProjectForAttributesTest).Trim();
        Assert.That(text.ToLines(), Is.EquivalentTo(expected.ToLines()));
    }
}