// Copyright (c) Mohamed Hassan & Contributors. All rights reserved. See License.md in the project root for license information.

namespace OData2Poco.Tests;
public class ArgHelpersTest
{
    [Test, TestCaseSource(nameof(RepeatingArgsTestData))]
    public void Merge_repeating_args_test(string[] args, string[] expected)
    {
        //Arrange
        //Act
        var result = args.MergeRepeatingArgs().ToArray();
        //Assert
        result.Should().BeEquivalentTo(expected);
    }

    private static IEnumerable<TestCaseData> RepeatingArgsTestData
    {
        //args,expected
        get
        {
            //repeating args
            yield return new TestCaseData(new[] { "-r", "http://localhost.com", "-H", "key1=abc", "-H", "key2=abc", "--auth", "token", "-a", "key", "-a", "json" },
                new[] { "-r", "http://localhost.com", "-H", "key1=abc", "key2=abc", "--auth", "token", "-a", "key", "json" })
                .SetName("1-repeating args");

            //repeating args with = sign at the start
            yield return new TestCaseData(new[] { "--url=http://localhost.com", "-H", "key1=abc", "-H", "key2=abc", "--auth", "token", "-a", "key", "-a", "json" },
                new[] { "--url=http://localhost.com", "-H", "key1=abc", "key2=abc", "--auth", "token", "-a", "key", "json" })
                .SetName("2-repeating args with equal sign at start");

            //repeating args with = sign at the end
            yield return new TestCaseData(new[] { "-H", "key1=abc", "-H", "key2=abc", "--auth", "token",
                        "-a", "key", "-a", "json" , "--url", "http://localhost.com", },
                    new[] { "--url", "http://localhost.com", "-H", "key1=abc", "key2=abc", "--auth", "token", "-a", "key", "json" })
                .SetName("3-repeating args with equal sign at end");

            //Non repeating args
            yield return new TestCaseData(new[] { "-r", "http://localhost.com", "-H", "key1=abc", "--auth", "token" },
                new[] { "-r", "http://localhost.com", "-H", "key1=abc", "--auth", "token" })
                .SetName("4-non repeating args");

            //empty args
            yield return new TestCaseData(Array.Empty<string>(), Array.Empty<string>())
                .SetName("5-empty args");

            //args with two args
            yield return new TestCaseData(new[] { "-r", "http://localhost.com" },
                    new[] { "-r", "http://localhost.com" })
                .SetName("6-args with two args");

            //args with two args
            yield return new TestCaseData(new[] { "-r", "http://localhost.com", "-H", "key1=123;key2=abc" },
                    new[] { "-r", "http://localhost.com", "-H", "key1=123;key2=abc" })
                .SetName("7-args have multi-value  arg");
        }
    }

}
