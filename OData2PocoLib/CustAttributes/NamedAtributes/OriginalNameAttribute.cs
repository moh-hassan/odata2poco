using System.Collections.Generic;

namespace OData2Poco.CustAttributes.NamedAtributes
{
    /*
      Method to keep originalName after renaming name due to conflication with reserved keywords
       [OriginalName("Applicant")]
		//or
		[DataMember(Name = "Applicant")]
		//or
		 [JsonProperty("Applicant")]
	    public virtual string applicant {get;set;}  //renamed to applicant
     */
    public class OriginalNameAttribute : INamedAttribute
    {
        public string Name { get; } = "original";

        public List<string> GetAttributes(PropertyTemplate property) =>
            new List<string> {
                property.OriginalName !=property.PropName 
                  //  ? $"//[OriginalName({property.OriginalName})]"
                ?$"[JsonProperty(\"{property.OriginalName}\")]"
                :string.Empty,
                };

        public List<string> GetAttributes(ClassTemplate classTemplate) =>
            new List<string>
            {
               classTemplate.OriginalName !=classTemplate.Name
                //? $"//[OriginalName({classTemplate.OriginalName})]"
                ?$"[JsonProperty(\"{classTemplate.OriginalName}\")]"
                :string.Empty,
                };
    }
}

