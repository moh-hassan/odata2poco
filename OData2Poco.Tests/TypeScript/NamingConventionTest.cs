// Copyright (c) Mohamed Hassan & Contributors. All rights reserved. See License.md in the project root for license information.

using System.Collections;
using OData2Poco.TypeScript;

namespace OData2Poco.Tests.TypeScript;

public class NamingConventionTest : BaseTest
{
    [Test]
    [TestCaseSource(nameof(TestCases))]
    public void Naming_convention_for_className_and_baseType_test(bool useFullName,
        string expectedClassName, string expectedBaseType)
    {
        //Arrange  
        var setting = new PocoSetting() { UseFullName = useFullName };
        var ct = GetClassTemplateSample("Flight");
        //Act
        NamingConvention nc = new(ct, setting);

        //Assert
        nc.ClassName.Should().Be(expectedClassName);
        nc.BaseType.Should().Be(expectedBaseType);
    }
    [Test]
    [TestCaseSource(nameof(PropertyTestCases))]
    public void Naming_convention_for_proprty_type_test(bool useFullName,
        string expectedProperties)
    {
        //Arrange  
        var setting = new PocoSetting() { UseFullName = useFullName };
        var ct = GetClassTemplateSample("Trip");
        //Act
        NamingConvention nc = new(ct, setting);

        //Assert
        ct.Properties.Select(a => nc.GetPropertyType(a.PropType))
            .Should().BeEquivalentTo(expectedProperties.ToLines());


    }

    #region testcaseSource
    //(bool useFullName, expectedClassName , expectedBasetype)
    public static IEnumerable TestCases
    {
        get
        {
            yield return new TestCaseData(false, "Flight", "PublicTransportation");
            yield return new TestCaseData(true, "MicrosoftODataSampleServiceModelsTripPinFlight",
                "MicrosoftODataSampleServiceModelsTripPinPublicTransportation");
        }
    }

    //(bool useFullName, string expectedProperties)
    public static IEnumerable PropertyTestCases
    {
        get
        {
            yield return new TestCaseData(false, @"
number
string
string
string
number
Date
Date
string[]
Photo[]
PlanItem[]"
                .Trim());
            yield return new TestCaseData(true, @"
number
string
string
string
number
Date
Date
string[]
MicrosoftODataSampleServiceModelsTripPinPhoto[]
MicrosoftODataSampleServiceModelsTripPinPlanItem[]"
                .Trim());
        }
    }
    #endregion
}