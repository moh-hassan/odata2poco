using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using OData2Poco.Extension;

namespace OData2Poco
{
    /// <summary>
    /// Static Helper Functions
    /// </summary>
    public class Helper
    {
        //for test as accepting null
        //private TimeSpan? ts;
        //private DateTime? dt;
        //private DateTimeOffset? dto;
        //private Guid? guid;


        /// <summary>
        /// Nullable DataType
        /// </summary>
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
            {"void" , "?"},
            ////Nullable DateTime issue #3 , included in v2.3.0
            {"DateTime","?" },   
            {"DateTimeOffset","?" },
            {"TimeSpan","?" },
            {"Guid","?" }
        };

        /// <summary>
        /// Get nullable symbol ? for the registerd DataType
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string GetNullable(string name)
        {
            if (NullableDataTypes.ContainsKey(name))
                return NullableDataTypes[name];
            return "";
        }

        /// <summary>
        /// Get MetaData Version from XML 
        /// </summary>
        /// <param name="metadataString">XML MetaData</param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        /// <exception cref="XmlException"></exception>
        public static string GetMetadataVersion(string metadataString)
        {
            if (string.IsNullOrEmpty(metadataString)) throw new Exception("Metadata is not available");

            var reader = XmlReader.Create(new StringReader(metadataString));
            reader.MoveToContent();
            var version = reader.GetAttribute("Version");//If the attribute is not found or the value is String.Empty, null is returned.
            if (version != null) return version;
            throw new XmlException("No Version Attribute in XML  MetaData");

        }

        /// <summary>
        /// Get OData Service  Version from Http Header
        /// </summary>
        /// <param name="header"></param>
        /// <returns></returns>
        public static string GetServiceVersion(Dictionary<string, string> header)
        {

            foreach (var entry in header)
            {
                if (entry.Key.Contains("OData-Version") || entry.Key.Contains("DataServiceVersion"))
                    return entry.Value;
            }
            return "";
        }

        /// <summary>
        /// Get NameSpace from XML
        /// </summary>
        /// <param name="metadataString">XML MetaData</param>
        /// <returns></returns>
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

    }
}


