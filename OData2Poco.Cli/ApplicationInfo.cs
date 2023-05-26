// Copyright (c) Mohamed Hassan & Contributors. All rights reserved. See License.md in the project root for license information.

using System.Reflection;

namespace OData2Poco.CommandLine;

internal static class ApplicationInfo
{
    public static  string Title => Utility.GetAssemblyAttribute<AssemblyTitleAttribute>(a => a.Title);
    public static   string Author => "Mohamed Hassan & Contributers";
    public static string Product => Utility.GetAssemblyAttribute<AssemblyProductAttribute>(a => a.Product);

    public static string Copyright => Utility.GetAssemblyAttribute<AssemblyCopyrightAttribute>(a => a.Copyright);

    public static string Version => Utility.GetAssemblyAttribute<AssemblyInformationalVersionAttribute>(a => a.InformationalVersion);

    public static string Description => Utility.GetAssemblyAttribute<AssemblyDescriptionAttribute>(a => a.Description);
    public static string HeadingInfo => $"{Title} Version {Version}";

}