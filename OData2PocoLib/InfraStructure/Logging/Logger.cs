using System;
using System.Collections.Generic;
using System.Text;

namespace OData2Poco.InfraStructure.Logging
{
  public  class PocoLogger: ColoredConsole
    {
        private static readonly Lazy<PocoLogger> Lazy = new Lazy<PocoLogger>(() => new PocoLogger());
        public static ColoredConsole Default => Lazy.Value;

        private PocoLogger()
        {
            
        }
    }
}
