// Copyright (c) Mohamed Hassan & Contributors. All rights reserved. See License.md in the project root for license information.

using System.Reflection;

namespace OData2Poco;

internal static class SystemConst
{
    public static string BaseDirectory
    {
        get
        {
            var assembly = Assembly.GetExecutingAssembly();
            var codebase = new Uri(assembly.Location);
            var path = codebase.LocalPath;
            return Path.GetDirectoryName(path) ?? string.Empty;
        }
    }
    public static string O2pGenPath => Path.GetFullPath( Path.Combine(BaseDirectory,"..","..", "o2pgen.exe"));
   
}
