using System.Reflection;
using CommandLine;
using CommandLine.Text;

//CommandLine is a class library developed by Authors: Giacomo Stelluti Scala, Copyright (c) 2005 - 2015 Giacomo Stelluti Scala gimmemoore under Mit License.
//Project site:  https://www.nuget.org/packages/CommandLineParser

//(c) 2016 Mohamed Hassan, MIT License
////Project site: http://odata2poco.codeplex.com/
namespace OData2Poco.CommandLine
{
    // Define a class to receive parsed values
    class Options
    {

        [Option('r', "url", Required = true, HelpText = "URL of OData feed.")]
        public string Url { get; set; }

        [Option('u', "user", Required = false, HelpText = "User name for authentication.")]
        public string User { get; set; }

        [Option('p', "password", Required = false, HelpText = "password for authentication.")]
        public string Password { get; set; }

        [Option('f', "filename", DefaultValue = "poco.cs", HelpText = "filename to save generated c# code.")]
        public string CodeFilename { get; set; }

        [Option('m', "metafile", HelpText = "Xml filename to save metadata.")]
        public string MetaFilename { get; set; }

        [Option('v', "verbose", DefaultValue = false, HelpText = "Prints C# code to standard output.")]
        public bool Verbose { get; set; }

        [Option('d', "header", DefaultValue = false, HelpText = "List  http header of the service")]
        public bool Header { get; set; }

        [Option('l', "list", DefaultValue = false, HelpText = "List POCO classes to standard output.")]
        public bool ListPoco { get; set; }


        [ParserState]
        public IParserState LastParserState { get; set; }

        [HelpOption]
        public string GetUsage()
        {

            var help = new HelpText
            {
                Heading = new HeadingInfo(ApplicationInfo.HeadingInfo),
                Copyright = new CopyrightInfo(ApplicationInfo.Author,2016),
                AdditionalNewLineAfterOption = true,
                AddDashesToOption = true
            };
            //help.AddPreOptionsLine("<<license details here.>>");
            help.AddPreOptionsLine("Usage: o2pGen [options]  ");
            help.AddOptions(this);
            return help;
        }
    }
}
