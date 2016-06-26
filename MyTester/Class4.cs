#if x
using System;
using System.Configuration;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Xml;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading;

public partial class Class3
{
    protected void Page_Load(object sender, EventArgs e)
    {
        object myObject = CreateOurNewObject();
        //DetailsView1.DataSource = new object[] { myObject };
        //DetailsView1.DataBind();
    }

    private object CreateOurNewObject()
    {
        string _xml = "<root>" +
            "<column name=\"Name\">Miron</column>" +
            "<column name=\"LastName\">Abramson</column>" +
            "<column name=\"Blog\">http://www.blog.mironabramson.com</column>" +
            "</root>";

        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.LoadXml(_xml);

        // create a dynamic assembly and module 
        AssemblyName assemblyName = new AssemblyName();
        assemblyName.Name = "tmpAssembly";
        AssemblyBuilder assemblyBuilder = Thread.GetDomain().DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
        ModuleBuilder module = assemblyBuilder.DefineDynamicModule("tmpModule");

        // create a new type builder
        TypeBuilder typeBuilder = module.DefineType("BindableRowCellCollection", TypeAttributes.Public | TypeAttributes.Class);

        // Loop over the attributes that will be used as the properties names in out new type
        foreach (XmlNode node in xmlDoc.SelectSingleNode("root").ChildNodes)
        {
            string propertyName = node.Attributes["name"].Value;

            // Generate a private field
            FieldBuilder field = typeBuilder.DefineField("_" + propertyName, typeof(string), FieldAttributes.Private);
            // Generate a public property
            PropertyBuilder property =
                typeBuilder.DefineProperty(propertyName,
                                 PropertyAttributes.None,
                                 typeof(string),
                                 new Type[] { typeof(string) });

            // The property set and property get methods require a special set of attributes:

            MethodAttributes GetSetAttr =
                MethodAttributes.Public |
                MethodAttributes.HideBySig;

            // Define the "get" accessor method for current private field.
            MethodBuilder currGetPropMthdBldr =
                typeBuilder.DefineMethod("get_value",
                                           GetSetAttr,
                                           typeof(string),
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
                                           new Type[] { typeof(string) });

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
        foreach (XmlNode node in xmlDoc.SelectSingleNode("root").ChildNodes)
        {
            string value = node.InnerText;
            properties[propertiesCounter].SetValue(generetedObject, value, null);
            propertiesCounter++;
        }

        //Yoopy ! Return our new genereted object.
        return generetedObject;
    }
}

#endif
