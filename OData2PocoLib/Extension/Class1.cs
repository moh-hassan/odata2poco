using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace OData2PocoLib.Extension
{
    class Class1
    {
         
    static string PrettyXml(string xml)
    {
        XDocument doc = XDocument.Parse(xml);
        return  doc.ToString();
    }


    }
}
