using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Linq;
//using Newtonsoft.Json;
//using Formatting = System.Xml.Formatting;

//using Formatting = Newtonsoft.Json.Formatting;
//using Formatting = System.Xml.Formatting;

namespace OData2Poco
{
    public class Helper
    {
       
      public   static string PrettyXml(string xml)
        {
            XDocument doc = XDocument.Parse(xml);
            return doc.ToString();
        }

      public static string GetSafeFilename(string filename)
      {

          return string.Join("_", filename.Split(Path.GetInvalidFileNameChars()));

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

        public static string GetNameSpace(string metadataString)
        {
            if (string.IsNullOrEmpty(metadataString)) return "MyNameSpace";
            var reader = XmlReader.Create(new StringReader(metadataString));
            reader.MoveToContent();
            reader.ReadToFollowing("Schema");
            var schemaNamespace = reader.GetAttribute("Namespace");
            return schemaNamespace;
        }

        public static Dictionary<string, string> ObjectToDictionary(object value)
        {
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            if (value != null)
            {
                foreach (PropertyDescriptor descriptor in TypeDescriptor.GetProperties(value))
                {
                    if (descriptor != null)
                    {
                        object propValue = descriptor.GetValue(value);
                        if (propValue != null)
                            dictionary.Add(descriptor.Name, String.Format("{0}", propValue));
                    }
                }

            }
            return dictionary;
        }
        /// <summary>
        /// Dump object for debuging (first level)
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string Dump(object obj)
        {
            var dic = ObjectToDictionary(obj);
            StringBuilder sb = new StringBuilder();
            foreach (var kvp in dic)
            {
                //textBox3.Text += ("Key = {0}, Value = {1}", kvp.Key, kvp.Value);
                sb.AppendFormat("{0}= {1}\n", kvp.Key, kvp.Value);
            }
            return obj +" : " + sb;
        }
    }
}

/*
 //JavaScriptSerializer js = new JavaScriptSerializer();
            //string json = js.Serialize(o2p.ClassList);
            //Console.WriteLine(json);

            Console.Write(new JSonPresentationFormatter().Format(o2p.ClassList));
*/
