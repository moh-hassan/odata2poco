// Copyright (c) Mohamed Hassan & Contributors. All rights reserved. See License.md in the project root for license information.

namespace OData2Poco.Tests.TextTransform;

using OData2Poco.TextTransform;

public class TemplateClassTest
{
    [Test]
    public void Class_declare_default_Test()
    {
        var ft = new FluentCsTextTemplate();
        string result = ft.StartClass("Circle");
        var expected = @"
    public partial class Circle
    {";
        Assert.That(result.TrimAllSpace(), Does.Contain(expected.TrimAllSpace()));
    }

    [Test]
    public void Class_inherit_Test()
    {
        var ft = new FluentCsTextTemplate();
        string result = ft.StartClass("Circle", "Shape");
        var expected = @"
    public partial class Circle : Shape
    {
";
        Assert.That(result.TrimAllSpace(), Does.Contain(expected.TrimAllSpace()));
    }

    [Test]
    public void Class_abstract_Test()
    {
        var ft = new FluentCsTextTemplate();
        string result = ft.StartClass("Circle", abstractClass: true);
        var expected = @"
    public abstract partial class Circle
    {
";
        Assert.That(result.TrimAllSpace(), Does.Contain(expected.TrimAllSpace()));
    }

    [Test]
    public void Class_inherit_abstarct_Test()
    {
        var ft = new FluentCsTextTemplate();
        string result = ft.StartClass("Circle", "Shape", abstractClass: true);
        var expected = @"
public abstract partial class Circle : Shape
    {
";
        Assert.That(result.TrimAllSpace(), Does.Contain(expected.TrimAllSpace()));
    }
}
