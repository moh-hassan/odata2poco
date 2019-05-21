
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
        //F:\odata2poco-workflow\odata2poco\OData2Poco.CommandLine.Test\bin\Release\net45
        private const double Timeout = 3 * 60; //sec
        private static string appCommand = @"..\..\..\..\OData2Poco.CommandLine\bin\Release\net45\o2pgen.exe";
        string   _workingDirectory="";
       
        [OneTimeSetUp]
        public void RunBeforeAnyTests()
        {
            Environment.CurrentDirectory = TestContext.CurrentContext.TestDirectory;
            _workingDirectory=Environment.CurrentDirectory;
            appCommand = Path.GetFullPath(WinLinuxPath(appCommand));
        }
        protected override async Task<Tuple<int, string>> RunCommand(string s)
        {
            Console.WriteLine($"WorkingDirectory= {_workingDirectory}");

            Console.WriteLine($"ProgramTestsDotnet - path= {Path.GetFullPath(appCommand)}");
            Console.WriteLine($"isexist= {File.Exists(appCommand)}");
           
           
            var command = Command.Run(appCommand, s.Split(' '),
                options =>
                {
                    options.Timeout(TimeSpan.FromSeconds(Timeout));
                    options.WorkingDirectory(_workingDirectory);
                });

            var outText = command.Result.StandardOutput;
            var errText = command.Result.StandardError;
            var exitCode = command.Result.ExitCode;
            Tuple<int, string> tuple = new Tuple<int, string>(exitCode, $"{outText}{Environment.NewLine}{errText}");
            await Task.FromResult (0);
            return tuple;

        }
       
    }
#endif
}
