using System;
using System.Text;

namespace OData2Poco.CommandLine.InfraStructure.Logging
{
    public interface ILog
    {
        StringBuilder Output { get; set; }
        void Debug(string msg);
        void Warn(string msg);
        void Warn(Func<string> message);
        void Info(string msg);
        void Info(Func<string> message);
        //void Trace(string msg);
        void Error(string msg);
        void Error(Func<string> message);
        void Fatal(string msg);
        void Sucess(string msg);
        void Confirm(string msg);
        void Clear();

    }
}