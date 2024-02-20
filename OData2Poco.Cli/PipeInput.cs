// Copyright (c) Mohamed Hassan & Contributors. All rights reserved. See License.md in the project root for license information.

namespace OData2Poco.CommandLine;

internal static class Pipes
{
    public static string? ReadPipe()
    {
        var input = string.Empty;
        // if nothing is being piped in, then exit
        if (!IsPipedInput())
            return input;
        Console.WriteLine("Redirecting: read password");
        while (Console.In.Peek() != -1)
            input = Console.In.ReadLine();
        return input;
    }

    private static bool IsPipedInput()
    {
        try
        {
            _ = Console.KeyAvailable;
            return false;
        }
        catch
        {
            return true;
        }
    }
}
