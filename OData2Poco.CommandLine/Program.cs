using System;
using System.Diagnostics;
using System.Threading.Tasks;
using CommandLine;
using OData2Poco.Extension;

//todo: file source
//(c) 2016 Mohamed Hassan
// MIT License
//project site: http://odata2poco.codeplex.com/

namespace OData2Poco.CommandLine
{
    class Program
    {
        private static readonly Stopwatch Sw = new Stopwatch();
        //private static PocoSetting _pocoSetting = new PocoSetting();

        // [STAThread]
        static void Main(string[] args)
        {
            try
            {
                // Catch all unhandled exceptions in all threads.
                AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
                Sw.Start();
                RunOptionsAsync(args).Wait();
                Sw.Stop();
                Console.WriteLine();
                Console.WriteLine("Total processing time: {0} sec", Sw.ElapsedMilliseconds/1000.0);

                Environment.Exit(0);
#if DEBUG
                 Console.ReadKey();
#endif
            }
            catch (Exception ex)
            {
                var argument = string.Join(" ", args);
                Console.WriteLine("Error in executing the command: o2pgen {0}", argument);
#if DEBUG
                Console.WriteLine("Error Message:\n {0}", ex.FullExceptionMessage(true));
#else
                Console.WriteLine("Error Message:\n {0}", ex.FullExceptionMessage());
#endif
                //Console.WriteLine("Error Details: {0}", ex);
                Environment.Exit(-1);
            }
        }


        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var exception = e.ExceptionObject as Exception;
            if (exception != null)
                Console.WriteLine("Unhandled exception: {0}", exception.Message);
            Environment.Exit(-99);
        }


        static async Task RunOptionsAsync(string[] args)
        {
            var options = new Options();
            if (Parser.Default.ParseArguments(args, options))
            {
                Console.WriteLine(ApplicationInfo.HeadingInfo);
                Console.WriteLine(ApplicationInfo.Copyright);
                Console.WriteLine(ApplicationInfo.Description);
                await new Command(options).Execute();
            }
        }
    }
}