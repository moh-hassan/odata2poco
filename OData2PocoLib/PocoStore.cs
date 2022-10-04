using OData2Poco.Extensions;
using OData2Poco.InfraStructure.FileSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace OData2Poco
{
    public class PocoStore : IEnumerable<PocoStoreEntry>
    {
        private readonly List<PocoStoreEntry> Entries = new();

        public int Count => Entries.Count;
        public PocoStoreEntry? this[string fullName]
        {
            get
            {
                return Entries.FirstOrDefault(a => a.FullName == fullName);
            }
        }

        public PocoStoreEntry? this[int id]
        {
            get
            {
                if (id >= 0 && id <= Entries.Count - 1)
                    return Entries[id];
                return null;
            }
        }

        public void Add(PocoStoreEntry row) => Entries.Add(row);
        //destination can be folderName when multFile =true otherwise it is filename 
        public void Save(string destination, IPocoFileSystem fileSystem, bool multFile)
        {
            //single file
            if (!multFile)
            {
                var code = Entries[0].Code ?? "";
                fileSystem.SaveToFile(destination, code);
                return;
            }
            //multi files
            foreach (var entry in Entries)
                entry.Save(destination, fileSystem);
        }
        public void Save(string destination, bool multFile)
        {
            IPocoFileSystem fileSystem = new PocoFileSystem();
            Save(destination, fileSystem, multFile);
        }
        public StringBuilder Display()
        {
            StringBuilder sb = new StringBuilder();
            if (Count==1)
            {
                sb.AppendLine(Entries[0].Code);
                return sb;
            }
            foreach (var entry in Entries)
            {
                sb.AppendLine($"//{entry.FileName}");                 
                sb.AppendLine(entry.Code);
            }
            return sb;
        }
        public IEnumerator<PocoStoreEntry> GetEnumerator() => Entries.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => Entries.GetEnumerator();
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
        // public string FileName => $"{FullName.Replace(".", "-")}.ts";
        public string FileName => $"{Name}.ts";
        public void Save(string folderName, IPocoFileSystem fileSystem)
        {
            var path = Path.Combine(folderName, Namespace.RemoveDot(), FileName);
            fileSystem.SaveToFile(path, Code);
        }
        public override string ToString() => Code;
    }
#nullable restore
}
