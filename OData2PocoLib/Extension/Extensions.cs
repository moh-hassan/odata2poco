using System;
using System.Collections.Generic;
using OData2Poco.TextTransform;

namespace OData2Poco.Extension
{
    public static class Extensions
    {
        public static string List2String(this List<string> list)
        {
            string text = string.Join(",", list);
            return text;
        }

        public  static string GetInnerException(this Exception ex)
        {
            if (ex.InnerException != null)
            {
                return string.Format("{0} ---> {1} ", ex.InnerException.Message, GetInnerException(ex.InnerException));//recursive
            }
            return "";
        }

        public static string FullExceptionMessage(this Exception ex, bool showTrace = false)
        {

            var s = ex.GetInnerException();
            var msg= string.IsNullOrEmpty(s) ? ex.Message : ex.Message + "-->" + s;

            if (showTrace)
                return string.Format("{0}\nException Details:\n {1}",msg ,ex);
            return msg;
        }
        //todo: _header implementation 
        /// <summary>
        /// Generte C# code for a given  Entity using FluentCsTextTemplate
        /// </summary>
        /// <param name="ent"> Class  to generate code</param>
        /// <param name="includeNamespace"></param>
        /// <returns></returns>
        public static  string ToCsCode(this ClassTemplate ent, bool includeNamespace = false)
        {
            var csTemplate = new FluentCsTextTemplate();
            //if (includeNamespace) csTemplate.WriteLine(_header());


            ////for enum
            if (ent.IsEnum)
            {
                var elements = string.Join(", ", ent.EnumElements.ToArray());
                var enumString = string.Format("public enum {0} {{ {1} }}", ent.Name, elements);
                return enumString;
            }

            //v1.4
            //add TableAttribute
            if (ent.EntitySetName != "")
            {
                var tableAtt = string.Format("Table(\"{0}\")", ent.EntitySetName);
                csTemplate.PushIndent("\t").WriteLineAttribute(tableAtt).PopIndent();
            }
            csTemplate.StartClass(ent.Name);
            foreach (var p in ent.Properties)
            {
                //@@@ v1.0.0-rc3
                //skip navigation properties
                if (p.IsNavigate) continue;

                //v1.4
                //add attributes
                if (p.IsKey) csTemplate.WriteLineAttribute("Key");
                if (!p.IsNullable) csTemplate.WriteLineAttribute("Required");

                // if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>)) 
                csTemplate.WriteLineProperty(p.PropType, p.PropName, comment: p.PropComment);
            }
            csTemplate.EndClass();
            if (includeNamespace) csTemplate.EndNamespace(); //"}" for namespace

            return csTemplate.ToString();
        }

       
    }
}
 
