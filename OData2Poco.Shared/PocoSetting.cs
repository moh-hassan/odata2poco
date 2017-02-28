namespace OData2Poco
{
    public class PocoSetting
    {
        /// <summary>
        /// Set nullabable ? to the type of property
        /// Example int? , double?
        /// </summary>
        public bool AddNullableDataType { get; set; }
        /// <summary>
        /// Add KeyAttribute [Key] to the property of POCO class
        /// </summary>
        public bool AddKeyAttribute { get; set; }
        /// <summary>
        ///  Add KeyAttribute [Key] to the POCO class 
        /// </summary>
        public bool AddTableAttribute { get; set; }
        /// <summary>
        ///  Add RequiredAttribute [Required] to the property of POCO class
        /// </summary>
        public bool AddRequiredAttribute { get; set; }
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
        /// Gets or sets a namespace prefix.
        /// </summary>
        /// <value>
        /// The namespace.
        /// </value>
        public string NamespacePrefix { get; set; }

        public PocoSetting()
        {
            Lang= Language.CS;
            NamespacePrefix = string.Empty;
            Inherit = string.Empty;
        }
       
    }
}
