// Copyright (c) Mohamed Hassan & Contributors. All rights reserved. See License.md in the project root for license information.

namespace OData2Poco.Tests;

[TestFixture]
internal class AssemplyManagerTest
{
    [Test]
    public void AddAsemplyTest()
    {
        var pocosetting = new PocoSetting();
        AssemplyManager am = new AssemplyManager(pocosetting, []);
        am.AddAssemply("xyz");
        Assert.That(am.AssemplyReference, Has.Member("xyz"));
    }
    [Test]
    public void AddAsemplyArrayTest()
    {
        var pocosetting = new PocoSetting();
        AssemplyManager am = new AssemplyManager(pocosetting, []);
        am.AddAssemply("xyz", "abc");
        Assert.That(am.AssemplyReference, Has.Member("xyz"));
        Assert.That(am.AssemplyReference, Has.Member("abc"));
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
            Attributes = [key],
        };

        AssemplyManager am = new AssemplyManager(pocosetting, []);
        Assert.That(am.AssemplyReference, Has.Member(value));
    }
    [Test]
    public void AddAsemplyMultiAttributes()
    {
        var pocosetting = new PocoSetting
        {
            Attributes = ["tab", "req"],
        };

        AssemplyManager am = new AssemplyManager(pocosetting, []);
        Assert.That(am.AssemplyReference, Has.Member("System.ComponentModel.DataAnnotations.Schema"));
    }

    [Test]
    public void AddExternalAsemply()
    {
        var pocosetting = new PocoSetting
        {
            Attributes = ["tab", "req"],
        };

        AssemplyManager am = new AssemplyManager(pocosetting, []);
        am.AddAssemply("xyz");
        Assert.That(am.AssemplyReference, Has.Member("System.ComponentModel.DataAnnotations.Schema"));
        Assert.That(am.AssemplyReference, Has.Member("xyz"));
    }
}