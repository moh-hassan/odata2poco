using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace OData2Poco.CommandLine.InfraStructure.FileSystem
{
    public interface IPocoFileSystem
    {
        void SaveToFile(string fname, string content);
        void SaveToFile(string filePath, Func<Stream> getStream);
        void SaveToFolder(string folderPath, Dictionary<string,string> content);
        void SaveToFile(string filePath, string content, Encoding encoding);
        
    }
}
