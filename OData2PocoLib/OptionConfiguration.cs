// Copyright (c) Mohamed Hassan & Contributors. All rights reserved. See License.md in the project root for license information.

using System.Diagnostics.CodeAnalysis;
using System.Text;
using OData2Poco.Extensions;
using OData2Poco.InfraStructure.FileSystem;

namespace OData2Poco;
public class OptionConfiguration
{
    private readonly IPocoFileSystem _fileSystem;
    public ErrorCollection Errors { get; set; }
    public OptionConfiguration(IPocoFileSystem fs)
    {
        _fileSystem = fs;
        Errors = new ErrorCollection();
    }
    public OptionConfiguration() : this(new PocoFileSystem())
    {
    }


    /// <summary>
    /// read args from a configuration file.
    /// </summary>
    /// <param name="args">Commandline arguments</param>
    /// <param name="commandLine">args array in the configuration file</param>
    /// <returns>true: Have a config file, false: no config file</returns>
    public bool TryGetConfigurationFile(string[] args, out string[] commandLine)
    {
        commandLine = args;
        var (head, _) = args; //ignore tail
        if (!TryFindFile(head, out string? fileName))
            return false;
        commandLine = ReadConfig(fileName);
        return true;
    }
    [SuppressMessage("Style", "IDE0062:Make local function 'static'", Justification = "<Pending>")]
    internal string[] ReadConfig(string fileName)
    {
        var sb = new StringBuilder();
        var text = _fileSystem.ReadAllText(fileName);
        if (string.IsNullOrEmpty(text))
            return Array.Empty<string>();

        using StringReader reader = new StringReader(text);
        while (reader.ReadLine() is { } line)
        {
            var line2 = LineClean(line);
            if (string.IsNullOrEmpty(line2)) continue;
            sb.Append($"{line2} ");
        }
        var args = sb.ToString().Trim().SplitArgs();
        return args;

        string? LineClean(string line)
        {
            line = line.Trim();
            if (string.IsNullOrEmpty(line) || line.StartsWith("#"))
                return null;
            var pos = line.IndexOf("#", StringComparison.Ordinal);
            if (pos > 0)
                line = line[..pos];
            return line;
        }
    }
    internal bool TryFindFile(string? head, [NotNullWhen(true)] out string? fileName)
    {
        fileName = "o2pgen.txt";
        var hasConfig = false;
        if (head?.StartsWith("@") ?? false)
        {
            hasConfig = true;
            fileName = head.Substring(1);
        }

        if (!_fileSystem.Exists(fileName))
        {
            if (hasConfig)
                Errors.AddWarning($"Configuration file: {fileName} is not existing");
            return false;
        }
        Errors.AddInfo($"Reading configuration file: {fileName}");
        return true;
    }
}