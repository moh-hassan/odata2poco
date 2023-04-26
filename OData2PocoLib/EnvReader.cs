// Copyright (c) Mohamed Hassan & Contributors. All rights reserved. See License.md in the project root for license information.

using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using OData2Poco.InfraStructure.FileSystem;

namespace OData2Poco;
/// <summary>
/// Read Environment variables and options with prefix @@
/// </summary>
public class EnvReader
{
    private readonly IPocoFileSystem _fileSystem;
    public EnvReader(IPocoFileSystem fileSystem)
    {
        _fileSystem = fileSystem;
    }


    public string[] ResolveArgEnv(string[] args, out List<string> envErrors)
    {
        List<string> newArgs = new List<string>();
        envErrors = new List<string>();
        if (args.Length == 0) return Array.Empty<string>();

        foreach (var arg in args)
        {
            if (arg.Contains("%") || arg.Contains("$")) //arg is env var
            {
                newArgs.Add(ResolveEnv(arg, out var errors));
                if (errors.Count > 0)
                    envErrors.AddRange(errors);
            }
            else if (arg.StartsWith("@@")) //arg value is saved in file
            {
                var flag = TryResolveFile(arg, out var value, out var error);
                if (flag) newArgs.Add(value!);
                else
                {
                    newArgs.Add(arg);
                    envErrors.Add(error!);
                }
            }
            else
            {
                newArgs.Add(arg);
            }
        }

        return newArgs.ToArray();
    }
    /// <summary>
    /// Resolve option start with @@fileName and read the corresponding text file fileName.
    /// </summary>
    /// <param name="arg"></param>
    /// <param name="value1"></param>
    /// <param name="error"></param>
    /// <returns></returns>
    internal bool TryResolveFile(string arg, [NotNullWhen(true)] out string? value,
        [NotNullWhen(false)] out string? error)
    {
        error = null;
        value = arg;
        var fileName = arg.TrimStart('@');
        if (!_fileSystem.Exists(fileName))
        {
            error = $"File '{fileName}' is not existing.";
            return false;
        }

        value = _fileSystem.ReadAllText(fileName).Trim();
        if (!string.IsNullOrEmpty(value)) return true;

        error = $"File '{fileName}' is empty.";
        return false;
    }

    internal string ResolveEnv(string arg, out List<string> errors)
    {
        errors = new List<string>();
        var newArg = arg;
        var pattern = @"%(?<key>\w+)%|\$(?<key>\w+)";
        var matches = Regex.Matches(arg, pattern);
        if (matches.Count == 0) return arg;


        foreach (Match match in matches)
        {
            var key = match.Groups["key"].ToString().Trim();
            var flag = TryReadEnv(key, out var value);
            if (flag)
                newArg = newArg.Replace(match.Value, value);
            else
            {
                var error = $"Environment '{arg}' is not existing.";
                errors.Add(error);
            }
        }
        return newArg;
    }

    internal bool TryReadEnv(string arg, out string? v)
    {
        v = Environment.GetEnvironmentVariable(arg);
        if (!string.IsNullOrEmpty(v)) return true;
        v = Environment.GetEnvironmentVariable(arg, EnvironmentVariableTarget.User);
        if (!string.IsNullOrEmpty(v)) return true;
        v = Environment.GetEnvironmentVariable(arg, EnvironmentVariableTarget.Machine);
        if (!string.IsNullOrEmpty(v)) return true;
        return false;
    }
}