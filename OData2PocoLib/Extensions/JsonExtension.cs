// Copyright (c) Mohamed Hassan & Contributors. All rights reserved. See License.md in the project root for license information.

namespace OData2Poco.Extensions;

using Newtonsoft.Json;

public static class JsonExtension
{
    public static string ToJson(this object data)
    {
        JsonSerializerSettings config = new()
        {
            DefaultValueHandling = DefaultValueHandling.Ignore,
            NullValueHandling = NullValueHandling.Ignore,
            PreserveReferencesHandling = PreserveReferencesHandling.None,
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
        };

        var json = JsonConvert.SerializeObject(data, Formatting.Indented, config);
        return json;
    }

    public static T? ToObject<T>(this string json)
    {
        var results = JsonConvert.DeserializeObject<T>(json);
        return results;
    }
}
