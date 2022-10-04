using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NUnit.Framework;
using OData2Poco.Extensions;

namespace OData2Poco.Tests
{
    class ProjectGeneratorTest
    {

        [Test]
        public void Generate_default_project_Test()
        {
            //Arrange
            var attributes = new List<string> { "" };
            //Act
            var sut = new ProjectGenerator(attributes);
            var text = sut.GetProjectCode().Trim();
            var expected = File.ReadAllText(ProjectTestData.Generate_default_project_Test).Trim();
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
            var expected = File.ReadAllText(ProjectTestData.Generate_project_for_json_Test).Trim();
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
            var expected = File.ReadAllText(ProjectTestData.Generate_project_for_attributes_Test).Trim();
            Assert.That(text.ToLines(), Is.EquivalentTo(expected.ToLines()));
        }
    }
}
