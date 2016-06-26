 
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Reflection.Emit;
//http://stackoverflow.com/questions/3862226/dynamically-create-a-class-in-c-sharp
/*
 * @bugz use the code above to create the class, then in the base class you can add this method: 
 * public void SetValue<T>(string name, T value)
 * { GetType().GetProperty(name).SetValue(this, value); }  
 * */

    namespace TypeBuilderNamespace
    {

        public static class MyTypeBuilder
        {
            public static void CreateNewObject()
            {
                var myType = CompileResultType();
                var myObject = Activator.CreateInstance(myType);
            }
            public static Type CompileResultType()
            {
                TypeBuilder tb = GetTypeBuilder();
                ConstructorBuilder constructor = tb.DefineDefaultConstructor(MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName);

                // NOTE: assuming your list contains Field objects with fields FieldName(string) and FieldType(Type)
                List<FieldClass> yourListOfFields = new List<FieldClass>()  ;
                foreach (var field in yourListOfFields)
                    CreateProperty(tb, field.FieldName, field.FieldType);

                Type objectType = tb.CreateType();
                return objectType;
            }

            private static TypeBuilder GetTypeBuilder()
            {
                var typeSignature = "MyDynamicType";
                var an = new AssemblyName(typeSignature);
                AssemblyBuilder assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(an, AssemblyBuilderAccess.Run);
                ModuleBuilder moduleBuilder = assemblyBuilder.DefineDynamicModule("MainModule");
                TypeBuilder tb = moduleBuilder.DefineType(typeSignature
                                    , TypeAttributes.Public |
                                    TypeAttributes.Class |
                                    TypeAttributes.AutoClass |
                                    TypeAttributes.AnsiClass |
                                    TypeAttributes.BeforeFieldInit |
                                    TypeAttributes.AutoLayout
                                    , null);
                return tb;
            }

            private static void CreateProperty(TypeBuilder tb, string propertyName, Type propertyType)
            {
                FieldBuilder fieldBuilder = tb.DefineField("_" + propertyName, propertyType, FieldAttributes.Private);

                PropertyBuilder propertyBuilder = tb.DefineProperty(propertyName, PropertyAttributes.HasDefault, propertyType, null);
                MethodBuilder getPropMthdBldr = tb.DefineMethod("get_" + propertyName, MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig, propertyType, Type.EmptyTypes);
                ILGenerator getIl = getPropMthdBldr.GetILGenerator();

                getIl.Emit(OpCodes.Ldarg_0);
                getIl.Emit(OpCodes.Ldfld, fieldBuilder);
                getIl.Emit(OpCodes.Ret);

                MethodBuilder setPropMthdBldr =
                    tb.DefineMethod("set_" + propertyName,
                      MethodAttributes.Public |
                      MethodAttributes.SpecialName |
                      MethodAttributes.HideBySig,
                      null, new[] { propertyType });

                ILGenerator setIl = setPropMthdBldr.GetILGenerator();
                Label modifyProperty = setIl.DefineLabel();
                Label exitSet = setIl.DefineLabel();

                setIl.MarkLabel(modifyProperty);
                setIl.Emit(OpCodes.Ldarg_0);
                setIl.Emit(OpCodes.Ldarg_1);
                setIl.Emit(OpCodes.Stfld, fieldBuilder);

                setIl.Emit(OpCodes.Nop);
                setIl.MarkLabel(exitSet);
                setIl.Emit(OpCodes.Ret);

                propertyBuilder.SetGetMethod(getPropMthdBldr);
                propertyBuilder.SetSetMethod(setPropMthdBldr);
            }
        }

        public class FieldClass
        {
            public string FieldName { get; set; }
            public Type FieldType { get; set; }
        }
    }
 
