namespace OData2Poco
{
    public class PropertyTemplate
    {
        public string PropName { get; set; }
        public string PropType { get; set; }
        public string PropComment { get; set; }
        //for debuging
        //public string ToDebugString { get; set; }
        public bool IsKey { get; set; }
        public bool IsNavigate { get; set; }
        public bool IsNullable { get; set; }
        public bool Iscomputed { get; set; }
        //public string ToTrace { get; set; }

    }
}