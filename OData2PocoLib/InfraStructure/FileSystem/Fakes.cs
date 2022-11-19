// Copyright (c) Mohamed Hassan & Contributors. All rights reserved. See License.md in the project root for license information.

using OData2Poco.Extensions;

namespace OData2Poco;

internal static class Fakes
{
    private static Dictionary<string, string> Data = new()
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
        Data[fileName] = content;
    }
    public static void Mock(string fileName, object obj)
    {
        var json = obj.ToJson();
        Data[fileName] = json;
    }
    public static string Get(string? fileName)
    {
        return fileName != null && Data.ContainsKey(fileName) ? Data[fileName] : string.Empty;
    }
    public static bool Exists(string? key)
    {
        return key != null && Data.ContainsKey(key);
    }
}