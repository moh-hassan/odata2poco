using System;
using System.Collections.Generic;
using OData2Poco.CustAttributes;
using OData2Poco.Extension;

namespace OData2Poco
{
    /// <summary>
    /// Generate All attributes of property :KeyAttribut,Required,JsonProperty
    /// Convert name to Camel/Pascal Case
    /// Generate the declaration of property e.g.   virtual public int? name  {get;set;} //comment
    /// </summary>
 public   class PropertyGenerator
    {
        private AttributeFactory AttributeManager = AttributeFactory.Default;
        private readonly PropertyTemplate _property; //{ get; set; }
        private readonly PocoSetting _setting;// { get; set; }
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
     //todo: support user defined custom attributes for properties/class
        /// <summary>
        /// Get all attributes based on PocoSetting initialization
        /// </summary>
        /// <returns></returns>
        public List<string> GetAllAttributes()
        {
            //var list = new List<string>();

            ////required Attribute
            //if (_setting.AddRequiredAttribute)
            //{
            //    // if (!Property.IsNullable) list.Add(_getAttribute("Required"));
            //    if (!_property.IsNullable) list.Add("Required".ToCsAttribute());
            //}

            //if (_setting.AddKeyAttribute)
            //{
            //    if (_property.IsKey) list.Add("Key".ToCsAttribute());
            //}

            //if (_setting.AddJsonAttribute)
            //{
            //    list.Add(string.Format("[JsonProperty(PropertyName = \"{0}\")]", _property.PropName));
            //}

            //if (_setting.AddDataMemberAttribute)
            //{
            //    list.Add("DataMember".ToCsAttribute());
            //}
            //return list;
            return AttributeManager.GetAllAttributes(_property);
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
        public string VirtualModifier
        {
            get
            {
                return _setting.AddNavigation && !_setting.AddEager ? "virtual" : String.Empty;
            }
        }

        /// <summary>
        /// NullableModifier represented by "?" added to type , e.g int?
        /// </summary>
        public string NullableModifier
        {
            get
            {
                return _setting.AddNullableDataType && _property.IsNullable ? Helper.GetNullable(_property.PropType) : String.Empty;
            }
        }

        /// <summary>
        /// The declaration of property in C# 
        /// </summary>
        public string Declaration
        {
            get
            {
                return string.Format("{0} {1} {2} {3} {{get;set;}} {4}\n",
                VirtualModifier, "public", _property.PropType + NullableModifier, Name, _property.PropComment);
            }
        }

        public override string ToString()
        {
            var text = string.Format("{0}\n{1}",
                string.Join(Environment.NewLine,GetAllAttributes()), Declaration);
            return text;
        }
      
    }//
}//

