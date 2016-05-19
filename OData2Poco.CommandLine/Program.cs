using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using CommandLine;

//(c) 2016 Mohamed Hassan
// MIT License
//project site: http://odata2poco.codeplex.com/
namespace OData2Poco.CommandLine
{
    class Program
    {
        private static readonly Stopwatch Sw = new Stopwatch();
        private static PocoSetting _PocoSetting = new PocoSetting();

        [STAThread]
        static void Main(string[] args)
        {

            try
            {

                //// Catch all unhandled exceptions in all threads.
                AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
                RunOptions(args);
            }
            catch (Exception ex)
            {
                var argument = string.Join(" ", args);
                Console.WriteLine("Error in executing the command: o2pgen {0}", argument);
                Console.WriteLine("Error Message:\n {0}", ex.Message);
                //Console.WriteLine("Error Details: {0}", ex);
                Environment.Exit(-1);
            }

        }


        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Console.WriteLine((e.ExceptionObject as Exception).Message);
            Environment.Exit(-99);
        }
        static void RunOptions(string[] args)
        {
            Sw.Start();
            var options = new Options();

            if (Parser.Default.ParseArguments(args, options))
            {

                Console.WriteLine(ApplicationInfo.HeadingInfo);
                Console.WriteLine(ApplicationInfo.Copyright);
                Console.WriteLine(ApplicationInfo.Description);
                Console.WriteLine("Start processing url: " + options.Url);
                ProcessComandLine(options);
            }
        }

        static void ProcessComandLine(Options options)
         {
           
            if (options.Url == null) return;
            O2P o2p = options.User == null
                ? new O2P(options.Url)
                : new O2P(options.Url, options.User, options.Password);
          
            //------- PocoSetting------
            if (options.Key) o2p.AddKeyAttribute();
            if (options.Table) o2p.AddTableAttribute();
            if (options.Required) o2p.AddRequiredAttribute();
            if (options.Navigation) o2p.AddNavigation();

        
             //var code = o2p.Generate(_PocoSetting);
            string code = o2p.Generate(options.CodeFilename);
            //.Generate(_PocoSetting);
            Console.WriteLine("Saving generated code to file : " + options.CodeFilename);
           // File.WriteAllText(options.CodeFilename, code);

            //---------metafile -m
            if (options.MetaFilename != null)
            {
                Console.WriteLine();
                o2p.SaveMetaDataTo(options.MetaFilename);
                Console.WriteLine("Saving Metadata to file : {0}", options.MetaFilename);
            }

            //------------ header -h for http media only not file--------------------
            if (options.Header && options.Url.StartsWith("http"))
            {
                MetaDataInfo meta = o2p;
                Console.WriteLine();
                Console.WriteLine("HTTP Header");
                Console.WriteLine(new string('=', 15));
              //  o2p.MetaData.ServiceHeader.ToList().ForEach(m =>
                meta.ServiceHeader.ToList().ForEach(m =>
                {
                    Console.WriteLine(" {0}: {1}", m.Key, m.Value);
                });
            }

            //---------list -l 
            if (options.ListPoco)
            {
                Console.WriteLine();
                Console.WriteLine("POCO classes (count: {0})", o2p.ClassList.Count);
                Console.WriteLine(new string('=', 20));
                var items = o2p.ClassList.OrderBy(m => m.Name).ToList();
                items.ForEach(m =>
                {
                    int index = items.IndexOf(m);
                    var remoteUrl = string.IsNullOrEmpty(m.EntitySetName) ? "" : options.Url + @"/" + m.EntitySetName;
                    //  Console.WriteLine("{0}: {1} ", index + 1, m.Name);
                    //v1.5
                    Console.WriteLine("{0}: {1} {2}", index + 1, m.Name, remoteUrl);
                });
            }
            //---------verbose -v
            if (options.Verbose)
            {
                Console.WriteLine();
                Console.WriteLine(code);
            }
           

            Sw.Stop();
            Console.WriteLine();
            Console.WriteLine("Total processing time: {0} sec", Sw.ElapsedMilliseconds / 1000.0);

        }

    }
}
