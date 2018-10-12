using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;


//using JetBrains.Annotations;

namespace OData2Poco.CustAttributes
{
    //todo: support alias names, e.g dm== datamember, disp==displayname
    public class AttributeFactory
    {
        public  List<string> _attributes;
         //readonly ConColor _console = ConColor.Default;
        public readonly PocoAttributesList _pocoAttributesList;
        private static readonly Lazy<AttributeFactory> Lazy =
            new Lazy<AttributeFactory>(() => new AttributeFactory());

        public static AttributeFactory Default => Lazy.Value;

        private AttributeFactory()
        {
            _pocoAttributesList = new PocoAttributesList();
            _attributes = new List<string>();
            //InitAttributes(new List<string>());
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="setting"></param>
        /// <returns></returns>
        public AttributeFactory Init(PocoSetting setting=null)
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
        
            InitAttributes(list);
            return this;
        }

        private  void InitAttributes(List<string> attList)
        {

            _attributes = new List<string>(attList);//add attributes of commandline options
            var list = _attributes.Where(x => !x.StartsWith("["));
           

        }
        private  INamedAttribute GetAttributeObject(string attName) => _pocoAttributesList[attName];


        public  List<string> GetAttributes(object property, string attName)
        {
            if (attName.StartsWith("[") && property is PropertyTemplate)
                return new List<string> { attName };

            INamedAttribute attributeObject = GetAttributeObject(attName);
            switch (property)
            {
                case PropertyTemplate p:
                    return attributeObject != null
                        ? attributeObject.GetAttributes(p)
                        : new List<string>();

                case ClassTemplate c:
                    return attributeObject != null
                        ? attributeObject.GetAttributes(c)
                        : new List<string>();
                default:
                    throw new Exception($"{property.GetType()} isn't supported for named attributes");
            }
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

