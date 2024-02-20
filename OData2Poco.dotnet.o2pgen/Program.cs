// Copyright (c) Mohamed Hassan & Contributors. All rights reserved. See License.md in the project root for license information.

#pragma warning disable IDE1006 // Naming Styles

namespace OData2Poco.dotnet.o2pgen;

using CommandLine;

internal static class Program
{
    public static Task Main(string[] args)
    {
        return StartUp.RunAsync(args);
    }
}

#pragma warning restore IDE1006 // Naming Styles
