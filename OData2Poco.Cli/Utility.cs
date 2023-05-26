// Copyright (c) Mohamed Hassan & Contributors. All rights reserved. See License.md in the project root for license information.

using System.Reflection;

namespace OData2Poco.CommandLine;
#nullable disable
public static class Utility
{
    //http://stackoverflow.com/questions/3127288/how-can-i-retrieve-the-assemblycompany-setting-in-assemblyinfo-cs
    public static string GetAssemblyAttribute<T>(Func<T, string> value)
        where T : Attribute
    {
        T attribute = (T)Attribute.GetCustomAttribute(Assembly.GetExecutingAssembly(), typeof(T));
        return value.Invoke(attribute);
    }
}
#nullable restore