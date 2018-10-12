using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using OData2Poco.Api;
using OData2Poco.Coloring;
using OData2Poco.Extension;

namespace OData2Poco.CommandLine
{
    /// <summary>
    ///     Command Pattern to manage all options of commandline
    /// </summary>
    internal class Command : ICommand
    {
        public readonly Options ArgOptions;
        public PocoSetting PocoSettingOptions;
        public string Code { get; private set; }
        private readonly ConColor _logger = ConColor.Default;
        public O2P O2PGen { get; set; }
        public Command(Options options)
        {
            ArgOptions = options;
            // O2PGen = O2P.Default.Init(config =>
            O2PGen = new O2P(config =>
            {
                config.AddNavigation = options.Navigation;
                config.AddNullableDataType = options.AddNullableDataType;
                config.AddEager = options.Eager; //v2.1.0
                config.Inherit = string.IsNullOrWhiteSpace(options.Inherit) ? null : options.Inherit; //v2.1.0
                config.NamespacePrefix = string.IsNullOrEmpty(options.Namespace) ? string.Empty : options.Namespace; //v2.1.0
                config.NameCase = options.NameCase.ToCaseEnum(); //v2.2,
                config.Attributes = options.Attributes?.ToList();
              //  config.Generators = options.Generators?.ToList(); //todo v3.1

                config.AddKeyAttribute = options.Key;
                config.AddTableAttribute = options.Table;
                config.AddRequiredAttribute = options.Required;
                config.AddJsonAttribute = options.AddJsonAttribute; //v2.2
               
            });
            PocoSettingOptions = O2PGen.Setting;
        }

        

        public async Task Execute()
        {
            ShowOptions();
            Console.WriteLine();
            _logger.Info("Start processing url: " + ArgOptions.Url);

            //show result
            await GenerateCodeCommandAsync();
            SaveMetaDataCommand();
            ShowHeaderCommand();
            ListPocoCommand();
            VerboseCommand();
          //  GenerateDocCommand(); //todo in v3.1
        }


        public void ShowOptions()
        {
            if (!ArgOptions.Verbose) return;
            Console.WriteLine("************* CommandLine options***********");
            Console.WriteLine("-r Url: {0}", ArgOptions.Url);
            Console.WriteLine("-u User: {0}", ArgOptions.User);
            Console.WriteLine("-m Namespace: {0}", ArgOptions.Namespace);
            Console.WriteLine("-c NameCase: {0}", ArgOptions.NameCase);
            Console.WriteLine("-j AddJsonAttribute: {0}", ArgOptions.AddJsonAttribute);
            Console.WriteLine("-b AddNullableDataType: {0}", ArgOptions.AddNullableDataType);
            Console.WriteLine("-e Eager: {0}", ArgOptions.Eager);
            Console.WriteLine("-i Inherit: {0}", ArgOptions.Inherit);
            Console.WriteLine("-k Key: {0}", ArgOptions.Key);
            Console.WriteLine("-n Navigation: {0}", ArgOptions.Navigation);
            Console.WriteLine("-q Required: {0}", ArgOptions.Required);
            Console.WriteLine("-t Table: {0}", ArgOptions.Table);
            Console.WriteLine("-d Header: {0}", ArgOptions.Header);
            Console.WriteLine("-l ListPoco: {0}", ArgOptions.ListPoco);
            Console.WriteLine("-x MetaFilename: {0}", ArgOptions.MetaFilename);
            Console.WriteLine("-f CodeFilename: {0}", ArgOptions.CodeFilename);
            Console.WriteLine("-a Attributes: {0}", string.Join(", ", ArgOptions.Attributes));
            Console.WriteLine("-L Language: {0}", ArgOptions.Lang);
            Console.WriteLine("********************************************");
        }



        #region Utility

        //utility functions
        private void SaveToFile(string fileName, string text, string errorMsg)
        {
            if (string.IsNullOrEmpty(text))
                throw new Exception(errorMsg);
            var file = new FileInfo(fileName);
            //file.Directory.Create(); // If the directory already exists, this method does nothing.
            File.WriteAllText(file.FullName, text);
            File.WriteAllText(fileName, text);
            var length = new FileInfo(fileName).Length;
            if (length == 0) throw new Exception(fileName + " is empty");
        }

        #endregion

        #region commands

        private void ListPocoCommand()
        {
            //---------list -l 
            if (!ArgOptions.ListPoco) return;

            Console.WriteLine();
            Console.WriteLine("POCO classes (count: {0})", O2PGen.ClassList.Count);
            Console.WriteLine(new string('=', 20));
            var items = O2PGen.ClassList.OrderBy(m => m.Name).ToList();
            items.ForEach(m =>
            {
                var index = items.IndexOf(m);
                var remoteUrl = string.IsNullOrEmpty(m.EntitySetName) ? "" : ArgOptions.Url + @"/" + m.EntitySetName;
                //v1.5
                Console.WriteLine("{0}: {1} {2}", index + 1, m.Name, remoteUrl);
            });
        }

        private void VerboseCommand()
        {
            //---------verbose -v
            if (!ArgOptions.Verbose) return;

            Console.WriteLine();
            Console.WriteLine(Code);
        }

        private void ShowHeaderCommand()
        {
            //------------ header -h for http media only not file--------------------
            if (ArgOptions.Header && ArgOptions.Url.StartsWith("http"))
            {
                //   MetaDataInfo meta = o2p;
                Console.WriteLine();
                Console.WriteLine("HTTP Header");
                Console.WriteLine(new string('=', 15));
                O2PGen.ServiceHeader.ToList().ForEach(m => { Console.WriteLine(" {0}: {1}", m.Key, m.Value); });
            }
        }

        private async Task GenerateCodeCommandAsync()
        {

            if (ArgOptions.Url.StartsWith("http"))
            {
                Code = await O2PGen.GenerateAsync(new Uri(ArgOptions.Url), ArgOptions.User, ArgOptions.Password);
            }
            else
            {
                var xml = File.ReadAllText(ArgOptions.Url);
                Code = O2PGen.Generate(xml);
            }
          

            if (ArgOptions.Lang == "cs")
            {
                _logger.Info("Saving generated CSharp code to file : " + ArgOptions.CodeFilename);
                SaveToFile(ArgOptions.CodeFilename, Code, " c# code is empty");
            }
            else
            {
                //vb.net
                    Code = VbCodeConvertor.CodeConvert(Code); //convert to vb.net
                    if (!string.IsNullOrEmpty(Code))
                    {
                        var filename = Path.ChangeExtension(ArgOptions.CodeFilename, ".vb");
                        Console.WriteLine("Saving generated VB.NET code to file : " + ArgOptions.CodeFilename);
                        SaveToFile(filename, Code, " VB.NET code is empty");
                    }
                    else
                    {
                        _logger.Warning("Vb Service Converter isn't available. Only CS code can be generated");
                    }
            }

        }

        private void SaveMetaDataCommand()
        {
            //---------metafile -m
            if (string.IsNullOrEmpty(ArgOptions.MetaFilename)) return;

            Console.WriteLine();
            Console.WriteLine("Saving Metadata to file : {0}", ArgOptions.MetaFilename);
            var metaData = O2PGen.MetaDataAsString.FormatXml();
            SaveToFile(ArgOptions.MetaFilename, metaData, " Metadata is empty");
        }

        //todo planned in v3.1
        //private void GenerateDocCommand()
        //{
        //    _logger.Info("generating command");
        //    var generators = ArgOptions.Generators.ToList();
        //    foreach (var name in generators)
        //    {
        //        Console.WriteLine(name);
        //        var obj = GeneratorManager.Default.GetGeneratorObject(name);
        //        if (obj != null)
        //            obj.Generate();
        //    }
        //}

        #endregion
    }

    internal static class VbCodeConvertor
    {
        public static string CodeConvert(string code)
        {
            throw new NotImplementedException();
        }
    }
}