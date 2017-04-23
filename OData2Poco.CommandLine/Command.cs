using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using OData2Poco.Shared;

namespace OData2Poco.CommandLine
{
    /// <summary>
    ///     Command Pattern to manage all options of commandline
    /// </summary>
    internal class Command : ICommand
    {
        public readonly Options ArgOptions;
        public readonly PocoSetting PocoSettingOptions;
        public string  Code { get; private set; } 

        public Command(Options options)
        {
            ArgOptions = options;
            PocoSettingOptions = new PocoSetting
            {
                AddKeyAttribute = ArgOptions.Key,
                AddTableAttribute = ArgOptions.Table,
                AddRequiredAttribute = ArgOptions.Required,
                AddNavigation = ArgOptions.Navigation,
                AddPartial = ArgOptions.PartialPocoClasses,
                AddNullableDataType = ArgOptions.AddNullableDataType,
                AddEager = ArgOptions.Eager, //v2.1.0
                Inherit = string.IsNullOrWhiteSpace(ArgOptions.Inherit) ? string.Empty : ArgOptions.Inherit, //v2.1.0
                NamespacePrefix = string.IsNullOrEmpty(ArgOptions.Namespace) ? string.Empty : ArgOptions.Namespace,
                //v2.1.0
                AddJsonAttribute = ArgOptions.AddJsonAttribute, //v2.2
                NameCase = ArgOptions.NameCase.ToCaseEnum() //v2.2
            };
            AddAttributes();
            O2PGen = new O2P(PocoSettingOptions);
            //Console.WriteLine("constrrrrrrrrr {0} {1}",ArgOptions.NameCase ,PocoSettingOptions.NameCase);
        }

        public O2P O2PGen { get; set; }

        public async Task Execute()
        {
            ShowOptions();
            Console.WriteLine();
            Console.WriteLine("Start processing url: " + ArgOptions.Url);

            //show result
            await GenerateCodeCommandAsync();
            SaveMetaDataCommand();
            ShowHeaderCommand();
            ListPocoCommand();
            VerboseCommand();
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
            Console.WriteLine("-g PartialPocoClasses: {0}", ArgOptions.PartialPocoClasses);
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


            Console.WriteLine("********************************************");
        }

        #region PocoSetting

        /// <summary>
        ///     full or first three chars
        /// </summary>
        public void AddAttributes()
        {
            foreach (var a in ArgOptions.Attributes)
            {
                switch (a.ToLower())
                {
                    case "key":
                        PocoSettingOptions.AddKeyAttribute = true;
                        break;
                    case "req":
                    case "required":
                        PocoSettingOptions.AddRequiredAttribute = true;
                        break;
                    case "tab":
                    case "table":
                        PocoSettingOptions.AddTableAttribute = true;
                        break;
                    case "par":
                        PocoSettingOptions.AddPartial = true;
                        break;
                    case "nul":
                    case "null":
                        PocoSettingOptions.AddNullableDataType = true;
                        break;
                    case "eag":
                    case "eager":
                        PocoSettingOptions.AddEager = true;
                        break;
                    case "jso":
                    case "json":
                        PocoSettingOptions.AddJsonAttribute = true;
                        break;
                }
                //Console.WriteLine("att: {0}", a);
            }
        }

        #endregion

        #region Utility

        //utility functions
        private void SaveToFile(string fname, string text, string errorMsg)
        {
            //todo:check file exist
            if (string.IsNullOrEmpty(text))
                throw new Exception(errorMsg);
            var file = new FileInfo(fname);
            //file.Directory.Create(); // If the directory already exists, this method does nothing.
            File.WriteAllText(file.FullName, text);
            File.WriteAllText(fname, text);
            var length = new FileInfo(fname).Length;
            if (length == 0) throw new Exception(fname + " is empty");
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
                //  Console.WriteLine("{0}: {1} ", index + 1, m.Name);
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
                //  o2p.MetaData.ServiceHeader.ToList().ForEach(m =>
                O2PGen.ServiceHeader.ToList().ForEach(m => { Console.WriteLine(" {0}: {1}", m.Key, m.Value); });
            }
        }

        private async Task GenerateCodeCommandAsync()
        {
            if (ArgOptions.Url.StartsWith("http"))
            {
                Code = await O2PGen.GenerateAsync(new Uri(ArgOptions.Url), ArgOptions.User, ArgOptions.Password); //
            }
            else
            {
                var xml = File.ReadAllText(ArgOptions.Url);
                Code = O2PGen.Generate(xml);
            }
            //   Console.WriteLine(code);
            //.Generate(_PocoSetting);
            Console.WriteLine("Saving generated code to file : " + ArgOptions.CodeFilename);
            SaveToFile(ArgOptions.CodeFilename, Code, " c# code is empty");
        }

        private void SaveMetaDataCommand()
        {
            //---------metafile -m
            if (string.IsNullOrEmpty(ArgOptions.MetaFilename)) return;

            Console.WriteLine();
            //SaveMetaDataTo(options.MetaFilename);
            Console.WriteLine("Saving Metadata to file : {0}", ArgOptions.MetaFilename);
            var metaData = O2PGen.MetaDataAsString.FormatXml();
            SaveToFile(ArgOptions.MetaFilename, metaData, " Metadata is empty");
        }

        #endregion
    }
}