// Copyright (c) Mohamed Hassan & Contributors. All rights reserved. See License.md in the project root for license information.

namespace OData2Poco.TypeScript;

using Extensions;

internal static class TsTypeExtension
{
    //mapping c# type with typescript
    private static readonly Dictionary<string, string> s_tsDictionary = new()
    {
        //string
        ["string"] = "string",
        ["char"] = "string",
        ["Guid"] = "string",
        //number
        //Signed integral
        ["sbyte"] = "number",
        ["short"] = "number",
        ["int"] = "number",
        ["long"] = "number",
        //Unsigned integral
        ["byte"] = "number",
        ["ushort"] = "number",
        ["uint"] = "number",
        ["ulong"] = "number",
        //Floating point
        ["float"] = "number",
        ["double"] = "number",
        //decimal
        ["decimal"] = "number",

        //bool
        ["bool"] = "boolean",
        //date
        ["DateTime"] = "Date",
        ["DateTimeOffset"] = "Date",
        ["TimeSpan"] = "Date",
        //GEO complex types
        ["GeographyPoint"] = "number[]",
        ["Stream"] = "number[]",
        ["byte[]"] = "number[]"
    };

    internal static string ToTypeScript(this string csType)
    {
        return s_tsDictionary.TryGetValue(csType, out var value)
            ? value
            : csType.GenenericToArray();
    }

    //Convert List<T> to T[]
    internal static string GenenericToArray(this string propType)
    {
        const string ListPattern = "List[<](.+)[>]";
        var m = propType.MatchPattern(ListPattern);
        return m.Success ? m.Groups[1].Value + "[]" : propType;
    }
}
