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
        //public Options()
        //{
        //    // Since we create this instance the parser will not overwrite it
        //    ConfigVerb = new ConfigSubOptions(); //{ Key = true };
        //}


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

        //PocoSetting options
        [Option('k', "key", DefaultValue = false, HelpText = "Add Key attribute [Key]")]
        public bool Key { get; set; }
        [Option('t', "table", DefaultValue = false, HelpText = "Add Table attribute")]
        public bool Table { get; set; }

        [Option('q', "required", DefaultValue = false, HelpText = "Add Required attribute")]
        public bool Required { get; set; }

        [Option('n', "Navigation", DefaultValue = false, HelpText = "Add Navigation Properties")]
        public bool Navigation { get; set; }
        //[VerbOption("config", HelpText = "Configure code generation.")]
        //public ConfigSubOptions ConfigVerb { get; set; }

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

     class ConfigSubOptions
    {
        [Option('k', "key", Required = false, HelpText = "Add Key Attribute")]
        public string  Key { get; set; }
    }
}
