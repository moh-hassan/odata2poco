// Copyright (c) Mohamed Hassan & Contributors. All rights reserved. See License.md in the project root for license information.

using System.Diagnostics.CodeAnalysis;
using System.Text;
using OData2Poco.Extensions;
using OData2Poco.InfraStructure.FileSystem;

namespace OData2Poco;
public class OptionConfiguration
{
    private readonly IPocoFileSystem _fileSystem;
  
    public OptionConfiguration(IPocoFileSystem fs)
    {
        _fileSystem = fs;
        
    }
    public OptionConfiguration() : this(new PocoFileSystem())
    {
    }



    public bool TryGetConfigurationFile(string[] args, out string[] commandLine,
        out string? error, out string? fileName)
    {
        error = null;
        fileName = null;
        if (args.Length == 0 && _fileSystem.Exists("o2pgen.txt"))
        {
            fileName = "o2pgen.txt";
            commandLine = ReadConfig(fileName);
            return true;
        }

        if (args.Length == 1 && args[0].StartsWith("@"))
        {
            fileName = args[0].TrimStart('@');
            if (!_fileSystem.Exists(fileName))
            {
                error = $"Configuration file {fileName} is not existing";
                commandLine = args;
                return false;
            }
            commandLine = ReadConfig(fileName);
            return true;
        }

        commandLine = args;
        return false;
    }

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
            return line.Trim();
        }
    }
}