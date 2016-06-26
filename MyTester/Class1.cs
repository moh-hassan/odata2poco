using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Schema;


namespace MyTester
{
    class Class1
    {
        /// <summary>
        /// Generates JSON schema for a given C# class using Newtonsoft.Json.Schema library.
        /// </summary>
        /// <param name="myType">class type</param>
        /// <returns>a string containing JSON schema for a given class type</returns>
        public string GenerateSchemaForClass(Type myType,string title="")
        {
            var jsonSchemaGenerator = new JsonSchemaGenerator();
          //  var myType = typeof(MyClass);
            var schema = jsonSchemaGenerator.Generate(myType);
            
            schema.Title =title?? myType.Name; // this doesn't seem to get done within the generator
            return schema.ToString();
        }

        public void CreateFolder(string path)
        {
            // Determine whether the directory exists.
            if (Directory.Exists(path))
            {
                Console.WriteLine("That path exists already.");
                return;
            }

            // Try to create the directory.
            DirectoryInfo di = Directory.CreateDirectory(path);
            Console.WriteLine("The directory was created successfully at {0}.",Directory.GetCreationTime(path));
        }
    }
}


/*
An unhandled exception of type 'Newtonsoft.Json.JsonException' occurred in Newtonsoft.Json.dll

Additional information: Unresolved circular reference for type 'ODataDemo.Product'. Explicitly define an Id for the type using a JsonObject/JsonArray attribute or automatically generate a type Id using the UndefinedSchemaIdHandling property.

*/
