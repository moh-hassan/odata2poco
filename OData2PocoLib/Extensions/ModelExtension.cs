using OData2Poco.Extensions;
using OData2Poco.graph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace OData2Poco
{
    public static class ModelExtension
    {
        //public static IEnumerable<ClassTemplate> FindParents(this ClassTemplate? ct,
        //    IEnumerable<ClassTemplate> list)
        //{
        //    List<ClassTemplate> result = new();
        //    if (ct is null)
        //        return result;
        //    var ct2 = ct;
        //    while (true)
        //    {
        //        // var found = list.Find(x => x.FullName == ct2.BaseType);
        //        var found = list.FirstOrDefault(x => x.FullName == ct2.BaseType);
        //        if (found == null)
        //            break;
        //        result.Add(found);
        //        ct2 = found;
        //    }
        //    return result;
        //}
        //public static IEnumerable<ClassTemplate> GetDependency(this ClassTemplate ct,
        //    IEnumerable<ClassTemplate> source)
        //{
        //    var dependency = new HashSet<ClassTemplate>();
        //    //1) property dependency             
        //    foreach (var property in ct.Properties)
        //    {                
        //        var type = property.PropType.GetGenericBaseType();                 
        //        var found = type.ToClassTemplate(source);
        //        if (found != null && found.FullName != ct.FullName)  
        //            dependency.Add(found);
        //    }
        //    //2) base type dependency            
        //    var baseType = ct.BaseType.ToClassTemplate(source);            
        //    if (baseType != null)
        //        dependency.Add(baseType);
            
        //    return dependency.Distinct();
        //}
        internal static string GetGenericBaseType(this string type)
        {
            var pattern = "^List<(.+)>$";
            var match = Regex.Match(type, pattern);
            return match.Success ? match.Groups[1].Value : type;
        }
        internal static ClassTemplate? BaseTypeToClassTemplate(this ClassTemplate ct, IEnumerable<ClassTemplate> model) => 
            ct.BaseType.ToClassTemplate(model);
        internal static ClassTemplate? ToClassTemplate(this string type, IEnumerable<ClassTemplate> list) => 
            list.FirstOrDefault(a => a.FullName == type);

        public static StringBuilder GetImports(this ClassTemplate ct, IEnumerable<ClassTemplate> model, PocoSetting setting)
        {
            var imports = new StringBuilder();
            var allDeps = Dependency.Search(model, ct);
            foreach (var item in allDeps)
            {
                var fileName = item.GlobalName(setting);
                imports.AppendLine($"import {{{item.GlobalName(setting)}}} from './{fileName}';");
            }
            imports.AppendLine();
            return imports;
        }

        //Use FullName or name according to PocoSetting.UseFullName
        public static string GlobalName(this ClassTemplate ct, PocoSetting setting)
        {
            if (setting.UseFullName)
                return ct.FullName.RemoveDot();
            return ct.Name;
        }        
        internal static string TailComment(this PropertyTemplate property)
        {
            List<string> comments = new List<string>();
            if (!property.IsNullable)
                comments.Add("Not null");
            if (property.IsKey)
                comments.Add("Primary key");
            if (property.IsReadOnly)
                comments.Add("ReadOnly");
            if (property.IsNavigate)
                comments.Add("navigator");

            if (comments.Any())
                return (" //" + string.Join(", ", comments));

            return string.Empty;

        }
    }
}
