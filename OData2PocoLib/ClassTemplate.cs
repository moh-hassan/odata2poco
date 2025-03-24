// Copyright (c) Mohamed Hassan & Contributors. All rights reserved. See License.md in the project root for license information.

#pragma warning disable CA1721, CA2227
namespace OData2Poco;

using CustAttributes;
using Extensions;

/// <summary>
///     Define the properties of the class
/// </summary>
public sealed class ClassTemplate : IEquatable<ClassTemplate?>, IComparable<ClassTemplate>
{
    private static int s_nextId = 1; // Static counter to generate unique IDs
    private readonly AttributeFactory _attributeFactory = AttributeFactory.Default;

    public ClassTemplate() : this(s_nextId++)
    {
    }

    public ClassTemplate(int id)
    {
        Id = id;
        Properties = [];
        Keys = [];
        EnumElements = [];
        Navigation = [];
        BaseType = string.Empty;
        Comment = string.Empty;
        Name = "UNDEFINED";
        NameSpace = string.Empty;
    }

    public int Id { get; set; }
    public string Name { get; set; }
    public string BaseType { get; set; }
    public string? Comment { get; set; }
    public List<PropertyTemplate> Properties { get; set; }
    public List<string> Keys { get; set; }
    public List<string> Navigation { get; set; }

    //to support enum generation code
    public bool IsEnum { get; set; }
    public bool IsFlags { get; set; } //v3, Add [FlagsAttribute] to enum
    public List<string> EnumElements { get; set; }
    public string OriginalName { get; set; } = null!;

    //v1.4.0
    public string? EntitySetName { get; set; }
    public string NameSpace { get; set; }

    public string FullName => string.IsNullOrEmpty(NameSpace)
        ? Name
        : $"{NameSpace}.{Name}";

    public bool IsComplex { get; set; }
    public bool IsEntity { get; set; }
    public bool IsAbstrct { get; set; }
    public bool IsOpen { get; set; }
    public bool IsOneOf { get; set; }
    public List<ClassTemplate> InnerClasses { get; set; } = [];

    public List<string> GetAllAttributes()
    {
        return _attributeFactory.GetAllAttributes(this);
    }

    public string GetComment()
    {
        var complex = IsComplex ? "Complex Entity" : string.Empty;
        var openType = IsOpen ? "OpenType" : string.Empty;
        var entityType = IsEntity ? $"EntitySetName: {EntitySetName}" : string.Empty;
        var comments = new[]
        {
            openType, entityType, complex
        }.Where(a => a.Length > 0);

        return string.Join(", ", comments);
    }

    public override string ToString()
    {
        // 'id:name(parent)'
        return string.IsNullOrEmpty(BaseType)
            ? $"{Id}:{Name}"
            : $"{Id}:{Name}({BaseType.Reduce()})";
    }

    #region IEquatable and Comparer

    public override bool Equals(object? obj)
    {
        return Equals(obj as ClassTemplate);
    }

    public bool Equals(ClassTemplate? other)
    {
        return other is not null && Id == other.Id;
    }

    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }

    public int CompareTo(ClassTemplate? other)
    {
        var compareTo = FullName == other?.BaseType
            ? -1
            : string.Compare(FullName, other?.FullName, StringComparison.Ordinal);
        return BaseType == other?.FullName
            ? 1
            : compareTo;
    }

    #endregion
}
