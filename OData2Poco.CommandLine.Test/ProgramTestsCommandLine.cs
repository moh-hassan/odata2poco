using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OData2Poco.Core;

namespace OData2Poco.CommandLine.Test
{
    class ProgramTestsCommandLine :ProgramTests
    {
        protected override async Task<Tuple<int, string>> RunCommand(string s)
        {
            return await new ParserUtility().RunCommand(s);
        }
    }
}
