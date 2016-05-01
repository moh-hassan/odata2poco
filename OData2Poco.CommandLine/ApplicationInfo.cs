using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace OData2Poco.CommandLine
{
    class ApplicationInfo
    {
        public static string Title = Utility.GetAssemblyAttribute<AssemblyTitleAttribute>(a => a.Title);
        public static string Author = "Mohamed Hassan";
        public static string Product
        {
            get
            {
                return Utility.GetAssemblyAttribute<AssemblyProductAttribute>(a => a.Product);
            }
        }

        public static string Copyright
        {
            get
            {
                return Utility.GetAssemblyAttribute<AssemblyCopyrightAttribute>(a => a.Copyright);
            }
        }

        public static string Version
        {
            get
            {
                return Utility.GetAssemblyAttribute<AssemblyInformationalVersionAttribute>(a => a.InformationalVersion);
            }
        }

        public static string Description
        {
            get
            {
                return Utility.GetAssemblyAttribute<AssemblyDescriptionAttribute>(a => a.Description);
            }
        }

        public static string HeadingInfo
        {
            get { return Product + " " + Version; }
        }

    }
}
