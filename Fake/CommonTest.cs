// Copyright (c) Mohamed Hassan & Contributors. All rights reserved. See License.md in the project root for license information.

using System.Text.RegularExpressions;

namespace OData2Poco.Fake;

internal static class CommonTest
{
    #region Name Mapping
    public static void AssertRenameMap(string code)
    {
        var expected = new List<string>
            {
                //class should be modified
                "public partial class a0_City",
                "public partial class a0_Location",
                //property should be modified
                "public a0_City City {get;set;}",
                //base class should be modified
                "public partial class EventLocation : a0_Location"
            };
        //Assert
        code.Should().ContainAll(expected);

    }

    public static void AssertRenameMap2(string code)
    {
        var expected = "public string f02_Name {get;set;}";
        //Assert
        code.Should().Contain(expected);
    }

    public static void AssertRenameMap3(string code)
    {
        //Assert
        var matches = Regex.Matches(code, "Short_Name");
        matches.Count.Should().Be(4);
        matches = Regex.Matches(code, "f02_Name");
        matches.Count.Should().Be(1);
    }
    #endregion
}