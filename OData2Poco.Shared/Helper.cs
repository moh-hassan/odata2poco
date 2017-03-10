//#define DEBUG
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OData2Poco.Shared;

namespace OData2Poco
{
    public class Helper
    {
        public static readonly Dictionary<string, string> NullableDataTypes = new Dictionary<string, string>
      {
            {"object" , ""}, 
            {"string" , ""}, 
            {"bool" , "?"}, 
            {"byte" , "?"}, 
            {"char" , "?"}, 
            {"decimal" , "?"}, 
            {"double" , "?"}, 
            {"short" , "?"}, 
            {"int" , "?"}, 
            {"long" , "?"}, 
            {"sbyte" , "?"}, 
            {"float" , "?"}, 
            {"ushort" , "?"}, 
            {"uint" , "?"}, 
            {"ulong" , "?"}, 
            {"void" , "?"}
        };
        //public static string GetNullable(Type type)
        //{
        //    if (!type.IsByRef) return ""; // ref-type
        //    if (Nullable.GetUnderlyingType(type) != null) return ""; // Nullable<T>
        //    return "?"; // value-type

        //}
        public static string GetNullable(string name)
        {
            if (NullableDataTypes.ContainsKey(name))
                return NullableDataTypes[name];
            return "";
        }

        public static string GetMetadataVersion(string metadataString)
        {
            if (string.IsNullOrEmpty(metadataString)) throw new Exception("Metadata is not available");

            var reader = XmlReader.Create(new StringReader(metadataString));
            reader.MoveToContent();
            var version = reader.GetAttribute("Version");//If the attribute is not found or the value is String.Empty, null is returned.
            if (version != null) return version;
            throw new XmlException("No Version Attribute in XML  MetaData");

        }

        public static string GetServiceVersion(Dictionary<string, string> header)
        {

            foreach (var entry in header)
            {
                if (entry.Key.Contains("OData-Version") || entry.Key.Contains("DataServiceVersion"))
                    return entry.Value;
            }
            return "";
        }

        public static string GetNameSpace(string metadataString)
        {
            if (string.IsNullOrEmpty(metadataString)) return "MyNameSpace";
            var reader = XmlReader.Create(new StringReader(metadataString));
            reader.MoveToContent();
            reader.ReadToFollowing("Schema");
            var schemaNamespace = reader.GetAttribute("Namespace");
            return schemaNamespace;
        }

        /// <summary>
        /// Compare strings ignoring spaces and newline Cr/LF
        /// </summary>
        /// <param name="text1"></param>
        /// <param name="text2"></param>
        /// <returns></returns>
        public static bool CompareStringIgnoringSpaceCr(string text1, string text2)
        {
            string fixedStringOne = text1.TrimAllSpace(); // Regex.Replace(text1.Trim(), @"\s+", " ");
            string fixedStringTwo = text2.TrimAllSpace(); // Regex.Replace(text2.Trim(), @"\s+", " ");
            //Debug.WriteLine("{0}\n{1}\n",fixedStringOne,fixedStringTwo);
            bool isEqual = String.Equals(fixedStringOne, fixedStringTwo, StringComparison.OrdinalIgnoreCase);
            return isEqual;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string ToJson( object data)
        {
            var json = JsonConvert.SerializeObject(data, Newtonsoft.Json.Formatting.Indented);
            //Console.WriteLine("-----{0}",json);
            return json;

        }

        /// <summary>
        /// Load json from fileName , then Extract class in json format 
        /// </summary>
        /// <param name="fname"></param>
        /// <param name="className"></param>
        public static  string  ExtractClassFromJson(string fname,string className)
        {
            var json = File.ReadAllText(fname);
            JObject googleSearch = JObject.Parse(json);
            //Console.WriteLine(googleSearch);
            // get JSON result objects into a list
            IList<JToken> results = googleSearch[className]["Properties"].Children().ToList();
            //Console.WriteLine(ToJson(results));
            return ToJson(results);
            
        }


        public static T ToObject<T>(string json)
        {
            var deserializedObject = JsonConvert.DeserializeObject<T>(json);
            return deserializedObject;

        }

        
        //public static string Xml2Json(string xml)
        //{
        //    XmlDocument doc = new XmlDocument();
        //    doc.LoadXml(xml);
        //    string jsonText = JsonConvert.SerializeXmlNode(doc,Formatting.Indented);
        //    return jsonText;
        //}
       
        //public static string Json2Xml(string json)
        //{
        //    XmlDocument doc = JsonConvert.DeserializeXmlNode(json);
        //    return doc.ToString();
        //}
 
    }
}


