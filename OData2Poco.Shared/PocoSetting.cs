namespace OData2Poco
{
    public class PocoSetting
    {
        /// <summary>
        /// SEt nullabable ? to the type of property
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
        //public bool AddDataContractAttribute { get; set; }
        /// <summary>
        /// The language of code generation, Defalt is CS 
        /// Current supported language is C# only
        /// </summary>
        public Language Lang { get; set; }

        public PocoSetting()
        {
            Lang= Language.CS;
        }
       
    }
}
