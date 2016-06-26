using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;


//using Newtonsoft.Json;
//using Formatting = System.Xml.Formatting;

//using Formatting = Newtonsoft.Json.Formatting;
//using Formatting = System.Xml.Formatting;

namespace OData2Poco
{
    public class Helper
    {
        private static readonly Dictionary<string, string> NullableDataTypes = new Dictionary<string, string>
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
        public static string GetNullable(Type type)
        {
            if (!type.IsByRef) return ""; // ref-type
            if (Nullable.GetUnderlyingType(type) != null) return ""; // Nullable<T>
            return "?"; // value-type

        }
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



    }
}


