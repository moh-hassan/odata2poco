// Copyright (c) Mohamed Hassan & Contributors. All rights reserved. See License.md in the project root for license information.

namespace OData2Poco.CommandLine;

internal static class Pipes
{
    public static string ReadPipe()
    {
        if (!Console.IsInputRedirected) return "";
        string input = "";
        // if nothing is being piped in, then exit
        if (!IsPipedInput())
            return input;

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