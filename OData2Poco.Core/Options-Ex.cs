// Copyright (c) Mohamed Hassan & Contributors. All rights reserved. See License.md in the project root for license information.

using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;

// ReSharper disable ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
#pragma warning disable S3267

namespace OData2Poco.CommandLine;

// Options of commandline
public partial class Options
{

    private string GetToken(string text)
    {
        if (string.IsNullOrEmpty(text)) return "";
        string password;
        if (text.Trim().StartsWith("{"))
        {
            //do json
            var jobject = JObject.Parse(text);
            password = jobject.ContainsKey("acces_token")
                ? jobject["acces_token"]?.ToString()
                : string.Empty;
        }
        else
            password = text;

        return password;
    }
    public void Validate()
    {
        if (string.IsNullOrEmpty(CodeFilename))
        {
            if (MultiFiles)
                CodeFilename = "Model";
            CodeFilename = $"poco.{Lang.ToString().ToLower()}";
        }

        //todo 
        //if (Password != null && Password.StartsWith("@@"))
        //{
        //    var fname = Password.Substring(1);
        //    var text = File.ReadAllText(fname);
        //    Password = GetToken(text);
        //}


        //validate Attributes
        foreach (var attribute in Attributes.ToList())
        {
            if (attribute.Trim().StartsWith("[")) continue;
            if (!Regex.IsMatch(attribute.Trim().ToLower(), "key|req|tab|table|json|db|proto|dm|display|original|max", RegexOptions.IgnoreCase))
            {
                Errors.Add($"Attribute '{attribute}' isn't valid. It will be  droped.");//warning

            }
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