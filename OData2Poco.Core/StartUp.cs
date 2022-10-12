// Copyright (c) Mohamed Hassan & Contributors. All rights reserved. See License.md in the project root for license information.

using System.Diagnostics;
using OData2Poco.Extensions;
using OData2Poco.InfraStructure.FileSystem;
using OData2Poco.InfraStructure.Logging;
#if !NETCOREAPP
using System.Runtime.InteropServices;
#endif
namespace OData2Poco.CommandLine
{
    public static class StartUp
    {
        private static readonly Stopwatch Sw = new();
        public static readonly ILog Logger = PocoLogger.Default;
        public static string OutPut => Logger.Output.ToString();
        public static int RetCode { get; set; } = (int)ExitCodes.Success;
        public static IPocoFileSystem FileSystem { get; set; }

        private static void SetBufferHeight()
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
                FileSystem = new PocoFileSystem();
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


