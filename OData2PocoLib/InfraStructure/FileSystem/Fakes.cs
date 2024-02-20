// Copyright (c) Mohamed Hassan & Contributors. All rights reserved. See License.md in the project root for license information.

namespace OData2Poco.InfraStructure.FileSystem;

using Extensions;

internal static class Fakes
{
    private static readonly Dictionary<string, string> s_data = new()
    {
        ["data1.txt"] = @"
 aaa
",
        ["data2.txt"] = @"
 bbb
",
    };

    public static void Mock(string fileName, string content)
    {
        s_data[fileName] = content;
    }

    public static void Mock(string fileName, object obj)
    {
        var json = obj.ToJson();
        s_data[fileName] = json;
    }

    public static string Get(string? fileName)
    {
        return fileName != null && s_data.TryGetValue(fileName, out var value) ? value : string.Empty;
    }

    public static bool Exists(string? key)
    {
        return key != null && s_data.ContainsKey(key);
    }

    public static void Remove(string? key)
    {
        if (!Exists(key))
        {
            return;
        }

        s_data.Remove(key!);
    }

    public static void Rename(string fromName, string toName)
    {
        if (!Exists(fromName))
        {
            return;
        }

        var value = s_data[fromName];
        s_data.Remove(fromName);
        s_data[toName] = value;
    }
}
