// Copyright (c) Mohamed Hassan & Contributors. All rights reserved. See License.md in the project root for license information.

using System.Collections;
using System.Text;
using OData2Poco.Extensions;
using OData2Poco.InfraStructure.FileSystem;

namespace OData2Poco;

public class PocoStore : IEnumerable<PocoStoreEntry>
{
    private readonly List<PocoStoreEntry> _entries = new();

    public int Count => _entries.Count;

    public PocoStoreEntry? this[string fullName] =>
        _entries.FirstOrDefault(a => a.FullName == fullName);

    public PocoStoreEntry? this[int id]
    {
        get
        {
            if (id >= 0 && id <= _entries.Count - 1)
                return _entries[id];
            return null;
        }
    }

    public IEnumerator<PocoStoreEntry> GetEnumerator()
    {
        return _entries.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return _entries.GetEnumerator();
    }

    public void Add(PocoStoreEntry row)
    {
        _entries.Add(row);
    }

    //destination can be folderName when multFile =true otherwise it is filename 
    public void Save(string destination, IPocoFileSystem fileSystem, bool multFile)
    {
        //single file
        if (!multFile)
        {
            var code = _entries[0].Code ?? "";
            fileSystem.SaveToFile(destination, code);
            return;
        }

        //multi files
        foreach (var entry in _entries)
            entry.Save(destination, fileSystem);
    }

    public void Save(string destination, bool multFile)
    {
        IPocoFileSystem fileSystem = new PocoFileSystem();
        Save(destination, fileSystem, multFile);
    }

    public StringBuilder Display()
    {
        StringBuilder sb = new();
        if (Count == 1)
        {
            sb.AppendLine(_entries[0].Code);
            return sb;
        }

        foreach (var entry in _entries)
        {
            sb.AppendLine($"//{entry.FileName}");
            sb.AppendLine(entry.Code);
        }

        return sb;
    }

    public override string ToString()
    {
        return Display().ToString();
    }
}

#nullable disable
public class PocoStoreEntry
{
    public string Name { get; set; } = "poco";
    public string Namespace { get; set; } = string.Empty;
    public string Code { get; set; }
    public string FullName { get; set; } = "poco";
    public string FileName => $"{Name}.ts";

    public void Save(string folderName, IPocoFileSystem fileSystem)
    {
        var path = Path.Combine(folderName, Namespace.RemoveDot(), FileName);
        fileSystem.SaveToFile(path, Code);
    }

    public override string ToString()
    {
        return Code;
    }
}
#nullable restore