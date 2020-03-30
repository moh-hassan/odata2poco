#nullable disable
namespace OData2Poco
{
   
    public partial class PropertyTemplate
    {
        public string PropName { get; set; }
        public string PropType { get; set; }
        public string PropComment { get; set; }
        public bool IsKey { get; set; }
        public bool IsNavigate { get; set; }
        public bool IsNullable { get; set; }
        //public bool Iscomputed { get; set; }
        public int Serial { get; set; }
        public string ClassName { get; set; }
        public string ClassNameSpace { get; set; }
        public string OriginalName { get; set; }
        public string OriginalType { get; set; }
        public int? MaxLength { get; set; }
        public int? Precision { get; set; }
        public int? Scale { get; set; }
        public bool IsReadOnly { get; set; }
    }
}

#nullable restore
