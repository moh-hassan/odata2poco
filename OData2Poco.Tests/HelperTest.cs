// Copyright (c) Mohamed Hassan & Contributors. All rights reserved. See License.md in the project root for license information.

#define DEBUG
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

        Assert.AreEqual(Helper.GetNullable(type), nullable);
    }
}