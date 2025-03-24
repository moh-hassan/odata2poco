// Copyright (c) Mohamed Hassan & Contributors. All rights reserved. See License.md in the project root for license information.

namespace OData2Poco.Tests.TypeScript;

using System.Collections;
using OData2Poco.TypeScript;
using static OData2Poco.Fake.TestCaseSources;
public class NamingConventionTest : BaseTest
{
    [Test]
    [TestCaseSource(typeof(TestCaseSources), nameof(TestCases3))]
    public void Naming_convention_for_className_and_baseType_test(
        bool useFullName,
        string expectedClassName,
        string expectedBaseType)
    {
        //Arrange
        var setting = new PocoSetting()
        {
            UseFullName = useFullName
        };
        var ct = GetClassTemplateSample("Flight");
        //Act
        NamingConvention nc = new(ct, setting);

        //Assert
        nc.ClassName.Should().Be(expectedClassName);
        nc.BaseType.Should().Be(expectedBaseType);
    }

    [Test]
    [TestCaseSource(typeof(TestCaseSources), nameof(PropertyTestCases))]
    public void Naming_convention_for_proprty_type_test(bool useFullName,
        string expectedProperties)
    {
        //Arrange
        var setting = new PocoSetting()
        {
            UseFullName = useFullName
        };
        var ct = GetClassTemplateSample("Trip");
        //Act
        NamingConvention nc = new(ct, setting);

        //Assert
        ct.Properties.Select(a => nc.GetPropertyType(a.PropType))
            .Should().BeEquivalentTo(expectedProperties.ToLines());
    }
}
