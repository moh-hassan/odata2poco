// Copyright (c) Mohamed Hassan & Contributors. All rights reserved. See License.md in the project root for license information.

using System.Text;

namespace OData2Poco.InfraStructure.FileSystem;

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
}