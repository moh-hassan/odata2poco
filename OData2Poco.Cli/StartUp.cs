// Copyright (c) Mohamed Hassan & Contributors. All rights reserved. See License.md in the project root for license information.

namespace OData2Poco.CommandLine;

using InfraStructure.FileSystem;
using InfraStructure.Logging;

public static class StartUp
{
    public static readonly ILog Logger = PocoLogger.Default;
    private static readonly Stopwatch s_sw = new();

    public static string OutPut => Logger.Output.ToString();

    public static int RetCode { get; set; } = (int)ExitCodes.Success;

    public static IPocoFileSystem FileSystem { get; set; } = new PocoFileSystem();

    public static async Task RunAsync(string[] args)
    {
        args ??= [];
        Logger.Success(ApplicationInfo.HeadingInfo);
        Logger.Normal(ApplicationInfo.Copyright);
        //read password from Input pipline if available
        var pw = Pipes.ReadPipe();
        if (!string.IsNullOrWhiteSpace(pw))
            args = args.Concat(["-p", pw.Trim()]).ToArray();
        //read configuration file if available
        if ((args.Length == 0) || (args.Length == 1 && args[0].StartsWith("@")))
        {
            var cfg = new OptionConfiguration(FileSystem);
            var flag = cfg.TryGetConfigurationFile(
                args,
                out var cli,
                out var error,
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

        if (errors.Count > 0)
        {
            foreach (var error in errors)
            {
                Logger.Warn(error);
            }
        }

        try
        {
            FileSystem = new PocoFileSystem();

            // Catch all unhandled exceptions in all threads.
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            s_sw.Start();
            RetCode = await RunOptionsAsync(args).ConfigureAwait(false);
            s_sw.Stop();
            Console.WriteLine();
            if (!ArgumentParser.ShowVersionOrHelp)
                Logger.Success($"Total processing time: {s_sw.ElapsedMilliseconds / 1000.0} sec");
        }
        catch (MetaDataNotUpdatedException ex)
        {
            //HttpRequestException status code does not indicate success: 304 (Not Modified)
            if (ex.InnerException != null)
                Logger.Warn(ex.InnerException.Message);
            else
                Logger.Warn(ex.Message);

            Logger.Info("The OData metadata is not modified, and No code generation is done.");
            Logger.Info($"Keeping the existing generated file: '{ArgumentParser.ParsedOptions.CodeFilename}'.");
            RetCode = 0;
            return;
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
            return;
        }
        finally
        {
            if (!ArgumentParser.ShowVersionOrHelp)
                Logger.Info($"Application exit code: {RetCode}");
            Environment.Exit(RetCode);
        }
    }

    internal static Task<int> RunOptionsAsync(string[] args)
    {
        var argumentParser = new ArgumentParser();
        return argumentParser.RunOptionsAsync(args);
    }

    private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
        RetCode = (int)ExitCodes.UnhandledException;
        if (e.ExceptionObject is Exception exception)
            Console.WriteLine("Unhandled exception:\n{0}", exception.Message);
        Environment.Exit(RetCode);
    }
}
