// Copyright (c) Mohamed Hassan & Contributors. All rights reserved. See License.md in the project root for license information.

#define DEBUG
using FluentAssertions;
using NUnit.Framework;

namespace OData2Poco.Tests;

[TestFixture]
internal class HelperTest
{
    [Test]
    [TestCase("int", "?")]
    [TestCase("DateTime", "?")]
    public void GetNullableTest(string type, string nullable)
    {

        Helper.GetNullable(type).Should().Be(nullable);
    }
}