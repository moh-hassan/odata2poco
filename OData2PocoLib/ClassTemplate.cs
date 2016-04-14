using System.Collections.Generic;

namespace OData2Poco
{
    public class ClassTemplate
    {
        public string Name { get; set; }
        public string Comment { get; set; }
        public string ToDebugString { get; set; }
        public List<PropertyTemplate> Properties { get; set; }
        public List<string> Keys { get; set; }
        public List<string> Navigation { get; set; }
        // public CodeTemplat CodeTemplatType { get; set; }
        //to support enum generation code
        public bool IsEnum { get; set; }
        public List<string> EnumElements { get; set; }
        public ClassTemplate()
        {
            Properties = new List<PropertyTemplate>();
            Keys = new List<string>();
            EnumElements = new List<string>();
            Navigation = new List<string>();
        }
    }
}