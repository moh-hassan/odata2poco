//#working , build type, generate source code
using System;
using System.IO;
using System.Threading;
using System.Reflection;
using System.Reflection.Emit;
using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.Ast;
using ICSharpCode.NRefactory;
using Mono.Cecil;
using FieldAttributes = System.Reflection.FieldAttributes;
using MethodAttributes = System.Reflection.MethodAttributes;
using PropertyAttributes = System.Reflection.PropertyAttributes;
using TypeAttributes = System.Reflection.TypeAttributes;

//https://msdn.microsoft.com/en-us/library/2sd82fz7%28v=vs.110%29.aspx
class Class9a //PropertyBuilderDemo
{
    private const string AssemplyName = "MyDynamicAssembly";
    private const string TypeName = "CustomerData";
    //private const string FieldName = "customerName";
    //private const string PropertyName = "CustomerName";
    private TypeBuilder myTypeBuilder;

    public void CreateProperty(string fieldName, string propertyName,Type type)
    {
        FieldBuilder customerNameBldr = myTypeBuilder.DefineField(fieldName,
                                                    //   typeof(string),
                                                    type,
                                                       FieldAttributes.Private);

        // The last argument of DefineProperty is null, because the
        // property has no parameters. (If you don't specify null, you must
        // specify an array of Type objects. For a parameterless property,
        // use an array with no elements: new Type[] {})
        PropertyBuilder custNamePropBldr = myTypeBuilder.DefineProperty(propertyName,
                                                         PropertyAttributes.HasDefault,
                                                        // typeof(string),
                                                        type,
                                                         null);

        // The property set and property get methods require a special
        // set of attributes.
        MethodAttributes getSetAttr =
            MethodAttributes.Public | MethodAttributes.SpecialName |
                MethodAttributes.HideBySig;

        // Define the "get" accessor method for CustomerName.
        var getAccessor = "get_" + propertyName;
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
        var setAccessor = "set_" + propertyName;
        MethodBuilder custNameSetPropMthdBldr =
            myTypeBuilder.DefineMethod(setAccessor,
                                       getSetAttr,
                                       null ,
                                    //   new[] { typeof(string) }
                                       new[] { type }
                                       );

        ILGenerator custNameSetIL = custNameSetPropMthdBldr.GetILGenerator();

        custNameSetIL.Emit(OpCodes.Ldarg_0);
        custNameSetIL.Emit(OpCodes.Ldarg_1);
        custNameSetIL.Emit(OpCodes.Stfld, customerNameBldr);
        custNameSetIL.Emit(OpCodes.Ret);

        // Last, we must map the two methods created above to our PropertyBuilder to 
        // their corresponding behaviors, "get" and "set" respectively. 
        custNamePropBldr.SetGetMethod(custNameGetPropMthdBldr);
        custNamePropBldr.SetSetMethod(custNameSetPropMthdBldr);
    }
    public   Type BuildDynamicTypeWithProperties()
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

      //  TypeBuilder myTypeBuilder = myModBuilder.DefineType(TypeName, TypeAttributes.Public);
            myTypeBuilder = myModBuilder.DefineType(TypeName, TypeAttributes.Public);

//--------------------
            CreateProperty("firstName", "FirstName",typeof(string));
            CreateProperty("lastName", "LastName", typeof(int));
            //BuildMethodget_ID(myTypeBuilder);
        //FieldBuilder customerNameBldr = myTypeBuilder.DefineField(FieldName,
        //                                                typeof(string),
        //                                                FieldAttributes.Private);

        //// The last argument of DefineProperty is null, because the
        //// property has no parameters. (If you don't specify null, you must
        //// specify an array of Type objects. For a parameterless property,
        //// use an array with no elements: new Type[] {})
        //PropertyBuilder custNamePropBldr = myTypeBuilder.DefineProperty(PropertyName,
        //                                                 PropertyAttributes.HasDefault,
        //                                                 typeof(string),
        //                                                 null);

        //// The property set and property get methods require a special
        //// set of attributes.
        //MethodAttributes getSetAttr =
        //    MethodAttributes.Public | MethodAttributes.SpecialName |
        //        MethodAttributes.HideBySig;

        //// Define the "get" accessor method for CustomerName.
        //var getAccessor = "get_" + PropertyName;
        //MethodBuilder custNameGetPropMthdBldr =
        //    myTypeBuilder.DefineMethod(getAccessor,
        //                               getSetAttr,
        //                               typeof(string),
        //                               Type.EmptyTypes);

        //ILGenerator custNameGetIL = custNameGetPropMthdBldr.GetILGenerator();

        //custNameGetIL.Emit(OpCodes.Ldarg_0);
        //custNameGetIL.Emit(OpCodes.Ldfld, customerNameBldr);
        //custNameGetIL.Emit(OpCodes.Ret);

        //// Define the "set" accessor method for CustomerName.
        //var setAccessor = "set_" + PropertyName;
        //MethodBuilder custNameSetPropMthdBldr =
        //    myTypeBuilder.DefineMethod(setAccessor,
        //                               getSetAttr,
        //                               null,
        //                               new Type[] { typeof(string) });

        //ILGenerator custNameSetIL = custNameSetPropMthdBldr.GetILGenerator();

        //custNameSetIL.Emit(OpCodes.Ldarg_0);
        //custNameSetIL.Emit(OpCodes.Ldarg_1);
        //custNameSetIL.Emit(OpCodes.Stfld, customerNameBldr);
        //custNameSetIL.Emit(OpCodes.Ret);

        //// Last, we must map the two methods created above to our PropertyBuilder to 
        //// their corresponding behaviors, "get" and "set" respectively. 
        //custNamePropBldr.SetGetMethod(custNameGetPropMthdBldr);
        //custNamePropBldr.SetSetMethod(custNameSetPropMthdBldr);

//-------------------------------------
        Type retval = myTypeBuilder.CreateType();

        // Save the assembly so it can be examined with Ildasm.exe,
        // or referenced by a test program.
        myAsmBuilder.Save(myAsmName.Name + ".dll");
        return retval;
    }

    public   void Main1()
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

        //object custData = Activator.CreateInstance(custDataType);
        //custDataType.InvokeMember(PropertyName, BindingFlags.SetProperty,
        //                              null, custData, new object[] { "Joe User" });

        //Console.WriteLine("The customerName field of instance custData has been set to '{0}'.",
        //                   custDataType.InvokeMember(PropertyName, BindingFlags.GetProperty,
        //                                              null, custData, new object[] { }));
    }

    //http://stackoverflow.com/questions/9811448/icsharpcode-decompiler-mono-cecil-how-to-generate-code-for-a-single-method
    //Code to generate code for all the types inside an assembly:
    public void Test2(string assemblyFile)
    {
        //DirectoryInfo di = new DirectoryInfo(appPath);
        //FileInfo[] allAssemblies = di.GetFiles("*.dll");
        //foreach (var assemblyFile in allAssemblies)
        {
           // var assemblyFile = "MyDynamicAssembly.dll";
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

    //Please see code below not working
        public FieldBuilder BuildField_id(TypeBuilder type)
{
FieldBuilder field = type.DefineField("_id",typeof(Int32),FieldAttributes.Private);
return field;
}
public MethodBuilder BuildMethodget_ID(TypeBuilder type)
{
// Declaring method builder
MethodBuilder method = type.DefineMethod("get_ID", MethodAttributes.Private);
// Preparing Reflection instances
FieldInfo field1 = typeof(Role).GetField("_id", BindingFlags.Public |
BindingFlags.NonPublic);
// Method attributes
//method.Attributes =
//System.Reflection.MethodAttributes.Public
//| System.Reflection.MethodAttributes.HideBySig;
// Setting return type
method.SetReturnType(typeof(Int32));
// Adding parameters
ILGenerator gen = method.GetILGenerator();
// Preparing locals
LocalBuilder CS  = gen.DeclareLocal(typeof(Int32));
// Preparing labels
Label label10 = gen.DefineLabel();
// Writing body
gen.Emit(OpCodes.Nop);
gen.Emit(OpCodes.Ldarg_0);
gen.Emit(OpCodes.Ldfld,field1);
gen.Emit(OpCodes.Stloc_0);
gen.Emit(OpCodes.Br_S,label10);
gen.MarkLabel(label10);
gen.Emit(OpCodes.Ldloc_0);
gen.Emit(OpCodes.Ret);
// finished
return method;
}


}

// --- O U T P U T ---
// The output should be as follows:
// -------------------
// Property 'System.String CustomerName [System.String]' created!
// ---
// The customerName field of instance custData has been set to 'Joe User'.
// -------------------

/*
https://www.simple-talk.com/dotnet/.net-framework/dynamically-generating--typed-objects-in-.net/
How does it work?

Here is what happens inside the extension method. First of all I generate dynamic assembly and I call it “TempAssembly” plus the hash code of the IEnumerable collection. I then define the type of the object will be public class.

After that, I get the first IDictionary in the IEnumerable to be used as a template for the newly generated statically typed object. First I check with a regular expression that each key is alphanumeric and starts with letter so it can be transformed into property name. This is the regular expression I have used:

 

Regex PropertNameRegex = new Regex(@"^[A-Za-z]+[A-Za-z0-9_]*$", RegexOptions.Singleline);

 

If the key does not follow this constraint, then I throw an application exception. Then for each key I generate a property. In .NET on a lower level getter property is transformed to get_<PropertyName>() method and setter property is transformed to set_<PropertyName>(value) method. So I generate both setter and getter and the private field lying underneath. The private field starts as usual with an underscore character. I get the type for the properties and private field from the current value in the IDictionary. If the value is NULL then the property is of type object.

Here is how the code generating properties looks:
 * 
FieldBuilder fieldBuilder = typeBuilder.DefineField("_" + propertyName,

                                            propertyType,

                                            FieldAttributes.Private);

 

 

PropertyBuilder propertyBuilder =

    typeBuilder.DefineProperty(

        propertyName, PropertyAttributes.HasDefault, propertyType, null);

MethodBuilder getPropMthdBldr =

    typeBuilder.DefineMethod("get_" + propertyName,

        MethodAttributes.Public |

        MethodAttributes.SpecialName |

        MethodAttributes.HideBySig,

        propertyType, Type.EmptyTypes);

 

ILGenerator getIL = getPropMthdBldr.GetILGenerator();

 

getIL.Emit(OpCodes.Ldarg_0);

getIL.Emit(OpCodes.Ldfld, fieldBuilder);

getIL.Emit(OpCodes.Ret);

 

MethodBuilder setPropMthdBldr =

    typeBuilder.DefineMethod("set_" + propertyName,

      MethodAttributes.Public |

      MethodAttributes.SpecialName |

      MethodAttributes.HideBySig,

      null, new Type[] { propertyType });

 

ILGenerator setIL = setPropMthdBldr.GetILGenerator();

 

setIL.Emit(OpCodes.Ldarg_0);

setIL.Emit(OpCodes.Ldarg_1);

setIL.Emit(OpCodes.Stfld, fieldBuilder);

setIL.Emit(OpCodes.Ret);

 

propertyBuilder.SetGetMethod(getPropMthdBldr);

propertyBuilder.SetSetMethod(setPropMthdBldr);
*/
