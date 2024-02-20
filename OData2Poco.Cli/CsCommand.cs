// Copyright (c) Mohamed Hassan & Contributors. All rights reserved. See License.md in the project root for license information.

namespace OData2Poco.CommandLine;

using Api;
using InfraStructure.FileSystem;
using InfraStructure.Logging;

#nullable disable
/// <summary>
///     Command Pattern to manage all options of commandline
/// </summary>
internal class CsCommand : IPocoCommand
{
    public readonly Options ArgOptions;
    public PocoSetting _pocoSettingOptions;
    public OdataConnectionString _odataConnectionString;
    public List<string> _errors; //model generation errors
    private readonly ILog _logger = PocoLogger.Default;
    private readonly IPocoFileSystem _fileSystem;

    public CsCommand(Options options, IPocoFileSystem fileSystem)
    {
        _fileSystem = fileSystem ?? new NullFileSystem();
        _errors = [];
        OptionManager optionManager = new(options);
        ArgOptions = optionManager.PocoOptions;
        (_odataConnectionString, _pocoSettingOptions) = optionManager;
        O2PGen = new O2P(_pocoSettingOptions);
    }

    public string Code { get; private set; }

    public O2P O2PGen { get; set; }

    public async Task Execute()
    {
        ShowOptions();
        Console.WriteLine();
        ArgOptions.Errors.ForEach(_logger.Error);

        //show warning
        ArgOptions.Errors.ForEach(_logger.Warn);
        _logger.Info($"Start processing url: {_odataConnectionString.ServiceUrl}");

        //show result
        await GenerateCodeCommandAsync().ConfigureAwait(false);
        ShowWarning(); //warning of model property/class renaming
        GenerateProjectCommand();
        ServiceInfo();
        SaveMetaDataCommand();
        ShowHeaderCommand();
        ListPocoCommand();
        VerboseCommand();
        ShowErrors();
    }

    private void ShowOptions()
    {
        ShowOptions(ArgOptions);
    }

    private void ShowOptions(Options option)
    {
        //format option as: -n Navigation= True
        _logger.Normal("************* CommandLine Options***********");
        var list = CommandLineUtility.GetOptions(option);
        list.ForEach(_logger.Normal);
        _logger.Normal("********************************************");
    }

    //errors of invalid commandline options
    private void ShowErrors()
    {
        if (_errors.Count == 0) return;
        _logger.Error("--------- Errors--------");
        _errors.ForEach(_logger.Error);
    }

    private void ServiceInfo()
    {
        _logger.Normal($"{new string('-', 15)}Service Information {new string('-', 15)}");
        _logger.Info($"OData Service Url: {_odataConnectionString.ServiceUrl} ");
        _logger.Info($"OData Service Version: {O2PGen.MetaDataVersion} ");
        _logger.Info($"Number of Entities: {O2PGen.ClassList.Count}");
        _logger.Normal(new string('-', 50));
        _logger.Success("Successfully Generated Poco Model");
    }

    private void ListPocoCommand()
    {
        //---------list -l
        if (!ArgOptions.ListPoco) return;

        Console.WriteLine();
        _logger.Info($"POCO classes (count: {O2PGen.ClassList.Count}) | EntitySet");
        _logger.Normal(new string('=', 40));
        var items = O2PGen.ClassList.OrderBy(m => m.NameSpace).ThenBy(x => x.Name).ToList();
        items.ForEach(m =>
        {
            var complex = m.IsComplex ? " Is Complex Entity" : string.Empty;
            var enumType = m.IsEnum ? " Is Enum Type" : string.Empty;
            var openType = m.IsOpen ? " (OpenType)" : string.Empty;
            var note = complex + enumType + openType;
            var url = _odataConnectionString.ServiceUrl.StartsWith("http")
                ? _odataConnectionString.ServiceUrl.TrimEnd('/')
                : string.Empty;
            var index = items.IndexOf(m);
            var remoteUrl = string.IsNullOrEmpty(m.EntitySetName)
                ? string.Empty
                : url + "/" + m.EntitySetName;

            _logger.Normal(!string.IsNullOrEmpty(remoteUrl)
                ? $"{index + 1}: {m.NameSpace}.{m.Name} {openType}| {remoteUrl}"
                : $"{index + 1}: {m.NameSpace}.{m.Name} | {note}");
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
    }

    private void ShowHeaderCommand()
    {
        //------------ header -h for http media only not file--------------------
        if (ArgOptions.Header && _odataConnectionString.ServiceUrl.StartsWith("http"))
        {
            Console.WriteLine();
            _logger.Normal("HTTP Header");
            _logger.Normal(new string('=', 15));
            O2PGen.ServiceHeader.ToList().ForEach(m => _logger.Normal($" {m.Key}: {m.Value}"));
        }
    }

    private async Task GenerateCodeCommandAsync()
    {
        if (ArgOptions.Lang == Language.CS)
        {
            Code = await O2PGen.GenerateAsync(_odataConnectionString).ConfigureAwait(false);
            _logger.Normal("Saving generated CSharp code to file : " + ArgOptions.CodeFilename);
            SaveToFile(ArgOptions.CodeFilename, Code);
            _logger.Confirm("CSharp code  is generated Successfully.");
        }
        else if (ArgOptions.Lang == Language.TS)
        {
            var pocoStore = await O2PGen.GenerateTsAsync(_odataConnectionString).ConfigureAwait(false);
            pocoStore.Save(ArgOptions.CodeFilename, _fileSystem, ArgOptions.MultiFiles);

            if (ArgOptions.MultiFiles)
            {
                _logger.Normal($"Saving generated typescript code to folder: '{ArgOptions.CodeFilename}'");
                _logger.Confirm("typescript code  is generated Successfully.");
            }
            else //single file
            {
                _logger.Normal($"Saving generated typescript code to file: '{ArgOptions.CodeFilename}'");
                if (ArgOptions.Verbose)
                    _logger.Normal(pocoStore.Display().ToString());
                _logger.Confirm("typescript code  is generated Successfully.");
            }
        }
        else
        {
            Console.WriteLine($"!!!!! ArgOptions.Lang {ArgOptions.Lang}");
            _logger.Warn(
                $"Lang option: '{ArgOptions.Lang}' isn't valid. Only cs/ts are accepted \r\n No code is generated");
            Code = string.Empty;
        }
    }

    private void SaveMetaDataCommand()
    {
        //---------metafile -m
        if (string.IsNullOrEmpty(ArgOptions.MetaFilename)) return;

        _logger.Normal(string.Empty);
        _logger.Normal($"Saving Metadata to file : {ArgOptions.MetaFilename}");
        var metaData = O2PGen.MetaDataAsString.FormatXml();
        SaveToFile(ArgOptions.MetaFilename, metaData);
    }

    private void GenerateProjectCommand()
    {
        //---------   --gen-project, -g
        if (!ArgOptions.GenerateProject) return;
        var fname = "un.proj";
        if (ArgOptions.Lang == Language.CS)
            fname = Path.ChangeExtension(ArgOptions.CodeFilename, ".csproj");
        var projectCode = O2PGen.GenerateProject();
        _logger.Normal($"Generating project file {fname}");
        File.WriteAllText(fname!, projectCode);
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
            O2PGen.ModelWarning.ForEach(_logger.Normal);
        }
    }
}
