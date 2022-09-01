using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using OData2Poco.Extensions;
using OData2Poco.InfraStructure.FileSystem;
using OData2Poco.InfraStructure.Logging;

 
//(c) 2016-2022 Mohamed Hassan
// MIT License
//project site:https://github.com/moh-hassan/odata2poco


namespace OData2Poco.CommandLine
{
    public class StartUp
    {
        private static readonly Stopwatch Sw = new Stopwatch();
        public static ILog Logger = PocoLogger.Default;
        public static string OutPut => Logger.Output.ToString();
        public static int RetCode = (int)ExitCodes.Success;
        public static IPocoFileSystem _pocoFileSystem;


        static void SetBufferHeight()
        {
#if !NETCOREAPP
            //only supported in windows
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                return;
            if (!(Console.IsOutputRedirected || Console.IsErrorRedirected))
                Console.BufferHeight = short.MaxValue - 1;
#endif
        }
        public static async Task Run(string[] args)
        {
            var argument = string.Join(" ", args);
            try
            {
                _pocoFileSystem = new PocoFileSystem();
                SetBufferHeight();

                // Catch all unhandled exceptions in all threads.
                AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
                Sw.Start();
                RetCode = await RunOptionsAsync(args);
                Sw.Stop();
                Console.WriteLine();
                if (!ArgumentParser.ShowVersionOrHelp)
                    Logger.Sucess($"Total processing time: {Sw.ElapsedMilliseconds / 1000.0} sec");

            }
            catch (Exception ex)
            {
                RetCode = (int)ExitCodes.HandledException;
                Logger.Error($"Error in executing the command: o2pgen {argument}");
                Logger.Error($"Error Message:\n {ex.FullExceptionMessage()}");

                //#if DEBUG
                Logger.Error("--------------------Exception Details---------------------");
                Logger.Error($"Error Message:\n {ex.FullExceptionMessage(true)}");
                // Console.ReadKey();
                Console.WriteLine(ex);
                //#endif
            }
            finally
            {
                if (!ArgumentParser.ShowVersionOrHelp)
                    Logger.Info($"Application Exit code: {RetCode}");
                Environment.Exit(RetCode);
            }

        }


        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            RetCode = (int)ExitCodes.UnhandledException;
            if (e.ExceptionObject is Exception exception)
                Console.WriteLine("Unhandled exception: \r\n{0}", exception.Message);
            Environment.Exit(RetCode);
        }

        internal static async Task<int> RunOptionsAsync(string[] args)
        {
            var argumentParser = new ArgumentParser();
            return await argumentParser.RunOptionsAsync(args);
        }
    }

}


