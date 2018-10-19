using System.Collections.Generic;
using OData2Poco.CustAttributes;


namespace OData2Poco
{
    /// <summary>
    /// Define the propertis of the class 
    /// </summary>
    public partial class ClassTemplate
    {
        public string Name { get; set; }
        public string BaseType { get; set; }
        public string Comment { get; set; }
        public string ToDebugString { get; set; }
        public List<PropertyTemplate> Properties { get; set; }
        public List<string> Keys { get; set; }
        public List<string> Navigation { get; set; }
        
        //to support enum generation code
        public bool IsEnum { get; set; }
        public bool IsFlags { get; set; } //v3, Add [FlagsAttribute] to enum
        public List<string> EnumElements { get; set; }
        public ClassTemplate()
        {
            Properties = new List<PropertyTemplate>();
            Keys = new List<string>();
            EnumElements = new List<string>();
            Navigation = new List<string>();
        }
        //v1.4.0
        public string EntitySetName { get; set; }
        public bool IsComplex { get; set; }

        private readonly AttributeFactory _attributeFactory = AttributeFactory.Default;

        public List<string> GetAllAttributes() => _attributeFactory.GetAllAttributes(this);
    }
}