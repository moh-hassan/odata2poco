// Copyright (c) Mohamed Hassan & Contributors. All rights reserved. See License.md in the project root for license information.

using System.Text.RegularExpressions;


namespace OData2Poco.CommandLine;

// Options of commandline
public partial class Options
{
    public void Validate()
    {
        if (string.IsNullOrEmpty(CodeFilename))
        {
            if (MultiFiles)
                CodeFilename = "Model";
            CodeFilename = $"poco.{Lang.ToString().ToLower()}";
        }

        if (!string.IsNullOrWhiteSpace(NameMapFile))
        {
            if (!File.Exists(NameMapFile))
            {
                Errors.Add($"{NameMapFile} does not exist or is not a file.");
                NameMapFile = string.Empty;
            }
            else
            {
                //We want to validate the JSON but it also has a side effect of setting the RenameMap.                   
                using var ifh = new StreamReader(NameMapFile);
                RenameMap = Newtonsoft.Json.JsonConvert.DeserializeObject<RenameMap>(ifh.ReadToEnd());
                if (RenameMap is null)
                {
                    Errors.Add("Failed to convert the rename map file to JSON.");
                }
                else
                {
                    // Validate regexes.
                    foreach (var className in RenameMap.PropertyNameMap.Keys)
                    {
                        foreach (var map in RenameMap.PropertyNameMap[className])
                        {
                            if (map.OldName.IndexOf('^') == 0)
                            {
                                // MUST start with ^
                                if (className.Equals("ALL", StringComparison.OrdinalIgnoreCase))
                                {
                                    try
                                    {
                                        _ = Regex.IsMatch("anything", map.OldName);
                                    }
                                    catch
                                    {
                                        Errors.Add("Invalid regex: " + map.OldName);
                                    }
                                }
                                else
                                {
                                    Errors.Add("There is an OldName regex for " + className + " -- They are only valid for the ALL class.");
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}