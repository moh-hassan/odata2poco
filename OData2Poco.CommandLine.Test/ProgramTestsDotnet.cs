
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace OData2Poco.CommandLine.Test
{
#if !NETFRAMEWORK
    //End to End test for the dotnetapp dll
    public class ProgramTestsDotnet : ProgramTests
    {

        private const double Timeout = 3 * 60; //sec
        private static readonly string appCommand = (@"..\..\..\..\OData2Poco.dotnet.o2pgen\bin\Release\netcoreapp2.1\o2pgen.dll");

        protected override async Task<Tuple<int, string>> RunCommand(string s)
        {
            Console.WriteLine(Path.GetFullPath(appCommand));
            var appCommand1 = "dotnet";
            var list = new List<string> { appCommand };
            list.AddRange(s.Split(' '));
            var command = Medallion.Shell.Command.Run(appCommand1, list,
                options => options.Timeout(TimeSpan.FromSeconds(Timeout)));

            var outText = command.Result.StandardOutput;
            var errText = command.Result.StandardError;
            var exitCode = command.Result.ExitCode;
            Tuple<int, string> tuple = new Tuple<int, string>(exitCode, $"{outText}{Environment.NewLine}{errText}");
            return tuple;

        }
    }
#endif
}
