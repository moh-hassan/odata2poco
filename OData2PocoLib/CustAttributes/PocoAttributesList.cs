using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using OData2Poco.Extensions;
using OData2Poco.InfraStructure.Logging;

namespace OData2Poco.CustAttributes
{
    public class PocoAttributesList : IEnumerable<INamedAttribute>
    {
        private readonly ILog _logger = PocoLogger.Default;
        readonly List<INamedAttribute> _namedAttributes; //= new List<ICustomeAttribute>();
        public INamedAttribute this[string index] => GetAttributeObject(index);

        public PocoAttributesList()
        {
            _namedAttributes = new List<INamedAttribute>();
            FillNamedAttributes();
            LoadPluginAttributes();
        }
        public List<string> SupportedAttributes()
        {
            return _namedAttributes.Select(x => x.Name).ToList();
        }
        public IEnumerator<INamedAttribute> GetEnumerator()
        {
            return _namedAttributes.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public INamedAttribute GetAttributeObject(string attName)
        {
            return _namedAttributes.FirstOrDefault(x => x.Name == attName);
        }

        private void FillNamedAttributes()
        {

            var asm = typeof(INamedAttribute).GetTypeInfo().Assembly;
            var types = asm.DefinedTypes
                  .Where(x => x.ImplementedInterfaces.Contains(typeof(INamedAttribute)));

            foreach (var type in types)
            {
                var item = (INamedAttribute)Activator.CreateInstance(type);

                _namedAttributes.Add(item);

            }

        }

        public void LoadPluginAttributes()
        {

            string foldr =Parameters.PluginPath;
            if (string.IsNullOrEmpty(foldr)) return;
            foldr = Path.GetFullPath(foldr);
            if (!Directory.Exists(foldr)) return;
            var pluginList = Helper.LoadPlugin<INamedAttribute>(foldr)
                .Cast<INamedAttribute>().ToList();

            if (!pluginList.Any()) return;
            _logger.Trace($"Loading Plugin folder={foldr}-found ({pluginList.Count})");
            _namedAttributes.AddRange(pluginList);

        }

    }
}
