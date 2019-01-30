using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using CommandLine;
using CommandLine.Text;
using OData2Poco.CommandLine.InfraStructure.Logging;

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

        [Option('f', "filename", Default = "poco.cs", HelpText = "filename to save generated c# code.")]
        public string CodeFilename { get; set; }


        [Option('x', "metafile", HelpText = "Xml filename to save metadata.")]
        public string MetaFilename { get; set; }

        [Option('v', "verbose", HelpText = "Prints C# code to standard output.")]
        public bool Verbose { get; set; }

        [Option('d', "header", HelpText = "List  http header of the service")]
        public bool Header { get; set; }

        [Option('l', "list", HelpText = "List POCO classes to standard output.")]
        public bool ListPoco { get; set; }

        [Option('n', "navigation", HelpText = "Add navigation properties")]
        public bool Navigation { get; set; }

        [Option('e', "eager", HelpText = "Add non virtual navigation Properties for Eager Loading")]
        public bool Eager { get; set; }

        [Option('b', "nullable", HelpText = "Add nullable data types")]
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


        public IEnumerable<string> Generators { get; set; }
        [Option("lang", Default = "cs", HelpText = "Type cs for CSharp, vb for VB.NET")]
        public string Lang { get; set; } //v3

        //TODO--- ---------------------------
        //following are obsolete and will be removed in the next release
        //obsolete use -a key
        [Option('k', "key", Hidden = true, HelpText = "Obsolete, use -a key, Add Key attribute [Key]")]
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
        public List<string> Errors = new List<string>();
        #region usage
#if NETFULL
        [Usage(ApplicationAlias = "o2pgen")]
#else
        [Usage(ApplicationAlias = "dotnet o2pgen")]
#endif
        public static IEnumerable<Example> Examples
        {
            get
            {
                yield return new Example("Default setting",
                    new Options { Url = "http://services.odata.org/V4/OData/OData.svc" });
                yield return new Example("Add json, key Attributes with camel case and nullable types", new Options
                {
                    Url = "http://services.odata.org/V4/OData/OData.svc",
                    Attributes = new List<string> { "json", "key" },
                    NameCase = "camel",
                    AddNullableDataType = true
                });
            }
        }
        #endregion
        public int Validate()
        {

            //validating Lang
            switch (Lang)
            {
                case "vb":
                    CodeFilename = Path.ChangeExtension(CodeFilename, ".vb");
                    break;
                case "cs":
                    CodeFilename = Path.ChangeExtension(CodeFilename, ".cs");
                    break;
                default:
                    Errors.Add($"Invalid Language Option '{Lang}'. It's set to 'cs'.");
                    CodeFilename = Path.ChangeExtension(CodeFilename, ".cs");
                    break;
                    //return -1;
            }
            //validate NameCase
            if (string.IsNullOrEmpty(NameCase))
            {
                Errors.Add($"NameCase '{NameCase}' is empty. It is set to 'pas'.");//warning
                NameCase = "pas";
            }
            if (!Regex.IsMatch(NameCase.ToLower(), "cam|camel|none|pas", RegexOptions.IgnoreCase))
            {
                Errors.Add($"NameCase '{NameCase}' isn't valid. It is set to 'pas'.");//warning
                NameCase = "pas";
            }

            //validate Attributes
            foreach (var attribute in Attributes.ToList())
            {
                if (attribute.Trim().StartsWith("[")) continue;
                if (!Regex.IsMatch(attribute.Trim().ToLower(), "key|req|tab|table|json|db|proto|dm|display", RegexOptions.IgnoreCase))
                {
                    Errors.Add($"Attribute '{attribute}' isn't valid. It will be  droped.");//warning

                }
            }


            return 0;
        }

    }
}
