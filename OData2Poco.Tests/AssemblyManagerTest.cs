// Copyright (c) Mohamed Hassan & Contributors. All rights reserved. See License.md in the project root for license information.

namespace OData2Poco.Tests;

[TestFixture]
public class AssemblyManagerTest
{
    [Test]
    public void AddAssemblyTest()
    {
        var pocoSetting = new PocoSetting();
        var am = new AssemblyManager(pocoSetting, []);
        am.AddAssemply("xyz");
        Assert.That(am._assemplyReference, Has.Member("xyz"));
    }

    [Test]
    public void AddAsemplyArrayTest()
    {
        var pocosetting = new PocoSetting();
        var am = new AssemblyManager(pocosetting, []);
        am.AddAssemply("xyz", "abc");
        Assert.That(am._assemplyReference, Has.Member("xyz"));
        Assert.That(am._assemplyReference, Has.Member("abc"));
    }

    [Test]
    [TestCase("key", "System.ComponentModel.DataAnnotations")]
    [TestCase("req", "System.ComponentModel.DataAnnotations")]
    [TestCase("tab", "System.ComponentModel.DataAnnotations.Schema")]
    [TestCase("json", "Newtonsoft.Json")]
    public void AddAsemplyByKey(string key, string value)
    {
        var pocosetting = new PocoSetting
        {
            Attributes = [key]
        };

        var am = new AssemblyManager(pocosetting, []);
        Assert.That(am._assemplyReference, Has.Member(value));
    }

    [Test]
    public void AddAsemplyMultiAttributes()
    {
        var pocosetting = new PocoSetting
        {
            Attributes = ["tab", "req"]
        };

        var am = new AssemblyManager(pocosetting, []);
        Assert.That(am._assemplyReference, Has.Member("System.ComponentModel.DataAnnotations.Schema"));
    }

    [Test]
    public void AddExternalAsemply()
    {
        var pocosetting = new PocoSetting
        {
            Attributes = ["tab", "req"]
        };

        var am = new AssemblyManager(pocosetting, []);
        am.AddAssemply("xyz");
        Assert.That(am._assemplyReference, Has.Member("System.ComponentModel.DataAnnotations.Schema"));
        Assert.That(am._assemplyReference, Has.Member("xyz"));
    }
}
