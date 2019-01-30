using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace OData2Poco.Extension
{
    
    /// <summary>
    /// Utility for CamelCase/PascalCase Conversion
    /// </summary>
    public static class StringExtensions
    {

        /// <summary>
        /// onvert the string to Pascal case.
        /// see definition: https://en.wikipedia.org/wiki/PascalCase
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string ToPascalCase(this string text)
        {
            text = text.Trim();
            if (string.IsNullOrEmpty(text)) return text;
            if (text.Length < 2) return text.ToUpper();  //one char

            // Split the string into words.
            char[] delimiterChars = {' ', '-','_', '.'};
            string[] words = text.Split(delimiterChars, StringSplitOptions.RemoveEmptyEntries);

           
            string result = "";
            foreach (string word in words)
            {
                result += char.ToUpper(word[0]); //Convert First Char in word to Capita letter
                result += word.Substring(1); // Combine with the rest of word
            }
            return result;
        }
        /// <summary>
        /// Convert the string to camel case.
        /// see Definition  https://en.wikipedia.org/wiki/Camel_case
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string ToCamelCase(this string text)
        {
            text = ToPascalCase(text);
            return text.Substring(0, 1).ToLower() + text.Substring(1);
        }

        /// <summary>
        /// Convert string expression to CaseEnum enumeration
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static CaseEnum ToCaseEnum(this string  name )
        {
            var nameCase = name.ToLower().Substring(0, 3);
          switch (nameCase)
            {
                case "pas": return CaseEnum.Pas;
                case "cam": return CaseEnum.Camel;
                default: return CaseEnum.None;
            }
        }

        /// <summary>
        /// remove extra white spaces and keeping CRLF if needed
        /// </summary>
        /// <param name="text"></param>
        /// <param name="keepCrLf"></param>
        /// <returns></returns>
        public static string TrimAllSpace(this string text,bool keepCrLf=false)
        {
          
            var result = "";
            Regex trimmer = new Regex(@"\s+");
            if (!keepCrLf)
            {
                 result = trimmer.Replace(text.Trim(), " ");
                return result;
            }
            
            var lines = text.Split(new[]{'\n','\r'}, StringSplitOptions.RemoveEmptyEntries);
            foreach (var line in lines)
            {
                result += string.Format("{0}\n",trimmer.Replace(line.Trim(), " "));
            }
       
            return result;
        }
        /// <summary>
        /// Convert name string to C# Attribute
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string ToCsAttribute(this string text)
        {
            return "[" +text + "]";
        }
     
        public static string NewLine(this string text)
        {
            return text + Environment.NewLine;
        }
        /// <summary>
        /// Format xml 
        /// </summary>
        /// <param name="xml"></param>
        /// <returns></returns>
        public static string FormatXml(this string xml)
        {
            XDocument doc = XDocument.Parse(xml);
            return doc.ToString();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string ToJson(this object data)
        {
            var json = JsonConvert.SerializeObject(data, Formatting.Indented);
         return json;
        }

        /// <summary>
        /// Validate json string
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public static bool ValidateJson(this string json)
        {
            try
            {
                JToken.Parse(json);
                return true;
            }
            catch (JsonReaderException)
            {
                return false;
            }
        }

        /// <summary>
        /// Convert Json string to Typed object
        /// </summary>
        /// <param name="json"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T ToObject<T>(this string json)
        {
            var deserializedObject = JsonConvert.DeserializeObject<T>(json);
            return deserializedObject;

        }

        public static string Quote(this string text, char c = '"') => $"{c}{text}{c}";

        public static string UnQuote(this string text)
        {
            return text.Trim('\'').Trim('"');
        }
        public static string ToTitle(this string text)
        {
            if (text.Length <= 2) return text;
            text = text.ToPascalCase();
            StringBuilder builder = new StringBuilder();
            foreach (char c in text)
            {
                if (Char.IsUpper(c) && builder.Length > 0) builder.Append(' ');
                builder.Append(c);
            }
            return builder.ToString();
        }

        public static string ToSnakeCase(this string str)
        {
            var text = string.Concat(str.Select((x, i) => i > 0 && char.IsUpper(x) ? "_" + x.ToString() : x.ToString())).ToLower();
            text = text.Replace("__", "_");
            return text;
        }

        public static string Dump(this object o)
        {
            return ToJson(o);
        }
        public static string GetRegexPattern(this string text, string escapeChar = "()[]?")
        {
            var pattern = Regex.Replace(text.Trim(), @"\s+", @"\s*");
            if (!string.IsNullOrEmpty(escapeChar))
            {
                char[] chars = escapeChar.ToCharArray();
                foreach (char c in chars)
                {
                    pattern = pattern.Replace(c.ToString(), $"\\{c}");
                }
            }

            return pattern;
        }

        public static string RemoveEmptyLines(this string input)
        {
            return Regex.Replace(input, @"^\s+$[\r\n]*", string.Empty, RegexOptions.Multiline);
        }
    }
}
