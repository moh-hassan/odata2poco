#nullable disable
using System;
using System.Collections.Generic;

namespace OData2Poco
{

    sealed public partial class PropertyTemplate// : IEquatable<PropertyTemplate>
    {
        public string PropName { get; set; }
        public string PropType { get; set; }
        public string PropComment { get; set; }
        public bool IsKey { get; set; }
        public bool IsNavigate { get; set; }
        public bool IsNullable { get; set; }
        public int Serial { get; set; }
        public string ClassName { get; set; }
        public string ClassNameSpace { get; set; }
        public string OriginalName { get; set; }
        public string OriginalType { get; set; }
        public int? MaxLength { get; set; }
        public int? Precision { get; set; }
        public int? Scale { get; set; }
        public bool IsReadOnly { get; set; }

#region IEquatable and Comparer
        public override bool Equals(object obj) => Equals(obj as PropertyTemplate);

        public bool Equals(PropertyTemplate other) =>
            other is not null && PropName == other.PropName;

        public override int GetHashCode() => PropName.GetHashCode();

        public static bool operator ==(PropertyTemplate left, PropertyTemplate right) => EqualityComparer<PropertyTemplate>.Default.Equals(left, right);

        public static bool operator !=(PropertyTemplate left, PropertyTemplate right) =>
            !(left == right);

#endregion

    }
}

#nullable restore
