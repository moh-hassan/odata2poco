// Copyright (c) Mohamed Hassan & Contributors. All rights reserved. See License.md in the project root for license information.

namespace OData2Poco.InfraStructure.FileSystem;

using System.Text;

public interface IPocoFileSystem
{
    void SaveToFile(string filePath, string content);
    void SaveToFile(string filePath, Func<Stream> getStream);
    void SaveToFile(string filePath, string content, Encoding encoding);
    void SaveToFolder(string folderPath, Dictionary<string, string> content);
    bool Exists(string? filePath);
    string ReadAllText(string? filePath);
    void WriteAllText(string filePath, string content);
    void Delete(string path);
    void Rename(string fromName, string toName);
}
