using System.Collections.Generic;
using CommandLine;

//(c) 2016-2018 Mohamed Hassan, MIT License
////Project site: https://github.com/moh-hassan/odata2poco
namespace OData2Poco.CommandLine
{
    // Define a class to receive parsed values
    class Options
    {
        public Options()
        {
            Attributes = new List<string>();
            Generators = new List<string>();
        }

        [Option('r', "url", Required = true, HelpText = "URL of OData feed.")]
        public string Url { get; set; }

        [Option('u', "user", HelpText = "User name for authentication.")]
        public string User { get; set; }

        [Option('p', "password", HelpText = "password for authentication.")]
        public string Password { get; set; }

        [Option('f', "filename", Default ="poco.cs", HelpText = "filename to save generated c# code.")]
        public string CodeFilename { get; set; }

        //bugfix in 2.2.0
        //change m to x , to be different than namespace
        [Option('x', "metafile", HelpText = "Xml filename to save metadata.")]
        public string MetaFilename { get; set; }

        [Option('v', "verbose", HelpText = "Prints C# code to standard output.")]
        public bool Verbose { get; set; }

        [Option('d', "header", HelpText = "List  http header of the service")]
        public bool Header { get; set; }

        [Option('l', "list",  HelpText = "List POCO classes to standard output.")]
        public bool ListPoco { get; set; }

        [Option('n', "navigation", HelpText = "Add navigation properties")]
        public bool Navigation { get; set; }

        [Option('e', "eager", HelpText = "Add non virtual navigation Properties for Eager Loading")]
        public bool Eager { get; set; }

        [Option('b', "nullable",  HelpText = "Add nullable data types")]
        public bool AddNullableDataType { get; set; }
      

        [Option('i', "inherit", HelpText = "for class inheritance from  BaseClass and/or interfaces")]
        public string Inherit { get; set; }

        [Option('m', "namespace", HelpText = "A namespace prefix for the OData namespace")]
        public string Namespace { get; set; }

      
        [Option('c', "case", Default = "none", HelpText = "Type pas or camel to Convert Property Name to PascalCase or CamelCase")]
        public string NameCase { get; set; }
      
        [Option('a', "attribute",
        HelpText = "Attributes, Allowed values: key, req, json,tab,dm,proto,db,display")]
        public IEnumerable<string> Attributes { get; set; }

       // [Option('g', "generate", HelpText = "generate text document")] //todo v3.1
        public IEnumerable<string> Generators { get; set; }
        [Option("lang", Default = "cs", Hidden = true, HelpText = "Type cs for CSharp, vb for VB.NET")]
        public string  Lang { get; set; } //v3

        //TODO--- ---------------------------
        //following are obsolete and will be removed in the next release
        //obsolete use -a key
        [Option('k', "key",Hidden = true,HelpText = "Obsolete, use -a key, Add Key attribute [Key]")]
        public bool Key { get; set; }

        //obsolete use -a tab
        [Option('t', "table", Hidden = true, HelpText = "Obsolete, use -a tab, Add Table attribute")]
        public bool Table { get; set; }

        //obsolete use -a req
        [Option('q', "required", Hidden = true, HelpText = "Obsolete, use -a req, Add Required attribute")]
        public bool Required { get; set; }
        //obsolete use -a json
        [Option('j', "Json", Hidden = true, HelpText = "Obsolete, use -a json, Add JsonProperty Attribute, example:  [JsonProperty(PropertyName = \"email\")]")]
        public bool AddJsonAttribute { get; set; }

    }
}
