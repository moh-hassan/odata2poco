using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OData2Poco.CommandLine;

namespace OData2Poco.Core
{
    class ParserUtility
    {
        public async Task<Tuple<int, string>> RunCommand(string[] args)
        {
            var argumentParser = new ArgumentParser();
            argumentParser.ClearLogger();
            argumentParser.SetLoggerSilent();
            var exitCode = await argumentParser.RunOptionsAsync(args);
            var help = ArgumentParser.Help;
            return new Tuple<int, string>(exitCode, help);
        }

        public async Task<Tuple<int, string>> RunCommand(string s)
        {
            var argumentParser = new ArgumentParser();
            string[] args = s.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            argumentParser.ClearLogger();
            argumentParser.SetLoggerSilent();
            var retcode = await argumentParser.RunOptionsAsync(args);
            string outText = ArgumentParser.OutPut;
            var exitCode = retcode;
            Tuple<int, string> tuple = new Tuple<int, string>(exitCode, outText);
            return tuple;

        }
    }
}
