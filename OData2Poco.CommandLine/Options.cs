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
        public Options()
        {
            Attributes =new string[]{};
            Assemplies = new string[] { };
        }

        [Option('r', "url", Required = true, HelpText = "URL of OData feed.")]
        public string Url { get; set; }

        [Option('u', "user",  HelpText = "User name for authentication.")]
        public string User { get; set; }

        [Option('p', "password", HelpText = "password for authentication.")]
        public string Password { get; set; }

        [Option('f', "filename", DefaultValue = "poco.cs", HelpText = "filename to save generated c# code.")]
        public string CodeFilename { get; set; }

        //bugfix in 2.2.0
        //change m to x , to be different than namespace
        [Option('x', "metafile", HelpText = "Xml filename to save metadata.")]
        public string MetaFilename { get; set; }

        [Option('v', "verbose", DefaultValue = false, HelpText = "Prints C# code to standard output.")]
        public bool Verbose { get; set; }

        [Option('d', "header", DefaultValue = false, HelpText = "List  http header of the service")]
        public bool Header { get; set; }

        [Option('l', "list", DefaultValue = false, HelpText = "List POCO classes to standard output.")]
        public bool ListPoco { get; set; }

        //version 2.0
        //PocoSetting options
        [Option('k', "key", DefaultValue = false, HelpText = "Add Key attribute [Key]")]
        public bool Key { get; set; }
        [Option('t', "table", DefaultValue = false, HelpText = "Add Table attribute")]
        public bool Table { get; set; }

        [Option('q', "required", DefaultValue = false, HelpText = "Add Required attribute")]
        public bool Required { get; set; }

        [Option('n', "navigation", DefaultValue = false, HelpText = "Add navigation properties")]
        public bool Navigation { get; set; }

        [Option('g', "partial", DefaultValue = false, HelpText = "Generate the poco classes as partial")]
        public bool PartialPocoClasses { get; set; }

        [Option('e', "eager", DefaultValue = false, HelpText = "Add non virtual navigation Properties for Eager Loading")]
        public bool Eager { get; set; }

        [Option('b', "nullable", DefaultValue = false, HelpText = "Add nullable data types")]
        public bool AddNullableDataType { get; set; }
        
        //[VerbOption("config", HelpText = "Configure code generation.")]
        //public ConfigSubOptions ConfigVerb { get; set; }

        [Option('i', "inherit", HelpText = "for class inheritance from  BaseClass and/or interfaces")]
        public string Inherit { get; set; }

        [Option('m', "namespace", HelpText = "A namespace prefix for the OData namespace")]
        public string Namespace { get; set; }

        //camel /pascal , at least the first three chars Caps/lower or mixed e.g PAS or pas or Pas
        [Option('c', "case", DefaultValue = "none", HelpText = "Type pas or camel to Convert Property Name to PascalCase or CamelCase")]
        public string NameCase { get; set; }

        //All attribues e.g -a key required json customAttribute
        [OptionArray('a', "attribute",
        HelpText = "Type all attributes separated by one or more space.Allowed are:key required json table.")]
        public string[] Attributes { get; set; }

        [Option('j', "Json", DefaultValue = false,
            HelpText = "Add JsonProperty Attribute, example:  [JsonProperty(PropertyName = \"email\")]")]
        public bool AddJsonAttribute { get; set; }

        //todo: add feature, modify pocosetting
        //external reference assemplies needed to be defined by user in using clause statement
      //  [OptionArray('y', "values", DefaultValue = new string[] { })]
        public string[] Assemplies { get; set; }
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

    // class ConfigSubOptions
    //{
    //    [Option('k', "key", Required = false, HelpText = "Add Key Attribute")]
    //    public string  Key { get; set; }
    //}
}
