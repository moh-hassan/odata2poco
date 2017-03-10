using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using OData2Poco.Shared;

namespace OData2Poco.CommandLine
{
    /// <summary>
    /// Command Pattern to manage all options of commandline 
    /// </summary>
    internal class Command : ICommand
    {
        
        public readonly Options ArgOptions;  
        public readonly PocoSetting PocoSettingOptions;  
        public O2P O2PGen { get; set; } //= new O2P(_PocoSetting);
        //private readonly O2P O2PGen; //= new O2P(_PocoSetting);
        string _code = "";
        public Command(Options options)
        {
            ArgOptions = options;
            PocoSettingOptions = new PocoSetting
            {
                AddKeyAttribute = ArgOptions.Key,
                AddTableAttribute = ArgOptions.Table,
                AddRequiredAttribute = ArgOptions.Required,
                AddNavigation = ArgOptions.Navigation,
                AddNullableDataType = ArgOptions.AddNullableDataType,
                AddEager = ArgOptions.Eager, //v2.1.0
                Inherit = string.IsNullOrWhiteSpace(ArgOptions.Inherit) ? string.Empty : ArgOptions.Inherit, //v2.1.0
                NamespacePrefix = string.IsNullOrEmpty(ArgOptions.Namespace) ? string.Empty : ArgOptions.Namespace, //v2.1.0
                AddJsonAttribute = ArgOptions.AddJsonAttribute, //v2.2
                NameCase = ArgOptions.NameCase.ToCaseEnum() //v2.2
            };
            AddAttributes();
            O2PGen = new O2P(PocoSettingOptions);
            //Console.WriteLine("constrrrrrrrrr {0} {1}",ArgOptions.NameCase ,PocoSettingOptions.NameCase);
        }


        public void ShowOptions()
        {
            //foreach (var a in ArgOptions.Attributes)
            //{
            //    Console.WriteLine("att: {0}", a);
            //}
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


            Console.WriteLine("********************************************");
        }

        #region PocoSetting
        /// <summary>
        /// full or first three chars
        /// </summary>
        public void AddAttributes()
        {
            foreach (var a in ArgOptions.Attributes)
            {
                switch (a.ToLower())
                {
                    case "key": PocoSettingOptions.AddKeyAttribute = true; break;
                    case "req":
                    case "required": PocoSettingOptions.AddRequiredAttribute = true; break;
                    case "tab":
                    case "table": PocoSettingOptions.AddTableAttribute = true; break;
                    case "nul":
                    case "null": PocoSettingOptions.AddNullableDataType = true; break;
                    case "eag":
                    case "eager": PocoSettingOptions.AddEager = true; break;
                    case "jso":
                    case "json": PocoSettingOptions.AddJsonAttribute = true; break;
                }
                //Console.WriteLine("att: {0}", a);
            }
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
                int index = items.IndexOf(m);
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
            Console.WriteLine(_code);

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
                O2PGen.ServiceHeader.ToList().ForEach(m =>
                {
                    Console.WriteLine(" {0}: {1}", m.Key, m.Value);
                });
            }
        }

        private async Task GenerateCodeCommandAsync()
        {
            if (ArgOptions.Url.StartsWith("http"))
            {
                _code = await O2PGen.GenerateAsync(new Uri(ArgOptions.Url), ArgOptions.User, ArgOptions.Password); //
            }
            else
            {
                var xml = File.ReadAllText(ArgOptions.Url);
                _code = O2PGen.Generate(xml);

            }
            //   Console.WriteLine(code);
            //.Generate(_PocoSetting);
            Console.WriteLine("Saving generated code to file : " + ArgOptions.CodeFilename);
            SaveToFile(ArgOptions.CodeFilename, _code, " c# code is empty");
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

        public async Task Execute()
        {
            ShowOptions();
            //------- PocoSetting------
            //AddKeyAttribute();
            //AddTableAttribute();
            //AddRequiredAttribute();
            //AddNavigation();
            //AddNullableDataType();
            //v2.1.0
            //AddEager();
            //AddInherit();
            //AddNamespacePrefix();
            //v2.2.0
            //AddNameCase();
            //AddJsonAttribute();
            //AddAttributes(); //

            Console.WriteLine();
            Console.WriteLine("Start processing url: " + ArgOptions.Url);

            //show result
            await GenerateCodeCommandAsync();
            SaveMetaDataCommand();
            ShowHeaderCommand();
            ListPocoCommand();
            VerboseCommand();

        }
        #region Utility
        //utility functions
        private bool SaveToFile(string fname, string text, string errorMsg)
        {
            //todo:check file exist
            if (string.IsNullOrEmpty(text))
                throw new Exception(errorMsg);
            FileInfo file = new FileInfo(fname);
            //file.Directory.Create(); // If the directory already exists, this method does nothing.
            File.WriteAllText(file.FullName, text);
            File.WriteAllText(fname, text);
            long length = new FileInfo(fname).Length;
            if (length == 0) throw new Exception(fname + " is empty");
            return true;
        }
        //public string FormatXml(string xml)
        //{
        //    XDocument doc = XDocument.Parse(xml);
        //    return doc.ToString();
        //}
        #endregion

#if x
        static async Task ProcessComandLineAsync(Options options)
        {
            //------- PocoSetting------
            if (options.Key) _PocoSetting.AddKeyAttribute = true;
            if (options.Table) _PocoSetting.AddTableAttribute = true;
            if (options.Required) _PocoSetting.AddRequiredAttribute = true;
            if (options.Navigation) _PocoSetting.AddNavigation = true;
            if (options.Eager) _PocoSetting.AddEager = true;
            if (options.AddNullableDataType) _PocoSetting.AddNullableDataType = true;
            _PocoSetting.Inherit = string.IsNullOrWhiteSpace(options.Inherit) ? string.Empty : options.Inherit;
            _PocoSetting.NamespacePrefix = string.IsNullOrWhiteSpace(options.Namespace) ? string.Empty : options.Namespace;

            //v2.2.0
            if (options.NameCase.ToLower().StartsWith("pas")) _PocoSetting.NameCase = CaseEnum.Pas;
            else if (options.NameCase.ToLower().StartsWith("cam")) _PocoSetting.NameCase = CaseEnum.Camel;
            else _PocoSetting.NameCase = CaseEnum.None;

            if (options.AddJsonAttribute) _PocoSetting.AddJsonAttribute = true;

            //  if (options.Url == null) return;
            //O2P o2p = options.User == null
            //    ? new O2P(new Uri(options.Url), _PocoSetting)
            //    : new O2P(new Uri(options.Url), options.User, options.Password, _PocoSetting);

            O2P o2p = new O2P(_PocoSetting);
            string code = "";
            if (options.Url.StartsWith("http"))
            {
                code = await o2p.GenerateAsync(new Uri(options.Url), options.User, options.Password); //
            }
            else
            {
                var xml = File.ReadAllText(options.Url);
                code = o2p.Generate(xml);

            }
            //   Console.WriteLine(code);
            //.Generate(_PocoSetting);
            Console.WriteLine("Saving generated code to file : " + options.CodeFilename);
            SaveToFile(options.CodeFilename, code, " c# code is empty");


            //---------metafile -m
            if (options.MetaFilename != null)
            {
                Console.WriteLine();
                //SaveMetaDataTo(options.MetaFilename);
                Console.WriteLine("Saving Metadata to file : {0}", options.MetaFilename);
                var metaData = FormatXml(o2p.MetaDataAsString);
                SaveToFile(options.MetaFilename, metaData, " Metadata is empty");
            }

            //------------ header -h for http media only not file--------------------
            if (options.Header && options.Url.StartsWith("http"))
            {
                //   MetaDataInfo meta = o2p;
                Console.WriteLine();
                Console.WriteLine("HTTP Header");
                Console.WriteLine(new string('=', 15));
                //  o2p.MetaData.ServiceHeader.ToList().ForEach(m =>
                o2p.ServiceHeader.ToList().ForEach(m =>
                {
                    Console.WriteLine(" {0}: {1}", m.Key, m.Value);
                });
            }

            //---------list -l 
            if (options.ListPoco)
            {
                Console.WriteLine();
                Console.WriteLine("POCO classes (count: {0})", o2p.ClassList.Count);
                Console.WriteLine(new string('=', 20));
                var items = o2p.ClassList.OrderBy(m => m.Name).ToList();
                items.ForEach(m =>
                {
                    int index = items.IndexOf(m);
                    var remoteUrl = string.IsNullOrEmpty(m.EntitySetName) ? "" : options.Url + @"/" + m.EntitySetName;
                    //  Console.WriteLine("{0}: {1} ", index + 1, m.Name);
                    //v1.5
                    Console.WriteLine("{0}: {1} {2}", index + 1, m.Name, remoteUrl);
                });
            }
            //---------verbose -v
            if (options.Verbose)
            {
                Console.WriteLine();
                Console.WriteLine(code);
            }


            Sw.Stop();
            Console.WriteLine();
            Console.WriteLine("Total processing time: {0} sec", Sw.ElapsedMilliseconds / 1000.0);

        }
#endif
    }
}
