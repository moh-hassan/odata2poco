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
            list.Update(c =>
             {
                 c.Name = cc.RenameClass(c.Name, caseEnum);
                 c.BaseType = cc.RenameParent(c.BaseType);
                 return c;
             });
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
            if (caseEnum == CaseEnum.None) return className;
            var newClassName = className.ChangeCase(caseEnum);
            AddItem(className, newClassName);
            return newClassName;
        }

        private string RenameParent(string className) => GetNewName(className);

        private void RenamePropertiesType(List<ClassTemplate> list) =>
            list.ForEach(ct => ct.Properties.Update(c => c.PropType = RenamePropertyType(c)));


        private string GetNewName(string key)
        {
            if (string.IsNullOrEmpty(key)) return "";
            if (ClassChangedName.ContainsKey(key)) return ClassChangedName[key];
            //check if key contains namespace
            var index = key.LastIndexOf('.');
            if (index < 0) return key;
            var entity = key.Substring(index + 1);
            var ns = key.Substring(0, index - 1);
            return ClassChangedName.ContainsKey(entity) ? ClassChangedName[entity] : key;
        }
        private string RenamePropertyType(PropertyTemplate prop)
        {
            var type = prop.PropType;
            const string pattern = @"List<([\w\.]+)>";
            var m = Regex.Match(type, pattern);
            var newName = "";
            if (!m.Success) return GetNewName(type);
            var name = m.Groups[1].ToString();
            newName = GetNewName(name);
            var newType = $"List<{newName}>";
            return newType;
        }
    }
}
