#if x
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using PropertyAttributes = System.Data.PropertyAttributes;
//https://mhusseini.wordpress.com/2014/07/21/reflection-emit-creating-properties/
namespace MyTester
{
    class Class8
    {
        public void Test1()
        {
            // Create a Type Builder that generates a type directly into the current AppDomain.
            var appDomain = AppDomain.CurrentDomain;
            var assemblyName = new AssemblyName("MyDynamicAssembly");
            var assemblyBuilder = appDomain.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.RunAndSave);
            var moduleBuilder = assemblyBuilder.DefineDynamicModule(assemblyName.Name);

            var typeBuilder = moduleBuilder.DefineType("MyDynamicType", TypeAttributes.Class | TypeAttributes.Public);

            var propertyName = "Name";

            // Generate a property called "Name"

            var propertyType = typeof(string);
            var field = typeBuilder.DefineField("_" + propertyName, propertyType, FieldAttributes.Private);
            var propertyBuilder = typeBuilder.DefineProperty(propertyName, PropertyAttributes.HasDefault, propertyType, null);

            // Generate getter method

            var getter = typeBuilder.DefineMethod("get_" + name, GetSetAttr, propertyType, Type.EmptyTypes);

            var il = getter.GetILGenerator();

            il.Emit(OpCodes.Ldarg_0);        // Push "this" on the stack
            il.Emit(OpCodes.Ldfld, field);   // Load the field "_Name"
            il.Emit(OpCodes.Ret);            // Return

            propertyBuilder.SetGetMethod(getter);

            // Generate setter method

            var setter = typeBuilder.DefineMethod("set_" + name, GetSetAttr, null, new[] { propertyType });

            il = setter.GetILGenerator();

            il.Emit(OpCodes.Ldarg_0);        // Push "this" on the stack
            il.Emit(OpCodes.Ldarg_1);        // Push "value" on the stack
            il.Emit(OpCodes.Stfld, field);   // Set the field "_Name" to "value"
            il.Emit(OpCodes.Ret);            // Return

            propertyBuilder.SetSetMethod(setter);
        }
    }
}

#endif