using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using OData2Poco.Extensions;
using OData2Poco.InfraStructure.Logging;

namespace OData2Poco
{
    /// <summary>
    ///   Change EntityName case
    /// </summary>
    internal class ModelChangeCase
    {
        private Dictionary<string, string> ClassChangedName { get; }
        private ModelChangeCase()
        {
            ClassChangedName = new Dictionary<string, string>();
        }
        /// <summary>
        /// Change case of class with its parent and related Property Type
        /// </summary>
        /// <param name="list">Class list</param>
        /// <param name="caseEnum">caseEnum</param>
        /// <returns></returns>
        public static ModelChangeCase RenameClasses(List<ClassTemplate> list, CaseEnum caseEnum)
        {
          
            var cc = new ModelChangeCase();
            if (caseEnum == CaseEnum.None) return cc;

            //rename classes
            list.Update(c =>
             {
                 c.Name = cc.RenameClass(c.Name, caseEnum);
                 return c;
             });
            //rename Parent
            list.Update(c =>
            {
                c.BaseType = cc.RenameParent(c.BaseType);
                return c;
            });
            //rename properties type
            cc.RenamePropertiesType(list);
            return cc;
        }

      
        private void AddItem(string key, string value)
        {
            if (!ClassChangedName.ContainsKey(key))
            {
                ClassChangedName.Add(key, value);
            }
        }

        private string RenameClass(string className, CaseEnum caseEnum)
        {
            var newClassName = className.ChangeCase(caseEnum);
            AddItem(className, newClassName);
            return newClassName;
        }

        private string RenameParent(string className) => GetNewName(className);

        private void RenamePropertiesType(List<ClassTemplate> list) =>
            list.ForEach(ct => ct.Properties.Update(c => c.PropType = RenamePropertyType(c)));


        private string GetNewName(string name)
        {
            if (string.IsNullOrEmpty(name)) return "";
            if (ClassChangedName.ContainsKey(name)) return ClassChangedName[name];
            //check if name contains namespace
            var index = name.LastIndexOf('.');
            if (index < 0) return name;
            var entity = name.Substring(index + 1);
            var ns = name.Substring(0, index - 1);
            return ClassChangedName.ContainsKey(entity) ? ClassChangedName[entity] : name;
        }
        private string RenamePropertyType(PropertyTemplate prop)
        {
            var type = prop.PropType;
            const string pattern = @"List<([\w\.]+)>";
            var m = Regex.Match(type, pattern);
            if (m.Success)
            {
                var name = m.Groups[1].ToString();
                var newName = GetNewName(name);
                var newType = $"List<{newName}>";
                return newType;
            }

            return GetNewName(type);
        }
    }
}
