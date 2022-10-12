// Copyright (c) Mohamed Hassan & Contributors. All rights reserved. See License.md in the project root for license information.

using NUnit.Framework;

namespace OData2Poco.Tests;

[TestFixture]
internal class AssemplyManagerTest
{
    [Test]
    public void AddAsemplyTest()
    {
        var pocosetting = new PocoSetting();
        AssemplyManager am = new AssemplyManager(pocosetting, new List<ClassTemplate>());
        am.AddAssemply("xyz");
        Assert.IsTrue(am.AssemplyReference.Exists(m => m.Contains("xyz")));
    }
    [Test]
    public void AddAsemplyArrayTest()
    {
        var pocosetting = new PocoSetting();
        AssemplyManager am = new AssemplyManager(pocosetting, new List<ClassTemplate>());
        am.AddAssemply("xyz", "abc");
        Assert.IsTrue(am.AssemplyReference.Exists(m => m.Contains("xyz")));
        Assert.IsTrue(am.AssemplyReference.Exists(m => m.Contains("abc")));
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
            Attributes = new List<string> { key },
        };

        AssemplyManager am = new AssemplyManager(pocosetting, new List<ClassTemplate>());
        Assert.IsTrue(am.AssemplyReference.Exists(m => m.Contains(value)));
    }
    [Test]
    public void AddAsemplyMultiAttributes()
    {
        var pocosetting = new PocoSetting
        {
            Attributes = new List<string> { "tab", "req" },

        };

        AssemplyManager am = new AssemplyManager(pocosetting, new List<ClassTemplate>());
        Assert.IsTrue(am.AssemplyReference.Exists(m => m.Contains("System.ComponentModel.DataAnnotations.Schema")));
    }
    [Test]
    public void AddExternalAsemply()
    {
        var pocosetting = new PocoSetting
        {
            Attributes = new List<string> { "tab", "req" },
        };

        AssemplyManager am = new AssemplyManager(pocosetting, new List<ClassTemplate>());
        am.AddAssemply("xyz");
        Assert.IsTrue(am.AssemplyReference.Exists(m => m.Contains("System.ComponentModel.DataAnnotations.Schema")));
        Assert.IsTrue(am.AssemplyReference.Exists(m => m.Contains("xyz")));
    }
}