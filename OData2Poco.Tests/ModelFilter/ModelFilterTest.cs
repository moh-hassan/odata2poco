using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using OData2Poco.Extensions;

namespace OData2Poco.Tests.Temp
{
    class ModelFilterTest
    {
        [Test]
        public void Filter_by_name_Test()
        {
            // Arrange
            var ct = new List<ClassTemplate>
            {
                new ClassTemplate {Name = "entity1"},
                new ClassTemplate {Name = "entity2"},
                new ClassTemplate {Name = "entity3"},
            };
            var filter = new List<string> { "entity1" };
            //Act
            var sut = ModelFilter.FilterList(ct, filter).Select(x => x.Name);
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
                new ClassTemplate {Name = "entity1"},
                new ClassTemplate {Name = "entity2"},
                new ClassTemplate {Name = "entity_3"},
            };
            //Act
            var filter = new List<string> { "entity*" };
            var result = ModelFilter.FilterList(ct, filter).Select(x => x.Name);
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
                new ClassTemplate {Name = "entity1"},
                new ClassTemplate {Name = "entity2"},
                new ClassTemplate {Name = "entityzz3"},
            };
            //Act
            var filter = new List<string> { "entity?" };
            var result = ModelFilter.FilterList(ct, filter).Select(x => x.Name);
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
                new ClassTemplate {Name = "entity1",NameSpace = "NameSpace1"},
                new ClassTemplate {Name = "entity2",NameSpace = "NameSpace1"},
                new ClassTemplate {Name = "entityzz3",NameSpace = "NameSpace2"},
            };
            //Act
            var filter = new List<string> { "NameSpace1.*" };
            var result = ModelFilter.FilterList(ct, filter).Select(x => x.Name);
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
                new ClassTemplate {Name = "entity1",NameSpace = "NameSpace1"},
                new ClassTemplate {Name = "entity2",NameSpace = "NameSpace1"},
                new ClassTemplate {Name = "entityzz3",NameSpace = "NameSpace2"},
            };
            //Act
            var filter = new List<string> { "NameSpace1.entity?" };
            var result = ModelFilter.FilterList(ct, filter).Select(x => x.Name);
            //Assert
            var expected = new[] { "entity1", "entity2" };
            Assert.That(result, Is.EquivalentTo(expected));
        }
    }
}
