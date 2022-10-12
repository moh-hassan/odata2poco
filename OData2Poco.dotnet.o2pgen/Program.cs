// Copyright (c) Mohamed Hassan & Contributors. All rights reserved. See License.md in the project root for license information.

using OData2Poco.CommandLine;

namespace OData2Poco.dotnet.o2pgen;

internal static class Program
{
    public static async Task Main(string[] args)
    {
        await StartUp.Run(args);
    }
}