// Copyright (c) Mohamed Hassan & Contributors. All rights reserved. See License.md in the project root for license information.

namespace OData2Poco.Tests;

[Category("PocoClassGeneratorCsTest")]
public class PocoClassGeneratorCsTest
{
    private readonly string _nl = Environment.NewLine;

    [Test]
    public void Class_basetype_has_the_same_namespace_test()
    {
        //Arrange
        var setting = new PocoSetting();
        var ct = new ClassTemplate(1)
        {
            Name = "Folder",
            NameSpace = "SP",
            BaseType = "SP.FileSystemItem"
        };
        var gen = Moq.Moq4IPocoGenerator(ct);

        //Act
        var sut = PocoClassGeneratorCs.GenerateCsPocoClass(gen, setting);
        var code = sut.ClassToString(ct);
        //Assert
        var expected = "public partial class Folder : FileSystemItem";
        Assert.That(code, Does.Contain(expected));
    }

    [Test]
    public void Class_basetype_has_different_namespace_test()
    {
        //Arrange
        var setting = new PocoSetting();
        var ct = new ClassTemplate(1)
        {
            Name = "Folder",
            NameSpace = "SP1",
            BaseType = "SP.FileSystemItem"
        };
        var gen = Moq.Moq4IPocoGenerator(ct);
        var sut = PocoClassGeneratorCs.GenerateCsPocoClass(gen, setting);

        //Act
        var code = sut.ClassToString(ct);
        //Assert
        var expected = "public partial class Folder : SP.FileSystemItem";
        Assert.That(code, Does.Contain(expected));
    }

    [Test]
    public void Class_is_abstract_test()
    {
        //Arrange
        var setting = new PocoSetting();
        var ct = new ClassTemplate(1)
        {
            Name = "Folder",
            NameSpace = "SP",
            IsAbstrct = true
        };
        var gen = Moq.Moq4IPocoGenerator(ct);

        //Act
        var sut = PocoClassGeneratorCs.GenerateCsPocoClass(gen, setting);
        var code = sut.ClassToString(ct);
        //Assert
        var expected = "public abstract partial class Folder";
        Assert.That(code, Does.Contain(expected));
    }

    [Test]
    public void Class_is_entity_test()
    {
        //Arrange
        var setting = new PocoSetting();
        var ct = new ClassTemplate(1)
        {
            Name = "Folder",
            NameSpace = "SP",
            IsEntity = true
        };
        var gen = Moq.Moq4IPocoGenerator(ct);

        //Act
        var sut = PocoClassGeneratorCs.GenerateCsPocoClass(gen, setting);
        var code = sut.ClassToString(ct);
        //Assert
        var expected = "public partial class Folder";
        Assert.That(code, Does.Contain(expected));
    }

    [Test]
    public void Class_is_complex_test()
    {
        //Arrange
        var setting = new PocoSetting();
        var ct = new ClassTemplate(1)
        {
            Name = "Folder",
            NameSpace = "SP",
            IsComplex = true
        };
        var gen = Moq.Moq4IPocoGenerator(ct);

        //Act
        var sut = PocoClassGeneratorCs.GenerateCsPocoClass(gen, setting);
        var code = sut.ClassToString(ct);
        //Assert
        var expected = "public partial class Folder";
        Assert.That(code, Does.Contain(expected));
    }

    [Test]
    public void Type_is_enum_test()
    {
        //Arrange
        var setting = new PocoSetting();
        var ct = new ClassTemplate(1)
        {
            Name = "Feature",
            NameSpace = "SP",
            IsEnum = true,
            // EnumElements = new List<string> { "Feature0", "Feature1" }
            EnumElements = ["Feature0", "Feature1"]
        };
        var gen = Moq.Moq4IPocoGenerator(ct);

        //Act
        var sut = PocoClassGeneratorCs.GenerateCsPocoClass(gen, setting);
        var code = sut.ClassToString(ct);
        //Assert
        var expected = @"
public enum Feature
	 {
         Feature0,
         Feature1 
	}
";
        Assert.That(code.TrimAllSpace(), Does.Contain(expected.TrimAllSpace()));
    }

    [Test]
    public void Type_is_enum_with_flags_test()
    {
        //Arrange
        var setting = new PocoSetting();
        var ct = new ClassTemplate(1)
        {
            Name = "Feature",
            NameSpace = "SP",
            IsEnum = true,
            IsFlags = true,
            EnumElements = ["Feature0", "Feature1"]
        };
        var gen = Moq.Moq4IPocoGenerator(ct);

        //Act
        var sut = PocoClassGeneratorCs.GenerateCsPocoClass(gen, setting);
        var code = sut.ClassToString(ct);
        //Assert
        var expected = @"
[Flags] public enum Feature
	 {
         Feature0,
         Feature1 
	}
";
        Assert.That(code.TrimAllSpace(), Does.Contain(expected.TrimAllSpace()));
    }

    [Test]
    [TestCase("key")]
    [TestCase("req")]
    [TestCase("json")]
    public void Class_is_entity_with_non_class_attributes_test(string att)
    {
        //Arrange
        var setting = new PocoSetting
        {
            Attributes = [att]
        };

        var ct = new ClassTemplate(1)
        {
            Name = "Folder",
            NameSpace = "SP",
            IsEntity = true
        };
        var gen = Moq.Moq4IPocoGenerator(ct);

        //Act
        var sut = PocoClassGeneratorCs.GenerateCsPocoClass(gen, setting);
        var code = sut.ClassToString(ct);

        //Assert
        var expected = "public partial class Folder";
        Assert.That(code, Does.Contain(expected));
    }

    [Test]
    [TestCase("tab")]
    [TestCase("dm")]
    [TestCase("proto")]
    public void Class_is_entity_with_class_attributes_test(string att)
    {
        //Arrange
        var setting = new PocoSetting
        {
            Attributes = [att]
        };
        var ct = new ClassTemplate(1)
        {
            Name = "Folder",
            NameSpace = "SP",
            IsEntity = true
        };
        var gen = Moq.Moq4IPocoGenerator(ct);

        //Act
        var sut = PocoClassGeneratorCs.GenerateCsPocoClass(gen, setting);
        var code = sut.ClassToString(ct);
        //Assert

        var expectedDm = $"[DataContract]{_nl}public partial class Folder".TrimAllSpace();
        var expectedProto = $"[ProtoContract]{_nl}public partial class Folder".TrimAllSpace();
        var expectedTab = "public partial class Folder".TrimAllSpace();
        switch (att)
        {
            case "tab":
                Assert.That(code.TrimAllSpace(), Does.Contain(expectedTab));
                break;
            case "dm":
                Assert.That(code.TrimAllSpace(), Does.Contain(expectedDm));
                break;
            case "proto":
                Assert.That(code.TrimAllSpace(), Does.Contain(expectedProto));
                break;
        }
    }

    [Test]
    public void Class_renamed_has_no_json_attribute_test()
    {
        //Arrange
        var setting = new PocoSetting();
        var ct = new ClassTemplate(1)
        {
            Name = "Event",
            NameSpace = "SP",
            OriginalName = "event"
        };
        var gen = Moq.Moq4IPocoGenerator(ct);

        //Act
        var sut = PocoClassGeneratorCs.GenerateCsPocoClass(gen, setting);
        var code = sut.ClassToString(ct);
        //Assert
        var expected = "public partial class Event";
        Assert.That(code, Does.Contain(expected));
    }

    [Test]
    public void ReducedBaseTyp_in_the_same_namespace_test()
    {
        //Arrange
        var setting = new PocoSetting();
        var ct = new ClassTemplate(1)
        {
            Name = "Folder",
            NameSpace = "SP",
            BaseType = "SP.FileSystem"
        };
        var gen = Moq.Moq4IPocoGenerator(ct);

        //Act
        var sut = PocoClassGeneratorCs.GenerateCsPocoClass(gen, setting);
        var reducedType = sut.ReducedBaseTyp(ct);

        //Assert
        Assert.That(reducedType, Is.EqualTo("FileSystem"));
    }

    [Test]
    public void ReducedBaseTyp_in_different_namespace_test()
    {
        //Arrange
        var setting = new PocoSetting();

        var ct = new ClassTemplate(1)
        {
            Name = "Folder",
            NameSpace = "SP1",
            BaseType = "SP.FileSystem"
        };
        var gen = Moq.Moq4IPocoGenerator(ct);

        //Act
        var sut = PocoClassGeneratorCs.GenerateCsPocoClass(gen, setting);
        var reducedType = sut.ReducedBaseTyp(ct);
        //Assert
        Assert.That(reducedType, Is.EqualTo("SP.FileSystem"));
    }

    [Test]
    public void Rename_reserved_keyword_test()
    {
        //Arrange
        var setting = new PocoSetting();
        var ct = new ClassTemplate(1)
        {
            Name = "event",
            NameSpace = "SP",
            BaseType = "SP.FileSystem",
            OriginalName = "event"
        };
        var gen = Moq.Moq4IPocoGenerator(ct);

        //Act
        var sut = PocoClassGeneratorCs.GenerateCsPocoClass(gen, setting);
        var name = sut.ClassList.FirstOrDefault()?.Name;
        //Assert
        Assert.Multiple(() =>
        {
            Assert.That(sut.ClassList, Has.Count.EqualTo(1));
            Assert.That(name, Is.EqualTo("Event"));
        });
    }

    [Test]
    public void Prefix_namespace_test()
    {
        //Arrange
        var setting = new PocoSetting
        {
            NamespacePrefix = "abc"
        };
        var ct = new ClassTemplate(1)
        {
            Name = "Folder",
            NameSpace = "SP"
        };
        var gen = Moq.Moq4IPocoGenerator(ct);

        //Act
        var sut = PocoClassGeneratorCs.GenerateCsPocoClass(gen, setting);
        var code = sut.GeneratePoco();
        //Assert
        Assert.That(code, Does.Contain("abc.SP"));
    }

    [Test]
    public void Code_generation_test()
    {
        //Arrange
        var setting = new PocoSetting();
        var ct = new ClassTemplate(1)
        {
            Name = "Folder",
            NameSpace = "SP"
        };
        var gen = Moq.Moq4IPocoGenerator(ct);

        //Act
        var sut = PocoClassGeneratorCs.GenerateCsPocoClass(gen, setting);
        var code = sut.GeneratePoco();
        //Assert
        Assert.That(code, Does.Contain("// <auto-generated>"));
        Assert.That(code, Does.Contain("Service Url: http://localhost"));
        Assert.That(code, Does.Contain("MetaData Version: v4.0"));
        Assert.That(code, Does.Contain("namespace SP"));
        Assert.That(code, Does.Contain("public partial class Folder"));
    }

    [Test]
    public void Code_generation_with_multi_namespace_test()
    {
        //Arrange
        var setting = new PocoSetting();
        var list = new List<ClassTemplate>
        {
            new(1)
            {
                Name = "Folder", NameSpace = "SP"
            },
            new(2)
            {
                Name = "Folder", NameSpace = "SP2", BaseType = "SP.Folder"
            },
            new(3)
            {
                Name = "File", NameSpace = "SP2"
            }
        };
        var gen = Moq.Moq4IPocoGenerator(list);

        //Act
        var sut = PocoClassGeneratorCs.GenerateCsPocoClass(gen, setting);
        var code = sut.GeneratePoco();
        //Assert
        var expected = @"
using SP;
using SP2;
namespace SP
{
	public partial class Folder
	{
	}

}
namespace SP2
{
	public partial class Folder : SP.Folder
	{
	}

	public partial class File
	{
	}
}
";
        Assert.That(code.TrimAllSpace(), Does.Contain(expected.TrimAllSpace()));
    }

    [Test]
    public void Code_generation_with_one_namespace_test()
    {
        //Arrange
        var setting = new PocoSetting();
        var list = new List<ClassTemplate>
        {
            new(1)
            {
                Name = "Shape", NameSpace = "BookStore", IsComplex = true
            },
            new(2)
            {
                Name = "Circle", NameSpace = "BookStore", BaseType = "BookStore.Shape", IsComplex = true
            },
            new(3)
            {
                Name = "Rectangle",
                NameSpace = "BookStore",
                BaseType = "BookStore.Shape",
                IsEntity = true,
                EntitySetName = "Rectangle"
            }
        };
        var gen = Moq.Moq4IPocoGenerator(list);

        //Act
        var sut = PocoClassGeneratorCs.GenerateCsPocoClass(gen, setting);
        var code = sut.GeneratePoco();
        //Assert
        var expected = @"
namespace BookStore
{
	// Complex Entity
	public partial class Shape
	{
	}

	// Complex Entity
	public partial class Circle : Shape
	{
	}

	// EntitySetName: Rectangle
	public partial class Rectangle : Shape
	{
	}
}
";
        Assert.That(code.TrimAllSpace(), Does.Contain(expected.TrimAllSpace()));
    }

    [Test]
    public void Code_generation_with_basetype_without_namespace_test()
    {
        //Arrange
        var setting = new PocoSetting();
        var list = new List<ClassTemplate>
        {
            new(1)
            {
                Name = "Shape", NameSpace = "BookStore"
            },
            new(2)
            {
                Name = "Circle", NameSpace = "BookStore", BaseType = "Shape", IsComplex = true
            },
            new(3)
            {
                Name = "Rectangle",
                NameSpace = "BookStore",
                BaseType = "Shape",
                IsEntity = true,
                EntitySetName = "Rectangle"
            }
        };
        var gen = Moq.Moq4IPocoGenerator(list);

        //Act
        var sut = PocoClassGeneratorCs.GenerateCsPocoClass(gen, setting);
        var code = sut.GeneratePoco();
        //Assert
        var expected = @"
namespace BookStore
{
	public partial class Shape
	{
	}

	// Complex Entity
	public partial class Circle : Shape
	{
	}

	// EntitySetName: Rectangle
	public partial class Rectangle : Shape
	{
	}

}
";
        Assert.That(code.TrimAllSpace(), Does.Contain(expected.TrimAllSpace()));
    }
}
