using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using OData2Poco.Api;
using OData2Poco.Extensions;
using OData2Poco.InfraStructure.FileSystem;
using OData2Poco.InfraStructure.Logging;
/*
   read parameter file into dictionary
   recursive resolve macros in dictionary
   replace options values for: password , tokens ,url
 * */
namespace OData2Poco.CommandLine
{
    /// <summary>
    ///     Command Pattern to manage all options of commandline
    /// </summary>
    internal class CsCommand : IPocoCommand
    {
        public readonly Options ArgOptions;
        public PocoSetting PocoSettingOptions;
        public OdataConnectionString odataConnectionString;
        public string Code { get; private set; }
        private readonly ILog _logger = PocoLogger.Default;
        public O2P O2PGen { get; set; }
        public List<string> Errors; //model generation errors
        private IPocoFileSystem _fileSystem;
        private OptionManager optionManager;
        public CsCommand(Options options, IPocoFileSystem fileSystem)
        {
            if (fileSystem == null)
                _fileSystem = new NullFileSystem();
            else
            {
                _fileSystem = fileSystem;
            }

            Errors = new List<string>();
            ArgOptions = optionManager = new OptionManager(options);
            odataConnectionString = optionManager.GetOdataConnectionString();
            PocoSettingOptions = optionManager.GetPocoSetting();
            O2PGen = new O2P(PocoSettingOptions);
        }

        public async Task Execute()
        {

            ShowOptions();
            Console.WriteLine();
           
            ArgOptions.Errors.ForEach(x =>
            {
                _logger.Error(x);
            });
           

            //show warning
            ArgOptions.Errors.ForEach(x =>
                {
                    _logger.Warn(x);
                });
            //setup plugin path
         
            if (!string.IsNullOrEmpty(ArgOptions.PluginPath))
            {
                Parameters.PluginPath =ArgOptions.PluginPath;
            }
            _logger.Info($"Start processing url: { odataConnectionString.ServiceUrl}");
            //show result
            await GenerateCodeCommandAsync();
            ShowWarning(); //warning of model property/class renaming
            GenerateProjectCommand();
            ServiceInfo();

            SaveMetaDataCommand();
            ShowHeaderCommand();
            ListPocoCommand();
            VerboseCommand();
            ShowErrors();

        }
        public void ShowOptions(Options option)
        {
            //format option as: -n Navigation= True
            _logger.Normal("************* CommandLine options***********");
            var list = CommandLineUtility.GetOptions(option);
            list.ForEach(x => _logger.Normal(x));
            _logger.Normal("********************************************");
        }
        //errors of invalid commandline options
        public void ShowErrors()
        {
            if (Errors.Count == 0) return;
            _logger.Error("--------- Errors--------");
            Errors.ForEach(x =>
            {
                _logger.Error(x);
            });
        }
        public void ServiceInfo()
        {
            _logger.Normal($"{new string('-', 15)}Service Information {new string('-', 15)}");
            _logger.Info($"OData Service Url: {odataConnectionString.ServiceUrl} ");
            _logger.Info($"OData Service Version: {O2PGen.MetaDataVersion} ");
            _logger.Info($"Number of Entities: {O2PGen.ClassList.Count}");
            _logger.Normal(new string('-', 50));
            _logger.Sucess("Success creation of the Poco Model");
        }

        public void ShowOptions()
        {
            ShowOptions(ArgOptions);
        }

        #region commands

        private void ListPocoCommand()
        {
            //---------list -l 
            if (!ArgOptions.ListPoco) return;

            Console.WriteLine();
            _logger.Info($"POCO classes (count: {O2PGen.ClassList.Count}) | EntitySet");
            _logger.Normal(new string('=', 40));
            var items = O2PGen.ClassList.OrderBy(m => m.NameSpace).ThenBy(x=>x.Name).ToList();
            items.ForEach(m =>
            {
                var index = items.IndexOf(m);
                var remoteUrl = string.IsNullOrEmpty(m.EntitySetName)
                    ? string.Empty
                    : odataConnectionString.ServiceUrl + @"/" + m.EntitySetName;
                _logger.Normal(!string.IsNullOrEmpty(remoteUrl)
                    ? $"{index + 1}: {m.NameSpace}.{m.Name} | {remoteUrl}"
                    : $"{index + 1}: {m.NameSpace}.{m.Name}");
            });
        }

        private void VerboseCommand()
        {
            //---------verbose -v
            if (!ArgOptions.Verbose) return;

            Console.WriteLine();
            if (!string.IsNullOrEmpty(Code))
            {
                _logger.Normal("---------------Code Generated--------------------------------");
                _logger.Normal(Code);
            }
            else
            {
                _logger.Error("Code not generated");
            }
        }

        private void ShowHeaderCommand()
        {
            //------------ header -h for http media only not file--------------------
            if (ArgOptions.Header && odataConnectionString.ServiceUrl.StartsWith("http"))
            {
                Console.WriteLine();
                _logger.Normal("HTTP Header");
                _logger.Normal(new string('=', 15));
                O2PGen.ServiceHeader.ToList().ForEach(m => { _logger.Normal($" {m.Key}: {m.Value}"); });
            }
        }

        private async Task GenerateCodeCommandAsync()
        {
            Code = await O2PGen.GenerateAsync(odataConnectionString);
            if (ArgOptions.Lang == "cs")
            {
                _logger.Normal("Saving generated CSharp code to file : " + ArgOptions.CodeFilename);
                SaveToFile(ArgOptions.CodeFilename, Code);
                _logger.Confirm("CSharp code  is generated Successfully.");
            }
            else if (ArgOptions.Lang == "vb")
            {
                //vb.net
                Code = await VbCodeConvertor.CodeConvert(Code); //convert to vb.net
                if (!string.IsNullOrEmpty(Code))
                {
                    var filename = Path.ChangeExtension(ArgOptions.CodeFilename, ".vb");
                    _logger.Normal("Saving generated VB.NET code to file : " + ArgOptions.CodeFilename);
                    SaveToFile(filename, Code);
                    _logger.Confirm("VB.NET code  is generated Successfully.");
                }
                else
                {
                    _logger.Warn("Vb Service Converter isn't available.");
                }
            }
            else
            {
                _logger.Warn($"Lang option: '{ArgOptions.Lang}' isn't valid. Only cs or vb are accepted \r\n No code is generated");
                Code = "";
            }
        }

        private void SaveMetaDataCommand()
        {
            //---------metafile -m
            if (string.IsNullOrEmpty(ArgOptions.MetaFilename)) return;

            _logger.Normal("");
            _logger.Normal($"Saving Metadata to file : {ArgOptions.MetaFilename}");
            var metaData = O2PGen.MetaDataAsString.FormatXml();
            SaveToFile(ArgOptions.MetaFilename, metaData);
        }

        private void GenerateProjectCommand()
        {
            //---------   --gen-project, -g
            if (!ArgOptions.GenerateProject) return;
            var fname = "un.proj";
            if (ArgOptions.Lang == "cs")
                fname = Path.ChangeExtension(ArgOptions.CodeFilename, ".csproj");
            if (ArgOptions.Lang == "vb")
                fname = Path.ChangeExtension(ArgOptions.CodeFilename, ".csproj");
            var projectCode = O2PGen.GenerateProject();
            _logger.Normal($"Generating project file {fname}");
            File.WriteAllText(fname, projectCode);
        }
        private void SaveToFile(string fileName, string text)
        {
            _fileSystem.SaveToFile(fileName, text);
        }
        //Show warning of model warning of renaming properties
        private void ShowWarning()
        {
            if (ArgOptions.ShowWarning)
            {
                O2PGen.ModelWarning.ForEach(x =>
                {
                    _logger.Normal(x);
                });
            }
        }
        #endregion
    }
}