
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
        private static string appCommand = (@"..\..\..\..\OData2Poco.CommandLine\bin\Release\net45\o2pgen.exe");
        string   WorkingDirectory="";
        public ProgramTestsConsole()
        {

           
        }
        [OneTimeSetUp]
        public void RunBeforeAnyTests()
        {
            Environment.CurrentDirectory = TestContext.CurrentContext.TestDirectory;
            WorkingDirectory=Environment.CurrentDirectory;
            appCommand = WinLinuxPath(appCommand);
            //WorkingDirectory =Path.GetDirectoryName(appCommand);
            //appCommand="O2Pgen.exe";
        }
        protected override async Task<Tuple<int, string>> RunCommand(string s)
        {
            Console.WriteLine($"WorkingDirectory= {WorkingDirectory}");

            Console.WriteLine($"ProgramTestsDotnet - path= {appCommand}");
            Console.WriteLine($"isexist= {File.Exists(appCommand)}");
           
           
            var command = Command.Run(appCommand, s.Split(' '),
                options =>
                {
                    options.Timeout(TimeSpan.FromSeconds(Timeout));
                    options.WorkingDirectory(WorkingDirectory);
                });

            var outText = command.Result.StandardOutput;
            var errText = command.Result.StandardError;
            var exitCode = command.Result.ExitCode;
            Tuple<int, string> tuple = new Tuple<int, string>(exitCode, $"{outText}{Environment.NewLine}{errText}");
            return tuple;

        }
        [Test]
        public void WinLinuxPathTest()
        {
           var path=WinLinuxPath( appCommand);
           
            Console.WriteLine(path);
            Assert.That(path,Is.EqualTo(appCommand));
        }
    }
#endif
}
