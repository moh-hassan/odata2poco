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
        public static IPocoFileSystem FileSystem { get; set; } = new PocoFileSystem();

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
            Logger.Success(ApplicationInfo.HeadingInfo);
            Logger.Normal(ApplicationInfo.Copyright);
            //read password from Input pipline if available
            var pw = Pipes.ReadPipe();
            if (!string.IsNullOrWhiteSpace(pw))
                args = args.Concat(new[] { "-p", pw.Trim() }).ToArray();

            //read configuration file if available
            if ((args.Length == 0) || (args.Length == 1 && args[0].StartsWith("@")))
            {
                var cfg = new OptionConfiguration(FileSystem);
                var flag = cfg.TryGetConfigurationFile(args, out var cli, out var error,
                    out var fileName);
                if (flag)
                {
                    Logger.Info($"Reading configuration file: {fileName}");
                    args = cli;
                }

                if (!string.IsNullOrEmpty(error))
                {
                    Logger.Error(error);
                }
            }
            //merge repeating args
            args = args.MergeRepeatingArgs();

            //read environment variables
            args = new EnvReader(FileSystem).ResolveArgEnv(args, out var errors);

            if (errors.Any())
            {
                foreach (var error in errors)
                {
                    Logger.Warn(error);
                }
            }

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
                    Logger.Success($"Total processing time: {Sw.ElapsedMilliseconds / 1000.0} sec");

            }
            catch (Exception ex)
            {
                RetCode = (int)ExitCodes.HandledException;
                Logger.Error("Error in executing o2pgen");
#if DEBUG
                Logger.Error($"{ex.FullExceptionMessage(true)}");
#else
                Logger.Error($"{ex.FullExceptionMessage()}");
                FileSystem.WriteAllText("error.txt", $"{ex.FullExceptionMessage(true)}");
                Logger.Info($"Error details with Stack trace are written to the file: '{Path.GetFullPath("error.txt")}'");
#endif
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


