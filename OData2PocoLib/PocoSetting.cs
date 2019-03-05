using System.Collections.Generic;

namespace OData2Poco
{
    /// <summary>
    /// Setting options to control the code generation
    /// </summary>
    public class PocoSetting
    {
        /// <summary>
        /// Set nullabable ? to the type of property
        /// Example int? , double?
        /// </summary>
        public bool AddNullableDataType { get; set; }
       
        /// <summary>
        /// Add Navigation properties as virtual properties
        /// </summary>
        public bool AddNavigation { get; set; }
        /// <summary>
        /// Add Navigation properties as non virtual properties for eager loading
        /// </summary>
        public bool AddEager { get; set; }
        //public bool AddDataContractAttribute { get; set; }
        /// <summary>
        /// The language of code generation, Defalt is CS 
        /// Current supported language is C# only
        /// </summary>
        public Language Lang { get; set; }

        /// <summary>
        /// Gets or sets the string to use after the colon of a class name for the base class to inherit and/or interfaces to implement
        /// </summary>
        /// <value>
        /// Base class and/or interfaces to implement
        /// </value>
        public string Inherit { get; set; }
        /// <summary>
        /// Indicates whether or not to generate classes that follow the inheritance hierarchy of the ODATA types. Default is true. Disable by setting Inherit to a non-null value.
        /// 
        /// </summary>
        public bool UseInheritance => string.IsNullOrEmpty(Inherit);
        /// <summary>
        /// Gets or sets a namespace prefix.
        /// </summary>
        /// <value>
        /// The namespace.
        /// </value>
        public string NamespacePrefix { get; set; }
        /// <summary>
        /// Gets or sets a NameCase: Pas/Camel/None for string conversion.
        /// </summary>
        public CaseEnum NameCase { get; set; }

      

        //add attributes: key,req,dm,tab,json,proto,display and db
        public List<string> Attributes { get; set; }

        /// <summary>
        /// Add KeyAttribute [Key] to the property of POCO class
        /// </summary>
        public bool AddKeyAttribute { get; set; }//obsolete, use Attributes.add("key")

        public bool AddTableAttribute { get; set; } //obsolete, use Attributes.add("tab")

       public bool AddRequiredAttribute { get; set; }//obsolete, use Attributes.add("req")
       
        /// <summary>
        /// Add JsonProperty Attribute
        ///example:     [JsonProperty(PropertyName = "email")]
        /// </summary>
        public bool AddJsonAttribute { get; set; } ////obsolete, use Attributes.add("json")
        /// <summary>
        /// Initialization
        /// </summary>
        public PocoSetting()
        {
            Lang = Language.CS;
            NamespacePrefix = string.Empty;
            Inherit = null;
            NameCase = CaseEnum.None;
            AddJsonAttribute = false;
            Attributes = new List<string>();
        }

    }
}
