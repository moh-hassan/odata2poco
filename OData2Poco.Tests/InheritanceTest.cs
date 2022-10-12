// Copyright (c) Mohamed Hassan & Contributors. All rights reserved. See License.md in the project root for license information.

using NUnit.Framework;
using OData2Poco.Api;

//these tests are moved from OData2Poco.CommandLine.Test, modified to use O2P class
namespace OData2Poco.Tests;

internal class InheritanceTest
{
    [Test]
    public void InheritanceEnabledByDefaultTest()
    {
        var o2P = new O2P();
        Assert.IsTrue(o2P.Setting.UseInheritance);
    }
    [Test]
    public void InheritanceDisabledWithInheritSettingTest()
    {

        var setting = new PocoSetting
        {
            Inherit = "MyBaseClass",
        };
        var o2P = new O2P(setting);
        Assert.IsFalse(o2P.Setting.UseInheritance);
    }
}