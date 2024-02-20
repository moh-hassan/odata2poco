// Copyright (c) Mohamed Hassan & Contributors. All rights reserved. See License.md in the project root for license information.

namespace OData2Poco.CommandLine;

public static class Program
{
    public static Task Main(string[] args)
    {
        return StartUp.RunAsync(args);
    }
}
