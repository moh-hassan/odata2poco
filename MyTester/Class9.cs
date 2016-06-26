//#working , build type, generate source code
using System;
using System.IO;
using System.Threading;
using System.Reflection;
using System.Reflection.Emit;
using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.Ast;
using Mono.Cecil;
using FieldAttributes = System.Reflection.FieldAttributes;
using MethodAttributes = System.Reflection.MethodAttributes;
using PropertyAttributes = System.Reflection.PropertyAttributes;
using TypeAttributes = System.Reflection.TypeAttributes;

//https://msdn.microsoft.com/en-us/library/2sd82fz7%28v=vs.110%29.aspx
class Class9 //PropertyBuilderDemo
{
    private const string AssemplyName = "MyDynamicAssembly";
    private const string TypeName = "CustomerData";
    private const string FieldName = "customerName";
    private const string PropertyName = "CustomerName";
    public static Type BuildDynamicTypeWithProperties()
    {
        AppDomain myDomain = Thread.GetDomain();
        AssemblyName myAsmName = new AssemblyName();
        myAsmName.Name = AssemplyName;

        // To generate a persistable assembly, specify AssemblyBuilderAccess.RunAndSave.
        AssemblyBuilder myAsmBuilder = myDomain.DefineDynamicAssembly(myAsmName,
                                                        AssemblyBuilderAccess.RunAndSave);
        // Generate a persistable single-module assembly.
        ModuleBuilder myModBuilder =
            myAsmBuilder.DefineDynamicModule(myAsmName.Name, myAsmName.Name + ".dll");

        TypeBuilder myTypeBuilder = myModBuilder.DefineType(TypeName, TypeAttributes.Public);

        FieldBuilder customerNameBldr = myTypeBuilder.DefineField(FieldName,
                                                        typeof(string),
                                                        FieldAttributes.Private);

        // The last argument of DefineProperty is null, because the
        // property has no parameters. (If you don't specify null, you must
        // specify an array of Type objects. For a parameterless property,
        // use an array with no elements: new Type[] {})
        PropertyBuilder custNamePropBldr = myTypeBuilder.DefineProperty(PropertyName,
                                                         PropertyAttributes.HasDefault,
                                                         typeof(string),
                                                         null);

        // The property set and property get methods require a special
        // set of attributes.
        MethodAttributes getSetAttr =
            MethodAttributes.Public | MethodAttributes.SpecialName |
                MethodAttributes.HideBySig;

        // Define the "get" accessor method for CustomerName.
        var getAccessor = "get_" + PropertyName;
        MethodBuilder custNameGetPropMthdBldr =
            myTypeBuilder.DefineMethod(getAccessor,
                                       getSetAttr,
                                       typeof(string),
                                       Type.EmptyTypes);

        ILGenerator custNameGetIL = custNameGetPropMthdBldr.GetILGenerator();

        custNameGetIL.Emit(OpCodes.Ldarg_0);
        custNameGetIL.Emit(OpCodes.Ldfld, customerNameBldr);
        custNameGetIL.Emit(OpCodes.Ret);

        // Define the "set" accessor method for CustomerName.
        var setAccessor = "set_" + PropertyName;
        MethodBuilder custNameSetPropMthdBldr =
            myTypeBuilder.DefineMethod(setAccessor,
                                       getSetAttr,
                                       null,
                                       new Type[] { typeof(string) });

        ILGenerator custNameSetIL = custNameSetPropMthdBldr.GetILGenerator();

        custNameSetIL.Emit(OpCodes.Ldarg_0);
        custNameSetIL.Emit(OpCodes.Ldarg_1);
        custNameSetIL.Emit(OpCodes.Stfld, customerNameBldr);
        custNameSetIL.Emit(OpCodes.Ret);

        // Last, we must map the two methods created above to our PropertyBuilder to 
        // their corresponding behaviors, "get" and "set" respectively. 
        custNamePropBldr.SetGetMethod(custNameGetPropMthdBldr);
        custNamePropBldr.SetSetMethod(custNameSetPropMthdBldr);


        Type retval = myTypeBuilder.CreateType();

        // Save the assembly so it can be examined with Ildasm.exe,
        // or referenced by a test program.
        myAsmBuilder.Save(myAsmName.Name + ".dll");
        return retval;
    }

    public static void Main1()
    {
        Type custDataType = BuildDynamicTypeWithProperties();

        PropertyInfo[] custDataPropInfo = custDataType.GetProperties();
        foreach (PropertyInfo pInfo in custDataPropInfo)
        {
            Console.WriteLine("Property '{0}' created!", pInfo.ToString());
        }

        Console.WriteLine("---");
        // Note that when invoking a property, you need to use the proper BindingFlags -
        // BindingFlags.SetProperty when you invoke the "set" behavior, and 
        // BindingFlags.GetProperty when you invoke the "get" behavior. Also note that
        // we invoke them based on the name we gave the property, as expected, and not
        // the name of the methods we bound to the specific property behaviors.

        object custData = Activator.CreateInstance(custDataType);
        custDataType.InvokeMember(PropertyName, BindingFlags.SetProperty,
                                      null, custData, new object[] { "Joe User" });

        Console.WriteLine("The customerName field of instance custData has been set to '{0}'.",
                           custDataType.InvokeMember(PropertyName, BindingFlags.GetProperty,
                                                      null, custData, new object[] { }));
    }

    //http://stackoverflow.com/questions/9811448/icsharpcode-decompiler-mono-cecil-how-to-generate-code-for-a-single-method
    //Code to generate code for all the types inside an assembly:
    public static void Test2()
    {
        //DirectoryInfo di = new DirectoryInfo(appPath);
        //FileInfo[] allAssemblies = di.GetFiles("*.dll");
        //foreach (var assemblyFile in allAssemblies)
        {
            var assemblyFile = "MyDynamicAssembly.dll";
            string pathToAssembly = assemblyFile; //assemblyFile.FullName;
            //  System.Reflection.Assembly assembly = System.Reflection.Assembly.ReflectionOnlyLoadFrom(pathToAssembly);
            ReaderParameters parameters= new ReaderParameters();
            Mono.Cecil.AssemblyDefinition assemblyDefinition = Mono.Cecil.AssemblyDefinition.ReadAssembly(pathToAssembly, parameters);
            //Mono.Cecil.AssemblyDefinition assemblyDefinition = Mono.Cecil.AssemblyDefinition.ReadAssembly(pathToAssembly);
            AstBuilder astBuilder = null;

            foreach (var typeInAssembly in assemblyDefinition.MainModule.Types)
            {
                if (typeInAssembly.IsPublic)
                {
                    Console.WriteLine("T:{0}", typeInAssembly.Name);
                    //just reset the builder to include only code for a single type
                    astBuilder = new AstBuilder(new ICSharpCode.Decompiler.DecompilerContext(assemblyDefinition.MainModule));
                    //  astBuilder = new AstBuilder(new ICSharpCode.Decompiler.DecompilerContext(assemblyDefinition.MainModule) { CurrentType = typeInAssembly });
                    astBuilder.AddType(typeInAssembly);
                    StringWriter output = new StringWriter();
                    astBuilder.GenerateCode(new PlainTextOutput(output));
                    string result = output.ToString();
                    Console.WriteLine(result);
                    output.Dispose();
                }
            }
        }
    }

//    public static void Test3()
//    {
//        //Code to generate code for all the public methods inside an assembly:

//        string appPath = ".";
//        DirectoryInfo di = new DirectoryInfo(appPath);
//        FileInfo[] allAssemblies = di.GetFiles("*.dll");
//        foreach (var assemblyFile in allAssemblies)
//        {
//            string pathToAssembly = assemblyFile.FullName;
//            System.Reflection.Assembly assembly = System.Reflection.Assembly.ReflectionOnlyLoadFrom(pathToAssembly);
//            ReaderParameters parameters =null;
//            Mono.Cecil.AssemblyDefinition assemblyDefinition = Mono.Cecil.AssemblyDefinition.ReadAssembly(pathToAssembly, parameters);
//            AstBuilder astBuilder = null;

//            foreach (var typeInAssembly in assemblyDefinition.MainModule.Types)
//            {
//                if (typeInAssembly.IsPublic)
//                {
//                    Console.WriteLine("T:{0}", typeInAssembly.Name);
//                    foreach (var method in typeInAssembly.Methods)
//                    {
//                        //just reset the builder to include only code for a single method
//                        astBuilder = new AstBuilder(new ICSharpCode.Decompiler.DecompilerContext(assemblyDefinition.MainModule));
//                        astBuilder.AddMethod(method);
//                        if (method.IsPublic && !method.IsGetter && !method.IsSetter && !method.IsConstructor)
//                        {
//                            Console.WriteLine("M:{0}", method.Name);
//                            StringWriter output = new StringWriter();
//                            astBuilder.GenerateCode(new PlainTextOutput(output));
//                            string result = output.ToString();
//                            output.Dispose();
//                        }
//                    }
//                }
//            }
//        }
//    }
////When I recently implemented a quick C# decompiler (MonoDecompiler based), I used the ILSpy methods :)

//public string GetSourceCode(MethodDefinition methodDefinition)
//{
//    try
//    {
//        var csharpLanguage = new CSharpLanguage();
//        var textOutput = new PlainTextOutput();
//        var decompilationOptions = new DecompilationOptions();
//        decompilationOptions.FullDecompilation = true;
//        csharpLanguage.DecompileMethod(methodDefinition, textOutput, decompilationOptions);
//        return textOutput.ToString();              

//    }
//    catch (Exception exception)
//    {
//        PublicDI.log.error("in getSourceCode: {0}", new object[] { exception.Message });
//        return ("Error in creating source code from IL: " + exception.Message);
//    }
//}


}

// --- O U T P U T ---
// The output should be as follows:
// -------------------
// Property 'System.String CustomerName [System.String]' created!
// ---
// The customerName field of instance custData has been set to 'Joe User'.
// -------------------