using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;

namespace OData2Poco.Shared
{
    /// <summary>
    /// Generate All attributes of property :KeyAttribut,Required,JsonProperty
    /// Convert name to Camel/Pascal Case
    /// Generate the declaration of property e.g.   virtual public int? name  {get;set;} //comment
    /// </summary>
 public   class PropertyGenerator
    {
        public readonly PropertyTemplate Property; //{ get; set; }
        public readonly PocoSetting Setting;// { get; set; }
        /// <summary>
        /// Initialize in cto
        /// </summary>
        /// <param name="propertyTemplate"></param>
        /// <param name="pocoSetting"></param>
        public PropertyGenerator(PropertyTemplate propertyTemplate, PocoSetting pocoSetting)
        {
            Property = propertyTemplate;
            Setting = pocoSetting;
        }

        //public string JsonAttribute
        //{
        //    get
        //    {
        //        if (Setting.AddJsonAttribute)
        //        {
        //            return string.Format("[JsonProperty(PropertyName = \"{0}\")]", Property.PropName);
        //        }
        //        return string.Empty;
        //    }
        //}
        //public string KeyAttribute
        //{
        //    get
        //    {
        //        if (Setting.AddKeyAttribute)
        //        {
        //            if (Property.IsKey) return string.Format("[{0}]", "Key");
        //        }
        //        return string.Empty;
        //    }
        //}
        //[DataMember]
        //public string DataMemberAttribute
        //{
        //    get
        //    {
        //        if (Setting.AddKeyAttribute)
        //        {
        //            return string.Format("[{0}]", "DataMember");
        //        }
        //        return string.Empty;
        //    }
        //}
        //public string RequiredAttribute
        //{
        //    get
        //    {
        //        if (Setting.AddRequiredAttribute)
        //        {
        //            if (!Property.IsNullable) return string.Format("[{0}]", "Required");
        //        }
        //        return string.Empty;
        //    }
        //}


        //private readonly Func<string, string> _getAttribute = (s) => string.Format("[{0}]", s);

        //private readonly Func<string ,string> _getSet = (name) => name + " {get;set;}";
        //public string Name
        //{
        //    get
        //    {

        //        switch (Setting.NameCase)
        //        {
        //            case CaseEnum.Pas:
        //                return Property.PropName.ToPascalCase();

        //            case CaseEnum.Camel:
        //                return Property.PropName.ToCamelCase();

        //            default:
        //                return Property.PropName;
        //        }

        //    }
        //}

        public List<string> GetAllAttributes()
        {
            var list = new List<string>();

            //required Attribute
            if (Setting.AddRequiredAttribute)
            {
                // if (!Property.IsNullable) list.Add(_getAttribute("Required"));
                if (!Property.IsNullable) list.Add("Required".ToCsAttribute());
            }

            if (Setting.AddKeyAttribute)
            {
                if (Property.IsKey) list.Add("Key".ToCsAttribute());
            }

            if (Setting.AddJsonAttribute)
            {
                list.Add(string.Format("[JsonProperty(PropertyName = \"{0}\")]", Property.PropName));
            }

            if (Setting.AddDataMemberAttribute)
            {
                list.Add("DataMember".ToCsAttribute());
            }
            return list;
        }
        /// <summary>
        /// Name in camlcase /pascase
        /// </summary>
        public string Name
        {
            get
            {
                switch (Setting.NameCase)
                {
                    case CaseEnum.Pas:
                        return Property.PropName.ToPascalCase();

                    case CaseEnum.Camel:
                        return Property.PropName.ToCamelCase();

                    case CaseEnum.None:
                        return Property.PropName;
                    default:
                        return Property.PropName;
                }
            }
        }

        //public string PropertyDeclaration2()
        //{

        //    //set nullable data types,e.g: int ?
        //    //var nulType = Setting.AddNullableDataType && Property.IsNullable
        //    //    ? Helper.GetNullable(Property.PropType)
        //    //    : "";
        //    var isVirtual = (Setting.AddNavigation && !Setting.AddEager);

        //    //string typeName = Property.PropType;
        //    ////string name=Name;
        //    string visiblity = "public";
        //    ////bool isVirtual = virtualprop;
        //    //bool isNullable = Property.IsNullable; // false;
        //    //string comment = Property.PropComment;


        //    //set nullable data types
        //    var nulType = Setting.AddNullableDataType && Property.IsNullable ? Helper.GetNullable(Property.PropType) : "";

        //    //var virtualprop = (PocoSetting.AddNavigation && !PocoSetting.AddEager);


        //    //property is declared in that order:
        //    //virtual public int? name  {get;set;} //comment

        //    var virtualText = isVirtual ? "virtual" : string.Empty;
        //    //var nullableText = Setting.AddNullableDataType && Property.IsNullable ? "?" : string.Empty;
        //    //var text = string.Format("{0} {1} {2}{3} {4} {{get;set;}} {5}",
        //    //    virtualText,
        //    //    visiblity,
        //    //    Property.PropType,
        //    //    nulType,
        //    //    Name,
        //    //    Property.PropComment);

        //    //declaration is in that order
        //    List<string> list = new List<string>
        //    {
        //        virtualText,
        //        visiblity,
        //        Property.PropType + nulType,
        //        Name,
        //        "{get;set;}",
        //        Property.PropComment
        //    };

        //    //return text.TrimAllSpace() + Environment.NewLine;
        //    var result = String.Join(" ", list);
        //    return result + Environment.NewLine;

        //}
        //public string GetDeclaration()
        //{

        //        //set nullable data types,e.g: int ?
        //        //var nulType = Setting.AddNullableDataType && Property.IsNullable
        //        //    ? Helper.GetNullable(Property.PropType)
        //        //    : "";
        //        var isVirtual = (Setting.AddNavigation && !Setting.AddEager);

        //        //string typeName = Property.PropType;
        //        ////string name=Name;
        //        string visiblity = "public";
        //        ////bool isVirtual = virtualprop;
        //        //bool isNullable = Property.IsNullable; // false;
        //        //string comment = Property.PropComment;


        //        //set nullable data types
        //        var nulType = Setting.AddNullableDataType && Property.IsNullable ? Helper.GetNullable(Property.PropType) : "";

        //        //var virtualprop = (PocoSetting.AddNavigation && !PocoSetting.AddEager);



        //        //virtual public int ? name  {get;set;} //comment

        //        var virtualText = isVirtual ? "virtual " : string.Empty;
        //        var nullableText = Setting.AddNullableDataType && Property.IsNullable ? "?" : string.Empty;
        //        var text = string.Format("{0} {1} {2}{3} {4} {{get;set;}} {5}",
        //            virtualText,
        //            visiblity,
        //            Property.PropType + nulType,
        //            Name,
        //            Property.PropComment);
        //        return text.TrimAllSpace() + Environment.NewLine;

        //}

        public string VirtualModifier
        {
            get
            {
                return Setting.AddNavigation && !Setting.AddEager ? "virtual" : String.Empty;
            }
        }

        public string NullableModifier
        {
            get
            {
                return Setting.AddNullableDataType && Property.IsNullable ? Helper.GetNullable(Property.PropType) : String.Empty;
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
                VirtualModifier, "public", Property.PropType + NullableModifier, Name, Property.PropComment);
            }
        }

        public override string ToString()
        {
            var text = string.Format("{0}\n{1}",
                string.Join(Environment.NewLine,GetAllAttributes()), Declaration);
            return text;
        }

        //public string GetDeclaration2()
        //{

        //    //set nullable data types,e.g: int ?
        //    //var nulType = Setting.AddNullableDataType && Property.IsNullable
        //    //    ? Helper.GetNullable(Property.PropType)
        //    //    : "";
        //    var isVirtual = (Setting.AddNavigation && !Setting.AddEager);

        //    //string typeName = Property.PropType;
        //    ////string name=Name;
        //    string visibility = "public";
        //    ////bool isVirtual = virtualprop;
        //    //bool isNullable = Property.IsNullable; // false;
        //    //string comment = Property.PropComment;


        //    //set nullable data types
        //    var nulType = Setting.AddNullableDataType && Property.IsNullable ? Helper.GetNullable(Property.PropType) : "";

        //    //var virtualprop = (PocoSetting.AddNavigation && !PocoSetting.AddEager);


        //    //property is declared in that order:
        //    //virtual public int? name  {get;set;} //comment

        //    var virtualText = isVirtual ? "virtual" : string.Empty;
        //    //var nullableText = Setting.AddNullableDataType && Property.IsNullable ? "?" : string.Empty;
        //    var text = string.Format("{0} {1} {2} {3} {{get;set;}} {4}",
        //        virtualText,
        //        visibility,
        //        Property.PropType +nulType,
        //        Name,
        //        Property.PropComment);

        //    //declaration is in that order
        //    //List<string> list = new List<string>
        //    //{
        //    //    virtualText,
        //    //    visibility,
        //    //    Property.PropType + nulType,
        //    //    _getSet(Name),
        //    //    //Name,
        //    //    //"{get;set;}",
        //    //    Property.PropComment
        //    //};

        //    ////return text.TrimAllSpace() + Environment.NewLine;
        //    //var result = String.Join(" ", list);
        //    //return result + Environment.NewLine;
        //    return text.TrimAllSpace().NewLine();

        //}
    }//
}//

