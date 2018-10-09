using System;

namespace OData2Poco.Coloring
{

    //Yellow: warning
    //Red: Error
    //Green: Pass, success messages
    //Cyan: Information

    public class ConColor : IConColor
    {
        private static readonly Lazy<ConColor> lazy =new Lazy<ConColor>(() => new ConColor());
        public static ConColor Default { get { return lazy.Value; } }
        private readonly object _colorLock = new object();
        public ConsoleColor WaningColor = ConsoleColor.Yellow;
        public ConsoleColor InfoColor = ConsoleColor.Cyan;
        public ConsoleColor ErrorColor = ConsoleColor.Red;
        public ConsoleColor SucessColor = ConsoleColor.Green;

        private ConColor()
        {
            
        }

        public void WriteLine(ConsoleColor foreColor, string format, params object[] arg)
        {
            lock (_colorLock)
            {
                //Console.BackgroundColor = ConsoleColor.White;
                Console.ForegroundColor = foreColor; //ConsoleColor.Yellow;
                Console.WriteLine(format, arg);
                Console.ResetColor();
            }
        }

        public void WriteLine(ConsoleColor foreColor, ConsoleColor backColor, string format, params object[] arg)
        {
            lock (_colorLock)
            {
                Console.BackgroundColor = backColor;
                Console.ForegroundColor = foreColor; //ConsoleColor.Yellow;
                Console.WriteLine(format, arg);
                Console.ResetColor();
            }
        }
        public void Warning(string format, params object[] arg)
        {
            ////Console.BackgroundColor = ConsoleColor.White;
            //Console.ForegroundColor = ConsoleColor.Yellow;
            //Console.WriteLine(format, arg);
            //Console.ResetColor();
            WriteLine(WaningColor, format, arg);
        }

        public void Info(string format, params object[] arg)
        {
            WriteLine(InfoColor, format, arg);
        }

        public void Error(string format, params object[] arg)
        {
            WriteLine(ErrorColor, format, arg);
        }

        public void Sucess(string format, params object[] arg)
        {
            WriteLine(SucessColor, format, arg);
        }
        //Console.BackgroundColor = ConsoleColor.White;
        //Console.ForegroundColor = ConsoleColor.Blue;
        public void Confirm(string format, params object[] arg)
        {
            WriteLine(ConsoleColor.White,ConsoleColor.Blue, format, arg);
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
