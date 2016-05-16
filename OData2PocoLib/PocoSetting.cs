using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OData2Poco
{
    public class PocoSetting
    {
        public bool AddNullableDataType { get; set; }
        public bool AddKeyAttribute { get; set; }
        public bool AddTableAttribute { get; set; }
        public bool AddRequiredAttribute { get; set; }
        public bool AddNavigation { get; set; }
        //public bool AddDataContractAttribute { get; set; }
       
    }
}
