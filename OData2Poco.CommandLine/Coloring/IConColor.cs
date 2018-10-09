using System;

namespace OData2Poco.Coloring
{
    public interface IConColor
    {
        void WriteLine(ConsoleColor foreColor, string format, params object[] arg);
        void WriteLine(ConsoleColor foreColor, ConsoleColor backColor, string format, params object[] arg);
        void Warning(string format, params object[] arg);
        void Info(string format, params object[] arg);
        void Error(string format, params object[] arg);
        void Sucess(string format, params object[] arg);
        void Confirm(string format, params object[] arg);
    }
}