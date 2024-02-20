// Copyright (c) Mohamed Hassan & Contributors. All rights reserved. See License.md in the project root for license information.

namespace OData2Poco.InfraStructure.Logging;

using System.Text;

public class ColoredConsole : ILog
{
    private readonly object _colorLock = new();
    public ConsoleColor ErrorColor { get; } = ConsoleColor.Red;
    public ConsoleColor InfoColor { get; } = ConsoleColor.Cyan;
    public ConsoleColor SuccessColor { get; } = ConsoleColor.Green;
    public ConsoleColor TraceColor { get; } = ConsoleColor.DarkGray;
    public ConsoleColor WarningColor { get; } = ConsoleColor.Yellow;

    public StringBuilder Output { get; set; } = new();
    public bool Silent { get; set; }

    public void Clear()
    {
        Output.Clear();
    }

    public void Debug(string msg)
    {
        Log(TraceColor, $"Debug: {msg}");
    }

    public void Trace(string msg)
    {
        Log(TraceColor, $"Trace: {msg}");
    }

    public void Warn(string msg)
    {
        Log(WarningColor, $"Warning: {msg}");
    }

    public void Warn(Func<string> message)
    {
        if (message != null)
        {
            Warn(message.Invoke());
        }
    }

    public void Info(string msg)
    {
        Log(InfoColor, $"Information: {msg}");
    }

    public void Info(Func<string> message)
    {
        if (message != null)
        {
            Info(message.Invoke());
        }
    }

    public void Error(string msg)
    {
        Log(ErrorColor, $"Error: {msg}");
    }

    public void Error(Func<string> message)
    {
        if (message != null)
        {
            Error(message.Invoke());
        }
    }

    public void Fatal(string msg)
    {
        Log(ErrorColor, $"Fatal Error: {msg}");
    }

    public void Success(string msg)
    {
        Log(SuccessColor, msg);
    }

    public void Confirm(string msg)
    {
        Log(ConsoleColor.White, ConsoleColor.Blue, msg);
    }

    public void Normal(string msg)
    {
        Log(ConsoleColor.Gray, ConsoleColor.Black, msg);
    }

    public void SetTheme(Action<ColoredConsole> action)
    {
        action?.Invoke(this);
    }

    public void Log(ConsoleColor foreColor, string msg)
    {
        lock (_colorLock)
        {
            Console.ForegroundColor = foreColor;
            if (!Silent)
            {
                Console.WriteLine(msg);
            }

            Console.ResetColor();
            Output.AppendLine(msg);
        }
    }

    public void Log(ConsoleColor foreColor, ConsoleColor backColor, string msg)
    {
        lock (_colorLock)
        {
            Console.BackgroundColor = backColor;
            Console.ForegroundColor = foreColor;
            if (!Silent)
            {
                Console.WriteLine(msg);
            }

            Console.ResetColor();
            Output.AppendLine(msg);
        }
    }
}
