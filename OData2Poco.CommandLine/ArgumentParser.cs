using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CommandLine;
using OData2Poco.CommandLine.InfraStructure.Logging;
using OData2Poco.Extensions;

namespace OData2Poco.CommandLine
{
    public class ArgumentParser
    {
        public static bool ShowVersionOrHelp = false;
        private static StringWriter HelpWriter;
        public static string Help => HelpWriter.ToString();
        public static ColoredConsole Logger;
        public static string OutPut => Logger.Output.ToString();


        public ArgumentParser(ColoredConsole logger)
        {
            Logger = logger;
        }
        public ArgumentParser()
        {
            Logger = ColoredConsole.Default;
        }

        internal async Task<int> RunOptionsAsync(string[] args, Func<Options, Task> func)
        {
            Logger.Clear();
            var result = GetParserResult(args);

            var retCode = await result.MapResult(
                async x =>
                {
                    await func.Invoke(x);
                    return 0;
                },
                errs =>
                {
                    var retValue = GetHelp(errs);
                    return Task.FromResult<int>(retValue);
                });

            return retCode;
        }

        internal async Task<int> RunOptionsAsync(string[] args)
        {
            return await RunOptionsAsync(args, async x =>
            {
                Logger.Info(ApplicationInfo.HeadingInfo);
                Logger.Normal(ApplicationInfo.Copyright);
                //Console.WriteLine(ApplicationInfo.Description);
                await new CsCommand(x, Program._pocoFileSystem).Execute().ConfigureAwait(false);

            });
        }

        private int GetHelp(IEnumerable<Error> errors)
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
           
            Logger.Normal(HelpWriter.ToString().RemoveEmptyLines());
            return (int)ExitCodes.ArgumentsInvalid;
        }

        internal ParserResult<Options> GetParserResult(string[] args)
        {
            HelpWriter = new StringWriter();
            var parser = new Parser(config =>
            {
                config.HelpWriter = HelpWriter;
                config.CaseSensitive = true;
                config.MaximumDisplayWidth = 4000;
                config.IgnoreUnknownArguments = false;
            });

            var result = parser.ParseArguments<Options>(args);
            return result;
        }
    }
}