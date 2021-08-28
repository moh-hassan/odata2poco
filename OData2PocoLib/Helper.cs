using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;
using OData2Poco.Extensions;

namespace OData2Poco
{
    /// <summary>
    /// Static Helper Functions
    /// </summary>
    public class Helper
    {
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
            {"Guid","?" },
            {"Microsoft.OData.Edm.Date","?"},
            {"Microsoft.OData.Edm.TimeOfDay","?"},
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
        public static string? GetNameSpace(string metadataString)
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
            bool isEqual = string.Equals(fixedStringOne, fixedStringTwo, StringComparison.OrdinalIgnoreCase);
            return isEqual;
        }

        public static string GetEmbeddedResource(string ns, string res)
        {
            using var reader = new StreamReader(Assembly
                                .GetExecutingAssembly()
                                .GetManifestResourceStream($"{ns}.{res}") 
                                     ?? throw new InvalidOperationException());
            return reader.ReadToEnd();
        }
        /// <summary>
        ///  find plugin dlls at runtime
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="foldr"></param>
        /// <returns></returns>
        public static List<object> LoadPlugin<T>(string foldr)
        {
            if (!Directory.Exists(foldr))
                return Enumerable.Empty<object>().ToList();
           
            string[] dlls = Directory.GetFiles(foldr, "*.dll");
            var list = new List<object>();
            //loop through the found dlls and load them
            foreach (string dll in dlls)
            {
                try
                {
                    Assembly plugin = Assembly.LoadFile(dll);
                    //find the classes that implement the interface T and get an object of that type
                    var instances = from t in plugin.GetTypes()
                                    where t.GetInterfaces().Contains(typeof(T))
                                          && t.GetConstructor(Type.EmptyTypes) != null //constructor w/o param
                                    select Activator.CreateInstance(t);
                    list.AddRange(instances);
                }
                catch 
                {
                    // ignored, bad plugin dll
                }
            }
            return list;
        }

    }
}


