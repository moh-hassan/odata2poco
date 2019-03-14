using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CommandLine;
using OData2Poco.Extensions;
using OData2Poco.InfraStructure.Logging;

namespace OData2Poco.CommandLine
{
    public class ArgumentParser
    {
        public static bool ShowVersionOrHelp = false;
        private static StringWriter _helpWriter;
        public static string Help => _helpWriter.ToString();
        public static ILog Logger = PocoLogger.Default;
        public static string OutPut => Logger.Output.ToString();

        public void ClearLogger()
        {
            //ArgumentParser.Logger.Silent = true;
            Logger.Clear();
        }
        public void SetLoggerSilent(bool flag = true)
        {
            Logger.Silent = flag;

        }

        public async Task<int> RunOptionsAsync(string[] args, Func<Options, Task> func)
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

        public async Task<int> RunOptionsAsync(string[] args)
        {
            return await RunOptionsAsync(args, RunCommandAsync);
        }

        private async Task RunCommandAsync(Options options)
        {
            Logger.Info(ApplicationInfo.HeadingInfo);
            Logger.Normal(ApplicationInfo.Copyright);
            //Logger.Normal(ApplicationInfo.Description);
            await new CsCommand(options, StartUp._pocoFileSystem).Execute().ConfigureAwait(false);
        }

        private int GetHelp(IEnumerable<Error> errors)
        {
            if (errors == null)
                return 0;
            ShowVersionOrHelp = true;
            var enumerable = errors.ToList();
            if (enumerable.Any(e => e.Tag == ErrorType.VersionRequestedError))
            {
                Logger.Info(_helpWriter.ToString());
                return 0;
            }

            if (enumerable.Any(e => e.Tag == ErrorType.HelpRequestedError))
            {
                Logger.Normal(_helpWriter.ToString().RemoveEmptyLines());
                return 0;
            }

            Logger.Normal(_helpWriter.ToString().RemoveEmptyLines());
            return (int)ExitCodes.ArgumentsInvalid;
        }

        internal ParserResult<Options> GetParserResult(string[] args)
        {
            _helpWriter = new StringWriter();
            var parser = new Parser(config =>
            {
                config.HelpWriter = _helpWriter;
                config.CaseSensitive = true;
                config.MaximumDisplayWidth = 4000;
                config.IgnoreUnknownArguments = false;
            });

            var result = parser.ParseArguments<Options>(args);
            return result;
        }
    }
}