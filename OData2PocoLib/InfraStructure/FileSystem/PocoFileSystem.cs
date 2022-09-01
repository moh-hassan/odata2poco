using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using OData2Poco.InfraStructure.Logging;

namespace OData2Poco.InfraStructure.FileSystem
{
    public class PocoFileSystem : IPocoFileSystem
    {
        static readonly ILog Logger = PocoLogger.Default;
        public void SaveToFile(string filePath, Func<Stream> getStream)
        {
            using (Stream incomingStream = getStream())
            using (Stream fileStream = File.Open(filePath, FileMode.CreateNew, FileAccess.Write))
            {
                incomingStream.CopyTo(fileStream);
                fileStream.Close();
            }
        }

        public void SaveToFolder(string folderPath, Dictionary<string, string> codeList)
        {
            if (File.Exists(folderPath))
            {
                var msg = $"Cannot create folder '{folderPath}' because a file with the same name already exists.";
                Logger.Error(msg);
                return;
            }

            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);
            foreach (var entry in codeList)
            {
                var fname = Path.Combine(folderPath, $"{entry.Key}.ts");
                var code = entry.Value;
                SaveToFile(fname, code);
            }
        }

        public void SaveToFile(string filePath, string content)
        {
            try
            {
                SaveToFile(filePath, content, Encoding.UTF8);
                Logger.Info($"Success saving to {filePath}");
            }
            catch (Exception e)
            {
                Logger.Error($"Fail to save file: {filePath}");
                Logger.Error(e.Message);
            }
        }

        public void SaveToFile(string filePath, string content, Encoding encoding)
        {
            var path = Path.GetFullPath(filePath);            
            //create directory if not exist
            new FileInfo(path)?.Directory?.Create();
            using (FileStream fileStream = File.Open(path, FileMode.Create, FileAccess.Write))
            using (var streamWriter = new StreamWriter(fileStream, encoding))
            {
                streamWriter.Write(content);
                streamWriter.Flush();
                streamWriter.Close();
                fileStream.Close();
            }
        }

    }
}