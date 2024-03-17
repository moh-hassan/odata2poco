// Copyright (c) Mohamed Hassan & Contributors. All rights reserved. See License.md in the project root for license information.

namespace OData2Poco.Tests;

using System;
using System.Collections.Generic;

[Category("ctor")]
[TestFixture]
public class ConstructorGeneratorTest
{
    [Test]
    [TestCase("CategoryID", "categoryID")]
    [TestCase("Customer", "customer")]
    [TestCase("OrderDetail", "orderDetail")]
    public void Variable_naming_should_be_camel_test(string name, string expected)
    {
        var property = new PropertyTemplate
        {
            PropName = name,
            PropType = "int",
            IsKey = true
        };
        var pg = new PropertyGenerator(property, new PocoSetting());
        Assert.Multiple(() =>
        {
            Assert.That(pg.VariableName, Is.EqualTo(expected));
            Assert.That(pg.VariableDeclaration, Does.Contain($"int {expected}"));
        });
    }

    [Test]
    public void Get_properties_for_constructor_test()
    {
        //Arrange
        List<PropertyTemplate> properties =
        [
            new PropertyTemplate
            {
                PropName = "CategoryID", PropType = "int", IsKey = true
            },

            new PropertyTemplate
            {
                PropName = "CategoryName", PropType = "string", IsNullable = true
            },

            new PropertyTemplate
            {
                PropName = "Description", PropType = "string", IsKey = false
            },
            new PropertyTemplate
            {
                PropName = "Navigate", PropType = "string", IsNavigate = true
            }
        ];

        //Act
        var sut = PropertyGenerator.GetPropertiesForConstructor(properties)
            .Select(p => p.PropName);
        //Assert
        Assert.That(sut.AsString(), Is.EqualTo("CategoryID, Description"));
    }

    [Test]
    public void Ctor_generation_full_test()
    {
        //Arrange
        var expected = """
                       public Customer () { }
                       
                       public Customer (int categoryID, string description)
                       {
                        CategoryID = categoryID;
                        Description = description;
                       }
                       """;
        List<PropertyTemplate> properties =
        [
            new PropertyTemplate
            {
                PropName = "CategoryID", PropType = "int", IsKey = true
            },

            new PropertyTemplate
            {
                PropName = "CategoryName", PropType = "string", IsNullable = true
            },

            new PropertyTemplate
            {
                PropName = "Description", PropType = "string", IsKey = false
            },
            new PropertyTemplate
            {
                PropName = "Navigate", PropType = "string", IsNavigate = true
            }
        ];
        ClassTemplate ct = new(1)
        {
            Name = "Customer",
            Properties = properties
        };
        PocoSetting setting = new()
        {
            WithConstructor = Ctor.Full
        };
        //Act
        var pg = PropertyGenerator.GenerateFullConstructor(ct, setting);
        //Assert
        Assert.That(pg.ToLines(), Is.EqualTo(expected.ToLines()));
    }

    [Test]
    public void Ctor_primary_generation()
    {
        //Arrange
        List<PropertyTemplate> properties =
        [
            new PropertyTemplate
            {
                PropName = "CategoryID", PropType = "int", IsKey = true
            },

            new PropertyTemplate
            {
                PropName = "CategoryName", PropType = "string", IsNullable = true
            },

            new PropertyTemplate
            {
                PropName = "Description", PropType = "string", IsKey = false
            }
        ];
        ClassTemplate ct = new(1)
        {
            Name = "Customer",
            Properties = properties
        };
        PocoSetting setting = new();
        //Act
        var sut = PropertyGenerator.GeneratePrimaryConstructor(ct, setting);
        //Assert
        Assert.That(sut.Trim(), Is.EqualTo("(int categoryID, string description)"));
    }
}
