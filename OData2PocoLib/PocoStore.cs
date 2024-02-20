// Copyright (c) Mohamed Hassan & Contributors. All rights reserved. See License.md in the project root for license information.

namespace OData2Poco;

using System.Text;
using Extensions;
using InfraStructure.FileSystem;

public class PocoStore : IEnumerable<PocoStoreEntry>
{
    private readonly List<PocoStoreEntry> _entries = [];

    public int Count => _entries.Count;

    public PocoStoreEntry? this[string fullName]
        => _entries.Find(a => a.FullName == fullName);

    public PocoStoreEntry? this[int id] => id >= 0 && id <= _entries.Count - 1 ? _entries[id] : null;

    IEnumerator IEnumerable.GetEnumerator()
    {
        return _entries.GetEnumerator();
    }

    public IEnumerator<PocoStoreEntry> GetEnumerator()
    {
        return _entries.GetEnumerator();
    }

    public void Add(PocoStoreEntry row)
    {
        _entries.Add(row);
    }

    //destination can be folderName when multFile =true otherwise it is filename
    public void Save(string destination, IPocoFileSystem fileSystem, bool multiFile)
    {
        _ = destination ?? throw new ArgumentNullException(nameof(destination));
        _ = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));
        //single file
        if (!multiFile)
        {
            var code = _entries[0].Code ?? string.Empty;
            fileSystem.SaveToFile(destination, code);
            return;
        }

        //multi files
        foreach (var entry in _entries)
        {
            entry.Save(destination, fileSystem);
        }
    }

    public void Save(string destination, bool multiFile)
    {
        IPocoFileSystem fileSystem = new PocoFileSystem();
        Save(destination, fileSystem, multiFile);
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
        _ = folderName ?? throw new ArgumentNullException(nameof(folderName));
        _ = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));
        var path = Path.Combine(folderName, Namespace.RemoveDot(), FileName);
        fileSystem.SaveToFile(path, Code);
    }

    public override string ToString()
    {
        return Code;
    }
}
