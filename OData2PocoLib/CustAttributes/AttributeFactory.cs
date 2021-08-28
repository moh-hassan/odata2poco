using System;
using System.Collections.Generic;


namespace OData2Poco.CustAttributes
{
    
    public class AttributeFactory
    {
        private List<string> _attributes;
        private readonly PocoAttributesList _pocoAttributesList;
        private static readonly Lazy<AttributeFactory> Lazy =
            new Lazy<AttributeFactory>(() => new AttributeFactory());
        public List<string> SupportedAttributes=>_pocoAttributesList.SupportedAttributes();
        public static AttributeFactory Default => Lazy.Value;

        private AttributeFactory()
        {
            _pocoAttributesList = new PocoAttributesList();
            _attributes = new List<string>();
         
        }
        /// <summary>
        /// Initialize factory with setting.Attributes
        /// </summary>
        /// <param name="setting"></param>
        /// <returns></returns>
        public AttributeFactory Init(PocoSetting? setting=null)
        {
            if (setting==null)
                setting= new PocoSetting();

            //obsolet: -k -q -j -t are replaced by -a <names>
            var list = setting.Attributes;
            if (setting.AddKeyAttribute && !list.Contains("key"))
                list.Add("key");

            if (setting.AddRequiredAttribute && !list.Contains("req"))
                list.Add("req");

            if (setting.AddTableAttribute && !list.Contains("tab"))
                list.Add("tab");

            //    case "json":
            if (setting.AddJsonAttribute && !list.Contains("json"))
                list.Add("json");
            
            _attributes = new List<string>(setting.Attributes);//add attributes of commandline options
            return this;
        }
      
        private  INamedAttribute? GetAttributeObject(string attName) => _pocoAttributesList[attName];


        public  List<string> GetAttributes(object property, string attName)
        {
            if (attName.StartsWith("[") && property is PropertyTemplate)
                return new List<string> { attName };

            var attributeObject = GetAttributeObject(attName);
            return property switch
            {
                PropertyTemplate p => attributeObject != null ? attributeObject.GetAttributes(p) : new List<string>(),
                ClassTemplate c => attributeObject != null ? attributeObject.GetAttributes(c) : new List<string>(),
                _ => throw new Exception($"{property.GetType()} isn't supported for named attributes")
            };
        }

        public  List<string> GetAttributes(object property, List<string> attNames)
        {
            var list = new List<string>();
            foreach (var s in attNames)
            {
                var att = GetAttributes(property, s);
                att.ForEach(x =>
                {
                    if (!list.Contains(x))
                        list.Add(x);
                });
            }

            return list;
        }


        public  List<string> GetAllAttributes(object property)
        {
            return GetAttributes(property, _attributes);
        }

    }
}

