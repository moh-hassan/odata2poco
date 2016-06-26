using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CSharp;
using System.CodeDom.Compiler;
using System.CodeDom;
//http://programmers.stackexchange.com/questions/93322/generating-a-class-dynamically-from-types-that-are-fetched-at-runtime
namespace Test
{
  public   class Program2
    {
     public   static void Main1()
        {
            string className = "BlogPost";

            var props = new Dictionary<string, Type>() {
                { "Title", typeof(string) },
                { "Text", typeof(string) },
                { "Tags", typeof(string[]) }
            };

            createType(className, props);
        }

        static void createType(string name, IDictionary<string, Type> props)
        {
            var csc = new CSharpCodeProvider(new Dictionary<string, string>() { { "CompilerVersion", "v4.0" } });
            var parameters = new CompilerParameters(new[] { "mscorlib.dll", "System.Core.dll" }, "Test.Dynamic.dll", false);
            parameters.GenerateExecutable = false;

            var compileUnit = new CodeCompileUnit();
            var ns = new CodeNamespace("Test.Dynamic");
            compileUnit.Namespaces.Add(ns);
            ns.Imports.Add(new CodeNamespaceImport("System"));

            var classType = new CodeTypeDeclaration(name);
            classType.Attributes = MemberAttributes.Public;
            ns.Types.Add(classType);

            foreach (var prop in props)
            {
                var fieldName = "_" + prop.Key;
                var field = new CodeMemberField(prop.Value, fieldName);
                classType.Members.Add(field);

                var property = new CodeMemberProperty();
                property.Attributes = MemberAttributes.Public | MemberAttributes.Final;
                property.Type = new CodeTypeReference(prop.Value);
                property.Name = prop.Key;
                property.GetStatements.Add(new CodeMethodReturnStatement(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), fieldName)));
                property.SetStatements.Add(new CodeAssignStatement(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), fieldName), new CodePropertySetValueReferenceExpression()));
                classType.Members.Add(property);
            }

            var results = csc.CompileAssemblyFromDom(parameters, compileUnit);
            results.Errors.Cast<CompilerError>().ToList().ForEach(error => Console.WriteLine(error.ErrorText));
        }
    }
}