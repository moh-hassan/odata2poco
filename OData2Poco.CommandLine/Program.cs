using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using CommandLine;

//(c) 2016 Mohamed Hassan
// MIT License
//project site: http://odata2poco.codeplex.com/
namespace OData2Poco.CommandLine
{
    class Program
    {
        private static Stopwatch sw = new Stopwatch();
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
            sw.Start();
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

            var code = o2p.Generate();
            Console.WriteLine("Saving generated code to file : " + options.CodeFilename);
            File.WriteAllText(options.CodeFilename, code);
            //------------ header -h --------------------
            if (options.Header)
            {
                Console.WriteLine();
                Console.WriteLine("HTTP Header");
                Console.WriteLine(new string('=', 15));
                o2p.ServiceHeader.ToList().ForEach(m =>
                {
                    Console.WriteLine(" {0}: {1}", m.Key, m.Value);
                });
            }

            //---------metafile -m
            if (options.MetaFilename != null)
            {
                Console.WriteLine();
                o2p.SaveMetadata(options.MetaFilename);
                Console.WriteLine("Saving Metadata to file : " + options.MetaFilename);
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
                    Console.WriteLine("{0}: {1}", index + 1, m.Name);
                });
            }
            //---------verbose -v
            if (options.Verbose)
            {
                Console.WriteLine();
                Console.WriteLine(code);
            }
            sw.Stop();
            Console.WriteLine();
            Console.WriteLine("Total processing time: {0} sec", sw.ElapsedMilliseconds / 1000.0);

        }

    }
}
