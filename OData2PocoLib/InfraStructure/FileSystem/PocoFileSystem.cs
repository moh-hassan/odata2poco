// Copyright (c) Mohamed Hassan & Contributors. All rights reserved. See License.md in the project root for license information.

namespace OData2Poco.InfraStructure.FileSystem;

using System.Text;
using Extensions;
using Logging;

public class PocoFileSystem : IPocoFileSystem
{
    private static readonly ILog s_logger = PocoLogger.Default;

    public void SaveToFile(string filePath, Func<Stream> getStream)
    {
        _ = filePath ?? throw new ArgumentNullException(nameof(filePath));
        _ = getStream ?? throw new ArgumentNullException(nameof(getStream));
        using var incomingStream = getStream();
        using Stream fileStream = File.Open(filePath, FileMode.CreateNew, FileAccess.Write);
        incomingStream.CopyTo(fileStream);
        fileStream.Close();
    }

    public void SaveToFile(string filePath, string content)
    {
        try
        {
            SaveToFile(filePath, content, Encoding.UTF8);
            s_logger.Info($"Success saving to {filePath}");
        }
        catch (Exception e)
        {
            s_logger.Error($"Fail to save file: {filePath}");
            s_logger.Error(e.Message);
        }
    }

    public void SaveToFile(string filePath, string content, Encoding encoding)
    {
        var path = Path.GetFullPath(filePath);
        //create directory if not exist
        new FileInfo(path).Directory?.Create();
        using var fileStream = File.Open(path, FileMode.Create, FileAccess.Write);
        using StreamWriter streamWriter = new(fileStream, encoding);
        streamWriter.Write(content);
        streamWriter.Flush();
        streamWriter.Close();
        fileStream.Close();
    }

    public void SaveToFolder(string folderPath, Dictionary<string, string> content)
    {
        if (File.Exists(folderPath))
        {
            var msg = $"Cannot create folder '{folderPath}' because a file with the same name already exists.";
            s_logger.Error(msg);
            return;
        }

        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        if (content == null) return;

        foreach (var (key, code) in content)
        {
            var fileName = Path.Combine(folderPath, $"{key}.ts");
            SaveToFile(fileName, code);
        }
    }

    public bool Exists(string? filePath)
    {
        return File.Exists(filePath);
    }

    public string ReadAllText(string? filePath)
    {
        return !Exists(filePath) ? string.Empty : File.ReadAllText(filePath!);
    }

    public void WriteAllText(string filePath, string content)
    {
        File.WriteAllText(filePath, content);
    }

    public void Delete(string path)
    {
        File.Delete(path);
    }

    public void Rename(string fromName, string toName)
    {
        File.Move(fromName, toName);
    }
}
