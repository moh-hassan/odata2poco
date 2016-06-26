using System;
using System.Reflection;
using System.Reflection.Emit;
using MyTester;
using ODataDemo;

//http://www.brainbell.com/tutors/C_Sharp/Creating_and_Executing_Code_at_Run_Time.htm
namespace MyTester
{
    public class CodeGenerator
    {
        public CodeGenerator()
        {
            // Get current currentDomain.
            currentDomain = AppDomain.CurrentDomain;
            // Create assembly in current currentDomain.
            assemblyName = new AssemblyName();
            assemblyName.Name = "TempAssembly";
            assemblyBuilder =
                    currentDomain.DefineDynamicAssembly
                    (assemblyName, AssemblyBuilderAccess.Run);
            // create a module in the assembly
            moduleBuilder = assemblyBuilder.DefineDynamicModule
                                ("TempModule");
            // create a type in the module
            typeBuilder = moduleBuilder.DefineType
                              ("TempClass",
                              TypeAttributes.Public);
            // add a member (a method) to the type
            methodBuilder = typeBuilder.DefineMethod
                                 ("HelloWorld",
                                  MethodAttributes.Public,
                                  null, null);
            
            // Generate MSIL.
            msil = methodBuilder.GetILGenerator();
            msil.EmitWriteLine("Hello World");
            msil.Emit(OpCodes.Ret);
            // Last "build" step : create type.
            t = typeBuilder.CreateType();
        }
        AppDomain currentDomain;
        AssemblyName assemblyName;
        AssemblyBuilder assemblyBuilder;
        ModuleBuilder moduleBuilder;
        TypeBuilder typeBuilder;
        MethodBuilder methodBuilder;
        ILGenerator msil;
        object o;
        Type t;
        public Type T
        {
            get
            {
                return this.t;
            }
        }
    }
}

//-----------
//using System;
//using System.Reflection;
//using ILGenServer;
public class ILGenClientApp
{
    public static void Main1()
    {
        Console.WriteLine("Calling DLL function to generate " +
                          "a new type and method in memory...");
        CodeGenerator gen = new CodeGenerator();
        Console.WriteLine("Retrieving dynamically generated type...");
        Type t = gen.T;

        Class1 c = new Class1();
       // var text = c.GenerateSchemaForClass(typeof(Product));
        var text = c.GenerateSchemaForClass(t.GetType());
        Console.WriteLine(text);
        if (null != t)
        {
            Console.WriteLine("Instantiating the new type...");
            object o = Activator.CreateInstance(t);
            Console.WriteLine("Retrieving the type's " +
                              "HelloWorld method...");
            MethodInfo helloWorld = t.GetMethod("HelloWorld");
            if (null != helloWorld)
            {
                Console.WriteLine("Invoking our dynamically " +
                                  "created HelloWorld method...");
                                  helloWorld.Invoke(o, null);
            }
            else
            {
                Console.WriteLine("Could not locate " +
                                  "HelloWorld method");
            }
        }
        else
        {
            Console.WriteLine("Could not access Type from server");
        }
    }
}

/*
Calling DLL function to generate a new type and method in memory...
Retrieving dynamically generated type...
{
  "title": "",
  "type": "object",
  "properties": {
    "ID": {
      "required": true,
      "type": "integer"
    },
    "Name": {
      "required": true,
      "type": [
        "string",
        "null"
      ]
    },
    "Description": {
      "required": true,
      "type": [
        "string",
        "null"
      ]
    },
    "ReleaseDate": {
      "required": true,
      "type": "string"
    },
    "DiscontinuedDate": {
      "required": true,
      "type": "string"
    },
    "Rating": {
      "required": true,
      "type": "integer"
    },
    "Price": {
      "required": true,
      "type": "number"
    }
  }
}
Instantiating the new type...
Retrieving the type's HelloWorld method...
Invoking our dynamically created HelloWorld method...
Hello World


*/
