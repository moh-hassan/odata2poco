// Copyright (c) Mohamed Hassan & Contributors. All rights reserved. See License.md in the project root for license information.

namespace OData2Poco;

using System;
using System.Security;

public static class PasswordReader
{
    public static SecureString ReadPassword(string? prompt = null, bool showMask = true)
    {
        var stack = new Stack<char>();
        ConsoleKey key;
        var mask = "*";
        if (prompt != null) Console.Write(prompt);

        do
        {
            var keyInfo = Console.ReadKey(true);
            key = keyInfo.Key;
            if (keyInfo.Key == ConsoleKey.Backspace && stack.Count > 0)
            {
                //delete the asterisk character from the screen,
                if (showMask) Console.Write("\b \b");
                stack.Pop();
            }
            else if (!char.IsControl(keyInfo.KeyChar))
            {
                if (showMask) Console.Write(mask);
                stack.Push(keyInfo.KeyChar);
            }
        } while (key != ConsoleKey.Enter);
        var cs = new SecureString();
        foreach (var ch in stack.Reverse()) cs.AppendChar(ch);
        cs.MakeReadOnly();
        Console.WriteLine();
        return cs;

    }
}


