
using System;
using System.IO;
using System.Threading.Tasks;
using Medallion.Shell;
using NUnit.Framework;

namespace OData2Poco.CommandLine.Test
{
#if NETFRAMEWORK
    class ProgramTestsConsole : ProgramTests
    {
        private const double Timeout = 3 * 60; //sec
        private static readonly string appCommand = (@"..\..\..\..\OData2Poco.CommandLine\bin\Release\net45\o2pgen.exe");
       
        protected override async Task<Tuple<int, string>> RunCommand(string s)
        {
            Console.WriteLine(Path.GetFullPath(appCommand));
            var command = Command.Run(appCommand, s.Split(' '),
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
