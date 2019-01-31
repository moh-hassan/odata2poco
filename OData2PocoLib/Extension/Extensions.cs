using System;
using System.Collections.Generic;
using System.Text;

namespace OData2Poco.Extension
{
    public static class Extensions
    {
        /// <summary>
        /// Merge list to one string
        /// </summary>
        /// <param name="list"></param>
        /// <param name="separator"></param>
        /// <returns></returns>
        public static string List2String(this List<string> list,string separator=",")
        {
            string text = string.Join(separator, list);
            return text;
        }

        public  static string GetInnerException(this Exception ex)
        {
            if (ex.InnerException != null)
            {
                return $"{ex.InnerException.Message} ---> {GetInnerException(ex.InnerException)} ";//recursive
            }
            return "";
        }

        public static string FullExceptionMessage(this Exception ex, bool showTrace = false)
        {

            var s = ex.GetInnerException();
            var msg= string.IsNullOrEmpty(s) ? ex.Message : ex.Message + "-->\n" + s;

            if (showTrace)
                return $"{msg}\nException Details:\n {ex}";
            return msg;
        }
        //todo: _header implementation 
        /// <summary>
        /// Generte C# code for a given  Entity using FluentCsTextTemplate
        /// </summary>
        /// <returns></returns>
        public   static string DicToString(this Dictionary<string,string> header)
        {
            StringBuilder builder = new StringBuilder();
            foreach (var item in header)
            {
                builder.Append(item.Key).Append(": ").Append(item.Value).AppendLine();
            }
            string result = builder.ToString();
            return result;
        }
    }
}
 
