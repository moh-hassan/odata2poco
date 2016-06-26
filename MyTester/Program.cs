using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Versioning;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using ODataDemo;
using Test;

namespace MyTester
{
    public class Program
    {
        public static void Main(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            //Program2.Main1();
            //  Test2();

            //fantastic, generate json schema in runtime
            Class5 c5 = new Class5();
            c5.Main1();

            //TestClass.Test1();
            //  ILGenClientApp.Main1();
           
            //working
            //Class9.Main1();
            //Class9.Test2();

            //working
            var c= new Class9a();
            //c.Main1();
            //c.Test2("MyDynamicAssembly.dll");

            //DemoAssemblyBuilder.Main1();
            //c.Test2("DynamicAssemblyExample.dll");

            //TestILGenerator.Main1();
            //c.Test2("Vector.dll");
            Console.ReadKey();


        }



        public static void Test1()
        {
            //AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            Console.WriteLine(IsUserAdministrator());
            Class1 c = new Class1();
            var text = c.GenerateSchemaForClass(typeof(Product));
            Console.WriteLine(text);
            var fname = @"product.json";
            //c.CreateFolder(fname);
            File.WriteAllText(fname, text);
        }
        public static void Test2()
        {
            var values = new Dictionary<string, object>();
            values.Add("Title", "Hello World!");
            values.Add("Text", "My first post");
            values.Add("Tags", new[] { "hello", "world" });

            var post = new DynamicEntity(values);

            dynamic dynPost = post;
            //var text = dynPost.Text;


            Console.WriteLine(IsUserAdministrator());
            Class1 c = new Class1();
            var text = c.GenerateSchemaForClass(dynPost.GetType());
            Console.WriteLine(text);
            var fname = @"dynamic.json";
            //c.CreateFolder(fname);
            File.WriteAllText(fname, text);
        }
        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Console.WriteLine((e.ExceptionObject as Exception).Message);
            Console.ReadKey();
            Environment.Exit(-99);

        }

        static public bool IsUserAdministrator()
        {
            bool isAdmin;
            try
            {
                WindowsIdentity user = WindowsIdentity.GetCurrent();
                WindowsPrincipal principal = new WindowsPrincipal(user);
                isAdmin = principal.IsInRole(WindowsBuiltInRole.Administrator);
            }
            catch (UnauthorizedAccessException ex)
            {
                isAdmin = false;
            }
            catch (Exception ex)
            {
                isAdmin = false;
            }
            return isAdmin;
        }

    }
}
