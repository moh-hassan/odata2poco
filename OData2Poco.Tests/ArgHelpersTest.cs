// Copyright (c) Mohamed Hassan & Contributors. All rights reserved. See License.md in the project root for license information.

namespace OData2Poco.Tests;

using System.Collections;

[TestFixture]
public class ArgHelpersTest
{
    private static IEnumerable RepeatingArgsTestData
    {
        //args,expected
        get
        {
            //repeating args
            string[] arg1 = ["-r", "http://localhost.com", "-H", "key1=abc", "-H", "key2=abc", "--auth", "token", "-a", "key", "-a", "json"];
            string[] arg2 = ["-r", "http://localhost.com", "-H", "key1=abc", "key2=abc", "--auth", "token", "-a", "key", "json"];
            yield return new TestCaseData(arg1, arg2).SetName("1-repeating args");

            //repeating args with = sign at the start
            string[] arg3 = ["--url=http://localhost.com", "-H", "key1=abc", "-H", "key2=abc", "--auth", "token", "-a", "key", "-a", "json"];
            string[] arg4 = ["--url=http://localhost.com", "-H", "key1=abc", "key2=abc", "--auth", "token", "-a", "key", "json"];
            yield return new TestCaseData(arg3, arg4).SetName("2-repeating args with equal sign at start");

            //repeating args with = sign at the end
            string[] arg5 = ["-H", "key1=abc", "-H", "key2=abc", "--auth", "token", "-a", "key", "-a", "json", "--url", "http://localhost.com"];
            string[] arg6 = ["--url", "http://localhost.com", "-H", "key1=abc", "key2=abc", "--auth", "token", "-a", "key", "json"];
            yield return new TestCaseData(arg5, arg6).SetName("3-repeating args with equal sign at end");

            //Non-repeating args
            string[] arg7 = ["-r", "http://localhost.com", "-H", "key1=abc", "--auth", "token"];
            yield return new TestCaseData(arg7, arg7).SetName("4-non repeating args");

            //empty args
            yield return new TestCaseData(Array.Empty<string>(), Array.Empty<string>())
                .SetName("5-empty args");

            //args with two args
            string[] arg8 = ["-r", "http://localhost.com"];
            yield return new TestCaseData(arg8, arg8).SetName("6-args with two args");

            //args with two args
            string[] arg9 = ["-r", "http://localhost.com", "-H", "key1=123;key2=abc"];
            yield return new TestCaseData(arg9, arg9).SetName("7-args have multi-value  arg");
        }
    }

    [Test]
    [TestCaseSource(nameof(RepeatingArgsTestData))]
    public void Merge_repeating_args_test(string[] args, string[] expected)
    {
        //Arrange
        //Act
        var result = args.MergeRepeatingArgs().ToArray();
        //Assert
        result.Should().BeEquivalentTo(expected);
    }
}
