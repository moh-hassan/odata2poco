// Copyright (c) Mohamed Hassan & Contributors. All rights reserved. See License.md in the project root for license information.

#nullable disable
namespace OData2Poco;

public sealed class PropertyTemplate : IEquatable<PropertyTemplate>
{
    public string PropName { get; set; }
    public string PropType { get; set; }
    public string PropComment { get; set; }
    public bool IsKey { get; set; }
    public bool IsNavigate { get; set; }
    public bool IsNullable { get; set; }
    public int Serial { get; init; }
    public string ClassName { get; set; }
    public string ClassNameSpace { get; init; }
    public string OriginalName { get; set; }
    public string OriginalType { get; set; }
    public int? MaxLength { get; set; }
    public int? Precision { get; set; }
    public int? Scale { get; set; }
    public bool IsReadOnly { get; set; }
    internal bool InConstructor => !IsNullable && !IsNavigate;

    #region IEquatable and Comparer

    public override bool Equals(object obj)
    {
        return Equals(obj as PropertyTemplate);
    }

    public bool Equals(PropertyTemplate other)
    {
        return other is not null && PropName == other.PropName;
    }

    public override int GetHashCode()
    {
        return $"{Serial}+{ClassNameSpace}".GetHashCode();
    }

    #endregion
}
