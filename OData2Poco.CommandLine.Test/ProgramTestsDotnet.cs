
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using NUnit.Framework;

namespace OData2Poco.CommandLine.Test
{
#if !NETFRAMEWORK
    //End to End test for the dotnetapp dll
    [Category("ProgramTestsDotnet")]
    public class ProgramTestsDotnet : ProgramTests
    {

        private const double Timeout = 3 * 60; //sec
        private static string appCommand = (@"..\..\..\..\OData2Poco.dotnet.o2pgen\bin\Release\netcoreapp2.1\O2Pgen.dll");
      string   WorkingDirectory="";

      [OneTimeSetUp]
      public void RunBeforeAnyTests()
      {
          Environment.CurrentDirectory = TestContext.CurrentContext.TestDirectory;
          WorkingDirectory=Environment.CurrentDirectory;
          appCommand = Path.GetFullPath( WinLinuxPath(appCommand));
          //WorkingDirectory =Path.GetDirectoryName(appCommand);
          //appCommand="O2Pgen.exe";
      }
        protected override async Task<Tuple<int, string>> RunCommand(string s)
        {
            Console.WriteLine($"ProgramTestsDotnet - path= {Path.GetFullPath(appCommand)}");
            Console.WriteLine($"isexist= {File.Exists(appCommand)}");
            Console.WriteLine($"WorkingDirectory= {WorkingDirectory}");
            var appCommand1 = "dotnet";
            var list = new List<string> { appCommand };
            list.AddRange(s.Split(' '));
            var command = Medallion.Shell.Command.Run(appCommand1, list,
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
    }
#endif
}
