using System;
using System.Diagnostics;
using System.Threading.Tasks;
using CommandLine;
using CommandLine.Text;
using OData2Poco.Coloring;
using OData2Poco.Extension;

//todo: file source
//(c) 2016-2018 Mohamed Hassan
// MIT License
//project site:https://github.com/moh-hassan/odata2poco


namespace OData2Poco.CommandLine
{
    internal class Program
    {
        private static readonly Stopwatch Sw = new Stopwatch();
        static readonly ConColor Logger = ConColor.Default;
        static int _retCode = (int)ExitCodes.Success;
        static async Task Main(string[] args)
        {
         
           
            var argument = string.Join(" ", args);
            try
            {
                if (!(Console.IsOutputRedirected || Console.IsErrorRedirected))
                    Console.BufferHeight = Int16.MaxValue - 1;
                // Catch all unhandled exceptions in all threads.
                AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
                Sw.Start();
                _retCode = await RunOptionsAsync(args);
                Sw.Stop();
                Console.WriteLine();
                Logger.Sucess("Total processing time: {0} sec", Sw.ElapsedMilliseconds / 1000.0);

            }
            catch (Exception ex)
            {
                _retCode = (int)ExitCodes.HandledException;
                Logger.Error("Error in executing the command: o2pgen {0}", argument);
                Logger.Error("Error Message:\n {0}", ex.FullExceptionMessage());
             
#if DEBUG
                Logger.Error("--------------------Exception Details---------------------");
                Logger.Error("Error Message:\n {0}", ex.FullExceptionMessage(true));
                Console.ReadKey();

#endif
            }
            finally
            {
                Logger.Info($"Application Exit code: {_retCode}");
                Environment.Exit(_retCode);
            }

        }


        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            _retCode = (int)ExitCodes.UnhandledException;
            if (e.ExceptionObject is Exception exception)
                Console.WriteLine("Unhandled exception: \r\n{0}", exception.Message);
            Environment.Exit(_retCode);
        }


        static async Task<int> RunOptionsAsync(string[] args)
        {


            var parser = new Parser(config =>
            {
                config.HelpWriter = null;
                config.CaseSensitive = true;
                config.MaximumDisplayWidth = 4000;
                config.IgnoreUnknownArguments = false;

            });

            //catch exception of parser before go on

            var result = parser.ParseArguments<Options>(args);

            var retCode = await result.MapResult(
               async x =>
                {
                    Logger.Info(ApplicationInfo.HeadingInfo);
                    Console.WriteLine(ApplicationInfo.Copyright);
                    Console.WriteLine(ApplicationInfo.Description);
                    await new Command(x).Execute().ConfigureAwait(false);
                    return 0;
                },
                errs =>
                {
                   
                    var helpText = HelpText.AutoBuild(result, h =>
                    {
                        h.AdditionalNewLineAfterOption = false;
                        return HelpText.DefaultParsingErrorsHandler(result, h);
                    }, e => e);
                    Console.WriteLine(helpText);
                    return Task.FromResult((int)ExitCodes.ArgumentsInvalid);
                });

            return retCode;
        }

    }

}


