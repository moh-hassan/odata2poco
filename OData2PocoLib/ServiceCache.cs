#nullable disable
using System;
using System.Collections.Generic;
using System.Text;

namespace OData2Poco
{
    /// <summary>
    /// Singletone container 
    /// </summary>
    internal class ServiceCache
    {
        private static readonly Lazy<ServiceCache> Lazy = new Lazy<ServiceCache>(() =>
            new ServiceCache());
        public MetaDataInfo MetaData => PocoGenerator?.MetaData;
        public List<ClassTemplate> ClassList { get; set; }
        private IPocoGenerator PocoGenerator { get; set; }
        public static ServiceCache Default => Lazy.Value;

        private ServiceCache()
        {

        }

        public void AddGenerator(IPocoGenerator pocoGenerator, PocoSetting setting)
        {
            PocoGenerator = pocoGenerator;
            ClassList = pocoGenerator.GeneratePocoList();
        }

    }
}

#nullable restore
