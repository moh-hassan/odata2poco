using System;
using System.Collections.Generic;
using System.Text;

namespace OData2Poco.InfraStructure.Logging
{
    public class PocoLogger : ColoredConsole
    {
        private static readonly Lazy<ILog> Lazy = new Lazy<ILog>(() => new PocoLogger());
        public static ILog Default => Lazy.Value;

        private PocoLogger()
        {

        }
    }
}
