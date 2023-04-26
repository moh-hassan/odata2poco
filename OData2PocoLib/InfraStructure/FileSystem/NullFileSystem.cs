// Copyright (c) Mohamed Hassan & Contributors. All rights reserved. See License.md in the project root for license information.

using System.Text;

namespace OData2Poco.InfraStructure.FileSystem;

public class NullFileSystem : IPocoFileSystem
{
    public NullFileSystem()
    {
    }
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
        if (string.IsNullOrWhiteSpace(filePath))
            return false;
        return Fakes.Exists(filePath);

    }

    public string ReadAllText(string? filePath)
    {
        if (!Exists(filePath))
            return "";
        return Fakes.Get(filePath!);
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