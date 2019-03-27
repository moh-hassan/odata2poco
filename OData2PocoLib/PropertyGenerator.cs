using System;
using System.Collections.Generic;
using OData2Poco.CustAttributes;
using OData2Poco.Extensions;

namespace OData2Poco
{
    /// <summary>
    /// Generate All attributes of property :KeyAttribut,Required,JsonProperty
    /// Convert name to Camel/Pascal Case
    /// Generate the declaration of property e.g.   virtual public int? name  {get;set;} //comment
    /// </summary>
 public   class PropertyGenerator 
    {
        private readonly AttributeFactory _attributeManager = AttributeFactory.Default;
        private readonly PropertyTemplate _property;  
        private readonly PocoSetting _setting; 
        /// <summary>
        /// Initialize in cto
        /// </summary>
        /// <param name="propertyTemplate"></param>
        /// <param name="pocoSetting"></param>
        public PropertyGenerator(PropertyTemplate propertyTemplate, PocoSetting pocoSetting)
        {
            _property = propertyTemplate;
            _setting = pocoSetting;
        }
    
        /// <summary>
        /// Get all attributes based on PocoSetting initialization
        /// </summary>
        /// <returns></returns>
        public List<string> GetAllAttributes()
        {
           
            return _attributeManager.GetAllAttributes(_property);
        }
        /// <summary>
        /// Name in camlcase /pascase
        /// </summary>
        public string Name
        {
            get
            {
                switch (_setting.NameCase)
                {
                    case CaseEnum.Pas:
                        return _property.PropName.ToPascalCase();

                    case CaseEnum.Camel:
                        return _property.PropName.ToCamelCase();

                    case CaseEnum.None:
                        return _property.PropName;
                    default:
                        return _property.PropName;
                }
            }
        }

        /// <summary>
        /// Virtual Modifier
        /// </summary>
        public string VirtualModifier => _setting.AddNavigation && !_setting.AddEager ? " virtual" : string.Empty;

        /// <summary>
        /// NullableModifier represented by "?" added to type , e.g int?
        /// </summary>
        public string NullableModifier => _setting.AddNullableDataType && _property.IsNullable ? Helper.GetNullable(_property.PropType) : String.Empty;

        /// <summary>
        /// The declaration of property in C# 
        /// </summary>
        public string Declaration => $"public{VirtualModifier} {_property.PropType + NullableModifier} {Name} {{get;set;}} {_property.PropComment}";

        public override string ToString()
        {
            var text = $"{string.Join(Environment.NewLine, GetAllAttributes())}\n{Declaration}";
            return text;
        }
      
    }//
}//

