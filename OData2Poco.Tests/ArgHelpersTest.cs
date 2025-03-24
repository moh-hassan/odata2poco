// Copyright (c) Mohamed Hassan & Contributors. All rights reserved. See License.md in the project root for license information.

namespace OData2Poco.Tests;

using static OData2Poco.Fake.TestCaseSources;

[TestFixture]
public class ArgHelpersTest
{
    [Test]
    [TestCaseSource(typeof(TestCaseSources), nameof(RepeatingArgsTestData))]
    public void Merge_repeating_args_test(string[] args, string[] expected)
    {
        //Arrange
        //Act
        var result = args.MergeRepeatingArgs().ToArray();
        //Assert
        result.Should().BeEquivalentTo(expected);
    }
}
