// Copyright (c) Mohamed Hassan & Contributors. All rights reserved. See License.md in the project root for license information.

namespace OData2Poco.InfraStructure.FileSystem;

using System.Text;

public class NullFileSystem : IPocoFileSystem
{
    public void SaveToFile(string filePath, string content)
    {
        //do nothing
    }

    public void SaveToFile(string filePath, Func<Stream> getStream)
    {
        //do nothing
    }

    public void SaveToFile(string filePath, string content, Encoding encoding)
    {
        //do nothing
    }

    public void SaveToFolder(string folderPath, Dictionary<string, string> content)
    {
        //do nothing
    }

    public bool Exists(string? filePath)
    {
        return !string.IsNullOrWhiteSpace(filePath) && Fakes.Exists(filePath);
    }

    public string ReadAllText(string? filePath)
    {
        return !Exists(filePath) ? throw new InvalidOperationException($"could not read file {filePath}") : Fakes.Get(filePath);
    }

    public void WriteAllText(string filePath, string content)
    {
        Fakes.Mock(filePath, content);
    }

    public void Delete(string path)
    {
        Fakes.Remove(path);
    }

    public void Rename(string fromName, string toName)
    {
        Fakes.Rename(fromName, toName);
    }
}
