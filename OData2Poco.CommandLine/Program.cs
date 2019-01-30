using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CommandLine;
using CommandLine.Text;
using OData2Poco.CommandLine.InfraStructure.FileSystem;
using OData2Poco.CommandLine.InfraStructure.Logging;
using OData2Poco.Extension;

//todo: file source
//(c) 2016-2018 Mohamed Hassan
// MIT License
//project site:https://github.com/moh-hassan/odata2poco


namespace OData2Poco.CommandLine
{
    public class Program
    {
        private static readonly Stopwatch Sw = new Stopwatch();
        public static ColoredConsole Logger = ColoredConsole.Default;
        public static int RetCode = (int)ExitCodes.Success;
        public static StringWriter HelpWriter;
        public static IPocoFileSystem _pocoFileSystem;
        public static bool ShowVersionOrHelp = false;

        static async Task Main(string[] args)
        {


            var argument = string.Join(" ", args);
            try
            {
                _pocoFileSystem = new PocoFileSystem();
                if (!(Console.IsOutputRedirected || Console.IsErrorRedirected))
                    Console.BufferHeight = Int16.MaxValue - 1;
                // Catch all unhandled exceptions in all threads.
                AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
                Sw.Start();
                RetCode = await RunOptionsAsync(args);
                Sw.Stop();
                Console.WriteLine();
                if (!ShowVersionOrHelp)
                    Logger.Sucess($"Total processing time: {Sw.ElapsedMilliseconds / 1000.0} sec");

            }
            catch (Exception ex)
            {
                RetCode = (int)ExitCodes.HandledException;
                Logger.Error($"Error in executing the command: o2pgen {argument}");
                Logger.Error($"Error Message:\n {ex.FullExceptionMessage()}");

#if DEBUG
                Logger.Error("--------------------Exception Details---------------------");
                Logger.Error($"Error Message:\n {ex.FullExceptionMessage(true)}");
                Console.ReadKey();

#endif
            }
            finally
            {
                if (!ShowVersionOrHelp)
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
            Logger.Clear();
            var result = GetParserResult(args);

            var retCode = await result.MapResult(
               async x =>
                {
                    Logger.Info(ApplicationInfo.HeadingInfo);
                    Logger.Normal(ApplicationInfo.Copyright);
                    //Console.WriteLine(ApplicationInfo.Description);
                    await new CsCommand(x, _pocoFileSystem).Execute().ConfigureAwait(false);
                    return 0;
                },
                errs =>
                {
                    var retValue = GetHelp(errs);
                    return Task.FromResult(retValue);
                });

            return retCode;
        }

        private static int GetHelp(IEnumerable<Error> errors)
        {
            if (errors == null)
                return 0;
            ShowVersionOrHelp = true;
            var enumerable = errors.ToList();
            if (enumerable.Any(e => e.Tag == ErrorType.VersionRequestedError))
            {
                Logger.Info(HelpWriter.ToString());
                return 0;
            }

            if (enumerable.Any(e => e.Tag == ErrorType.HelpRequestedError))
            {
                Logger.Normal(HelpWriter.ToString().RemoveEmptyLines());
                return 0;
            }
            //options errors
            Logger.Normal(HelpWriter.ToString().RemoveEmptyLines());
            return (int)ExitCodes.ArgumentsInvalid;
        }

        internal static ParserResult<Options> GetParserResult(string[] args)
        {
            HelpWriter = new StringWriter();
            var parser = new Parser(config =>
            {
                config.HelpWriter = HelpWriter;
                config.CaseSensitive = true;
                config.MaximumDisplayWidth = 4000;
                config.IgnoreUnknownArguments = false;
            });

            //catch exception of parser before go on

            var result = parser.ParseArguments<Options>(args);
            return result;
        }
    }

}


