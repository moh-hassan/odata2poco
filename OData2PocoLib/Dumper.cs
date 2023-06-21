// Copyright (c) Mohamed Hassan & Contributors. All rights reserved. See License.md in the project root for license information.

using Newtonsoft.Json;

namespace OData2Poco;

internal static class Dumper
{
    public static string Dump<T>(this T obj, string? title = null)
    {
        try
        {
            var json = JsonConvert.SerializeObject(obj, Formatting.Indented,
                new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                    //  PreserveReferencesHandling = PreserveReferencesHandling.Objects,
                    MaxDepth = 2
                });
            Console.WriteLine($"{title}\n{json}\n");
            return json;
        }
        catch (Exception e)
        {
            Console.WriteLine($"Can't dump {title}: {e.Message}");
        }
        return string.Empty;
    }
}