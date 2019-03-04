using System.Collections.Generic;
using System.Text.RegularExpressions;
using OData2Poco.Extensions;
using OData2Poco.InfraStructure.Logging;

namespace OData2Poco
{
    /// <summary>
    ///   handle reserved cs keywords
    /// </summary>
    internal class ModelManager
    {
        private static readonly ColoredConsole Logger = PocoLogger.Default;
        private static Dictionary<string, string> ClassChangedName { get; }

        static ModelManager()
        {
            ClassChangedName = new Dictionary<string, string>();
        }
        public static void RenameeClasses(List<ClassTemplate> list)
        {
            list.Update(c => c.Name = RenameClass(c.Name));
            list.ForEach(classTemplate => classTemplate.Properties.Update(RenameProperty));
            ModifyPropertiesType(list);
        }

        public static string RenameClass(string className)
        {
            if (!className.IsCSharpReservedWord()) return className;
            var newClassName = className.ToggleFirstLetter();
            Logger.Normal($"The class: '{className}' is a reserved keyword. It's renamed to '{newClassName}'");
            ClassChangedName.Add(className, newClassName);
            return newClassName;
        }

        public static void ModifyPropertiesType(List<ClassTemplate> list) =>
            list.ForEach(ct => ct.Properties.Update(c => c.PropType = ModifyPropertyType(c)));


        private static string ModifyPropertyType(PropertyTemplate prop)
        {
            var type = prop.PropType;
            const string pattern = "List\\<(\\w+)\\>";
            Match m = Regex.Match(type, pattern);
            string newType;
            if (m.Success)
            {
                var name = m.Groups[1].ToString();
                if (!ClassChangedName.ContainsKey(name)) return type;
                newType = $"List<{ClassChangedName[name]}>";
                Logger.Normal($"++ Modify the type of the property: '{prop.ClassName}.{prop.PropName}' from  {type} to {newType}");
                return newType;
            }

            if (!ClassChangedName.ContainsKey(type)) return type;

            newType = ClassChangedName[type];
           Logger.Normal($"++ Modify the type of the property: '{prop.ClassName}.{prop.PropName}' from  {type} to {newType}");
            return newType;
        }
        public static PropertyTemplate RenameProperty(PropertyTemplate property)
        {
            string newName;
            if (property.PropName == property.ClassName)
            {
                //issue12, property name is the same as class name
                //error CS0542: '<PropName>': member names cannot be the same as their enclosing type
                newName = property.PropName.ToggleFirstLetter();
               Logger.Normal($"Rename the property '{property.ClassName}.{property.PropName}' to '{newName}' for  avoiding the Compiler error CS0542 ");
                property.PropName = newName;

                property.PropComment += "//Renamed";
            }
            //check if property is reserved keyword
            if (!property.PropName.IsCSharpReservedWord()) return property;

            newName = property.PropName.ToggleFirstLetter();
         Logger.Normal($"Rename the property {property.ClassName}.{property.PropName} to '{newName}' becauuse its name is a reserved keyword");
            property.PropName = newName;
            return property;
        }
    }
}
