using System;
using System.Text;

namespace OData2Poco.CommandLine.InfraStructure.Logging
{
    public class ColoredConsole : ILog
    {
        private static readonly Lazy<ColoredConsole> Lazy = new Lazy<ColoredConsole>(() => new ColoredConsole());
        public static ColoredConsole Default => Lazy.Value;
        private readonly object _colorLock = new object();
        public ConsoleColor WarningColor = ConsoleColor.Yellow;
        public ConsoleColor InfoColor = ConsoleColor.Cyan;
        public ConsoleColor ErrorColor = ConsoleColor.Red;
        public ConsoleColor SucessColor = ConsoleColor.Green;
        public StringBuilder Output { get; set; }
        private ColoredConsole()
        {
            Output = new StringBuilder();
        }

        public void Clear()
        {
            Output.Clear();
        }
        public void SetTheme(Action<ColoredConsole> action)
        {
            action(this);
        }
       

        public void Log(ConsoleColor foreColor, string msg)
        {
            lock (_colorLock)
            {
                Console.ForegroundColor = foreColor; //ConsoleColor.Yellow;
                Console.WriteLine(msg);
                Console.ResetColor();
                Output.AppendLine(msg);
            }
        }

        public void Log(ConsoleColor foreColor, ConsoleColor backColor, string msg)
        {
            lock (_colorLock)
            {
                Console.BackgroundColor = backColor;
                Console.ForegroundColor = foreColor; //ConsoleColor.Yellow;
                Console.WriteLine(msg);
                Console.ResetColor();
                Output.AppendLine(msg);
            }
        }

        public void Debug(string msg)
        {
            //throw new NotImplementedException();
        }

        public void Warn(string msg)
        {
            Log(WarningColor, msg);
        }
        public void Warn(Func<string> message)
        {
            Warn(message.Invoke());
        }
        public void Info(string msg)
        {
            Log(InfoColor, msg);
        }

        public void Info(Func<string> message)
        {
            Info(message.Invoke());
        }

        public void Error(string msg)
        {
            Log(ErrorColor, msg);
        }
        public void Error(Func<string> message)
        {
            Error(message.Invoke());
        }
        public void Fatal(string msg)
        {
            //<highlight-row condition="level == LogLevel.Fatal" foregroundColor="Red" backgroundColor="White" />
        }

        public void Sucess(string msg)
        {
            Log(SucessColor, msg);
        }

        public void Confirm(string msg)
        {
            Log(ConsoleColor.White, ConsoleColor.Blue, msg);
        }
        public void Normal(string msg)
        {
            Log(ConsoleColor.Gray, ConsoleColor.Black, msg);
        }
    }
}

/*
 *  // We want to save the current console color and background color so we can restore it later
            ConsoleColor oldColor = Console.ForegroundColor;
            ConsoleColor oldBackgroundColor = Console.BackgroundColor;    
            .....

    // Restore the old foreground color and background color
            Console.ForegroundColor = oldColor;
            Console.BackgroundColor = oldBackgroundColor;
    */
