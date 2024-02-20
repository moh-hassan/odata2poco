// Copyright (c) Mohamed Hassan & Contributors. All rights reserved. See License.md in the project root for license information.

namespace OData2Poco.CommandLine;

using System.Reflection;

public static class Utility
{
    //http://stackoverflow.com/questions/3127288/how-can-i-retrieve-the-assemblycompany-setting-in-assemblyinfo-cs
    public static string GetAssemblyAttribute<T>(Func<T, string> value)
        where T : Attribute
    {
        _ = value ?? throw new ArgumentNullException(nameof(value));
        var attribute = (T)Attribute.GetCustomAttribute(Assembly.GetExecutingAssembly(), typeof(T))!;
        return attribute != null ? value.Invoke(attribute) : string.Empty;
    }
}
