// Copyright (c) Mohamed Hassan & Contributors. All rights reserved. See License.md in the project root for license information.

using FluentAssertions;
using NUnit.Framework;
using OData2Poco.Fake;

namespace OData2Poco.CommandLine.Test;

[TestFixture]
public partial class ProgramTests
{
    #region MapFile

    private async Task<Tuple<int, string>> BuildTest(string mapFile)
    {
        //Arrange
        var url = TestSample.TripPin4;
        var a = $"-r {url} -v --name-map {mapFile}";

        //Act
        var tuple = await RunCommand(a);
        return tuple;
    }
    [Test]
    public async Task Check_NameMapping_for_classes_cli()
    {
        //Arrange
        var tuple = await BuildTest(TestSample.RenameMap);
        //Assert
        CommonTest.AssertRenameMap(tuple.Item2);
        tuple.Item1.Should().Be(0);
    }

    [Test]
    [Category("map")]
    public async Task Check_NameMapping_for_class_properties_cli()
    {
        //Arrange
        var tuple = await BuildTest(TestSample.RenameMap2);
        //Assert
        CommonTest.AssertRenameMap2(tuple.Item2);
        tuple.Item1.Should().Be(0);
    }
    [Test]
    [Category("map")]
    public async Task Check_NameMapping_for_class_properties_with_all_cli()
    {
        //Arrange
        var tuple = await BuildTest(TestSample.RenameMap3);
        //Assert
        CommonTest.AssertRenameMap3(tuple.Item2);
        tuple.Item1.Should().Be(0);
    }
    #endregion

}