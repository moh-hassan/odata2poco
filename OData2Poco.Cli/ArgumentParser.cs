// Copyright (c) Mohamed Hassan & Contributors. All rights reserved. See License.md in the project root for license information.

using CommandLine;
using OData2Poco.Extensions;
using OData2Poco.InfraStructure.Logging;

namespace OData2Poco.CommandLine;
#nullable disable
public class ArgumentParser
{
    public static bool ShowVersionOrHelp { get; set; }
    public static StringWriter HelpWriter { get; private set; }
    public static string Help => HelpWriter.ToString();
    private static readonly ILog Logger = PocoLogger.Default;
    public static string OutPut => Logger.Output.ToString();

    public void ClearLogger() => Logger.Clear();

    public void SetLoggerSilent(bool flag = true) => Logger.Silent = flag;

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
                return Task.FromResult(retValue);
            });

        return retCode;
    }

    public async Task<int> RunOptionsAsync(string[] args)
    {
        return await RunOptionsAsync(args, RunCommandAsync);
    }

    private async Task RunCommandAsync(Options options) =>
        await new CsCommand(options, StartUp.FileSystem).Execute().ConfigureAwait(false);

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
            config.CaseInsensitiveEnumValues = true;
        });

        var result = parser.ParseArguments<Options>(args);
        return result;
    }
}
#nullable restore