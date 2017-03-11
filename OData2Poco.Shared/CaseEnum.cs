using System;
using System.Collections.Generic;
using System.Text;

namespace OData2Poco.Shared
{

    /// <summary>
    /// Writing words in CamelCase/ PascalCase or none (as is)
    /// </summary>
    public enum CaseEnum
    {
       
        None, //No change
        Pas, //PascalCase
        Camel  //CamelCase
    }
}
