using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading;
using MyTester;
using ODataDemo;

//http://mironabramson.com/blog/post/2008/06/Create-you-own-new-Type-and-use-it-on-run-time-%28C%29.aspx
public   class Class5
{
    private Dictionary<string, string> CustomerDic = new Dictionary<string, string>()
    {
        {"Name", "System.String"},
        {"LastName", "System.String"},
        {"Age", "System.Int32"}
    };

 

    public  void Main1()
    {
        object myObject = CreateOurNewObject(CustomerDic);

        Class1 c = new Class1();
        var text = c.GenerateSchemaForClass(myObject.GetType(),"Customer");
        Console.WriteLine(text);
        //var fname = @"product.json";
        ////c.CreateFolder(fname);
        //File.WriteAllText(fname, text);
        ////DetailsView1.DataSource = new object[] { myObject };
        ////DetailsView1.DataBind();

       // foreach (TypeCode t in Enum.GetValues(typeof(TypeCode)))
        // foreach (var t in GetBuiltInTypes())
            
        //{
        //    // do something interesting with the value...
        //    Console.WriteLine(" {0} {1}  {2} {3}", t, IsNullableType(t.GetType()), t.FullName , t.Name);
        //}
    }
   
    private object CreateOurNewObject(Dictionary<string, string> dic)
    {
        //string _xml = "<root>" +
        //    "<column name=\"Name\">Miron</column>" +
        //    "<column name=\"LastName\">Abramson</column>" +
        //    "<column name=\"Blog\">http://www.blog.mironabramson.com</column>" +
        //    "</root>";

        //XmlDocument xmlDoc = new XmlDocument();
        //xmlDoc.LoadXml(_xml);

        // create a dynamic assembly and module 
        AssemblyName assemblyName = new AssemblyName();
        assemblyName.Name = "tmpAssembly";
        AssemblyBuilder assemblyBuilder = Thread.GetDomain().DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
        ModuleBuilder module = assemblyBuilder.DefineDynamicModule("tmpModule");

        // create a new type builder
        TypeBuilder typeBuilder = module.DefineType("BindableRowCellCollection", TypeAttributes.Public | TypeAttributes.Class);

        // Loop over the attributes that will be used as the properties names in out new type
        foreach (var entry in dic)
        {
            string propertyName = entry.Key;
            string typeName = entry.Value;

            // Generate a private field
            FieldBuilder field = typeBuilder.DefineField("_" + propertyName, typeof(string), FieldAttributes.Private);
            // Generate a public property
            PropertyBuilder property =
                typeBuilder.DefineProperty(propertyName,
                                 PropertyAttributes.None,
                               //  typeof(string),
                               Type.GetType(typeName),
                                 new Type[] { typeof(string) });

            // The property set and property get methods require a special set of attributes:

            MethodAttributes GetSetAttr =
                MethodAttributes.Public |
                MethodAttributes.HideBySig;
         
            // Define the "get" accessor method for current private field.
            MethodBuilder currGetPropMthdBldr =
                typeBuilder.DefineMethod("get_value",
                                           GetSetAttr,
                                        //   typeof(string),
                                        Type.GetType(typeName),
                                           Type.EmptyTypes);

            // Intermediate Language stuff...
            ILGenerator currGetIL = currGetPropMthdBldr.GetILGenerator();
            currGetIL.Emit(OpCodes.Ldarg_0);
            currGetIL.Emit(OpCodes.Ldfld, field);
            currGetIL.Emit(OpCodes.Ret);

            // Define the "set" accessor method for current private field.
            MethodBuilder currSetPropMthdBldr =
                typeBuilder.DefineMethod("set_value",
                                           GetSetAttr,
                                           null,
                                           new Type[] { typeof(string) }
                                           //  new Type[] { Type.GetType(typeName) }
                                           );

            // Again some Intermediate Language stuff...
            ILGenerator currSetIL = currSetPropMthdBldr.GetILGenerator();
            currSetIL.Emit(OpCodes.Ldarg_0);
            currSetIL.Emit(OpCodes.Ldarg_1);
            currSetIL.Emit(OpCodes.Stfld, field);
            currSetIL.Emit(OpCodes.Ret);

            // Last, we must map the two methods created above to our PropertyBuilder to 
            // their corresponding behaviors, "get" and "set" respectively. 
            property.SetGetMethod(currGetPropMthdBldr);
            property.SetSetMethod(currSetPropMthdBldr);
        }

        // Generate our type
        Type generetedType = typeBuilder.CreateType();
         
        // Now we have our type. Let's create an instance from it:
        object generetedObject = Activator.CreateInstance(generetedType);

        // Loop over all the generated properties, and assign the values from our XML:
        PropertyInfo[] properties = generetedType.GetProperties();

        int propertiesCounter = 0;

        // Loop over the values that we will assign to the properties
        //foreach (XmlNode node in xmlDoc.SelectSingleNode("root").ChildNodes)
        //{
        //    string value = node.InnerText;
        //    properties[propertiesCounter].SetValue(generetedObject, value, null);
        //    propertiesCounter++;
        //}
        
        //Yoopy ! Return our new genereted object.
        return generetedObject;
    }
}

/*
 *   //bool isPrimitive = typeName.GetType().IsPrimitive; // False
            //Console.WriteLine(" {0} {1} ",typeName, IsNullableType(typeName.GetType()));
            //if (Nullable.GetUnderlyingType(typeName.GetType()) != null)
            //{
            //    Console.WriteLine(" {0} {1}", typeName, "It's nullable");
            //}
            //else
            //{
            //    Console.WriteLine(" {0} {1}", typeName, "IS NOT  nullable");
            //}
 * 
 *  //https://msdn.microsoft.com/en-us/library/ya5y69ds.aspx

    public   bool IsNullableType(Type type)
    {
        if (!type.IsGenericType)
        {
            return false;
        }
        if (type.GetGenericTypeDefinition() == typeof(Nullable<>))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public IEnumerable<Type> GetBuiltInTypes()
    {
        return typeof(int).Assembly
                          .GetTypes()
                          .Where(t => t.IsPrimitive);
    }
 * */

/*
The following table shows the keywords for built-in C# types, which are aliases of predefined types in the System namespace.

C# Type
	

.NET Framework Type

bool
	

System.Boolean

byte
	

System.Byte

sbyte
	

System.SByte

char
	

System.Char

decimal
	

System.Decimal

double
	

System.Double

float
	

System.Single

int
	

System.Int32

uint
	

System.UInt32

long
	

System.Int64

ulong
	

System.UInt64

object
	

System.Object

short
	

System.Int16

ushort
	

System.UInt16

string
	

System.String
Remarks

All of the types in the table, except object and string, are referred to as simple types.
*/
