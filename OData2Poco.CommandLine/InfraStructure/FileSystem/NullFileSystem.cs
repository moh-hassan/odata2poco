using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using OData2Poco.CommandLine.InfraStructure.Logging;

namespace OData2Poco.CommandLine.InfraStructure.FileSystem
{
    public class NullFileSystem : IPocoFileSystem
    {
        static readonly ILog Logger = ColoredConsole.Default;
        public NullFileSystem()
        {
            Logger.Info($"Creating Null File System for test");
        }
        public void SaveToFile(string fname, string content)
        {
            Logger.Info($"Save To Null File System for test {fname} ");
        }

        public void SaveToFile(string filePath, Func<Stream> getStream)
        {

        }

        public void SaveToFolder(string folderPath, Dictionary<string, string> content)
        {
            Logger.Info($"Save To Null Folder System for test {folderPath} ");
        }

        public void SaveToFile(string filePath, string content, Encoding encoding)
        {
             
        }
         
    }
}