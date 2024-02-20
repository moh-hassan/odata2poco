// Copyright (c) Mohamed Hassan & Contributors. All rights reserved. See License.md in the project root for license information.

namespace OData2Poco.CustAttributes.NamedAtributes;

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
    public string Name { get; set; } = "original";
    public string Scope { get; set; } = "property";
    public bool IsUserDefined { get; set; }
    public bool IsValid { get; set; } = true;

    public List<string> GetAttributes(PropertyTemplate propertyTemplate)
    {
        _ = propertyTemplate ?? throw new ArgumentNullException(nameof(propertyTemplate));
        return
        [
            propertyTemplate.OriginalName != propertyTemplate.PropName
                ? $"[JsonProperty(\"{propertyTemplate.OriginalName}\")]"
                : string.Empty
        ];
    }

    public List<string> GetAttributes(ClassTemplate classTemplate)
    {
        return [];
    }
}
