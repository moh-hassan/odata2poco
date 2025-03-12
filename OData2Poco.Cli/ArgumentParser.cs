// Copyright (c) Mohamed Hassan & Contributors. All rights reserved. See License.md in the project root for license information.

namespace OData2Poco.CommandLine;

using InfraStructure.Logging;

#nullable disable
public class ArgumentParser
{
    private static readonly ILog s_logger = PocoLogger.Default;
    public static bool ShowVersionOrHelp { get; set; }
    public static StringWriter HelpWriter { get; private set; }
    public static Options ParsedOptions { get; private set; }
    public static string Help => HelpWriter.ToString();

    public static string OutPut => s_logger.Output.ToString();

    public void ClearLogger() => s_logger.Clear();

    public void SetLoggerSilent(bool flag = true) => s_logger.Silent = flag;

    public async Task<int> RunOptionsAsync(string[] args, Func<Options, Task> func)
    {
        s_logger.Clear();
        var result = GetParserResult(args);

        var retCode = await result.MapResult(
            async x =>
            {
                await func.Invoke(x).ConfigureAwait(false);
                return 0;
            },
            errs =>
            {
                var retValue = GetHelp(errs);
                return Task.FromResult(retValue);
            }).ConfigureAwait(false);

        return retCode;
    }

    public Task<int> RunOptionsAsync(string[] args)
    {
        return RunOptionsAsync(args, RunCommandAsync);
    }

    internal ParserResult<Options> GetParserResult(string[] args)
    {
        HelpWriter = new StringWriter();
        using var parser = new Parser(config =>
        {
            config.HelpWriter = HelpWriter;
            config.CaseSensitive = true;
            config.MaximumDisplayWidth = 4000;
            config.IgnoreUnknownArguments = false;
            config.CaseInsensitiveEnumValues = true;
        });

        var result = parser.ParseArguments<Options>(args);
        result.WithParsed(options => ParsedOptions = options); // v6.3.0
        return result;
    }

    private Task RunCommandAsync(Options options)
    {
        var command = new CsCommand(options, StartUp.FileSystem);
        return command.Execute();
    }

    private int GetHelp(IEnumerable<Error> errors)
    {
        if (errors == null)
            return 0;
        ShowVersionOrHelp = true;
        var enumerable = errors.ToList();
        if (enumerable.Exists(e => e.Tag == ErrorType.VersionRequestedError))
        {
            s_logger.Info(HelpWriter.ToString());
            return 0;
        }

        if (enumerable.Exists(e => e.Tag == ErrorType.HelpRequestedError))
        {
            s_logger.Normal(HelpWriter.ToString().RemoveEmptyLines());
            return 0;
        }

        s_logger.Normal(HelpWriter.ToString().RemoveEmptyLines());
        return (int)ExitCodes.ArgumentsInvalid;
    }
}
