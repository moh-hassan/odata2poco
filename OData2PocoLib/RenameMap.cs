using System.Collections.Generic;

namespace OData2Poco
{
    public class RenameMap
    {
        // This is a list so we have the option to control how we match
        // the OldName.
        public List<ClassNameMap> ClassNameMap { get; set;  } = new List<ClassNameMap>();

        public Dictionary<string, List<PropertyNameMap>> PropertyNameMap { get; set; } = new Dictionary<string, List<PropertyNameMap>>();
    }

    public class ClassNameMap
    {
        public string OldName { get; set; } = string.Empty;

        public string NewName { get; set; } = string.Empty;
    }

    public class PropertyNameMap
    {
        public string OldName { get; set; } = string.Empty;

        public string NewName { get; set; } = string.Empty;
    }

}

