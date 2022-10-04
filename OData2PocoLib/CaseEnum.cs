// Copyright 2016-2022 Mohamed Hassan & Contributors. All rights reserved. See License.md in the project root for license information.

namespace OData2Poco
{

    /// <summary>
    /// Writing words in CamelCase/ PascalCase or none (as is)
    /// </summary>
    public enum CaseEnum
    {       
        None, //No change
        Pas, //PascalCase
        Camel , //CamelCase
        Kebab,
        Snake
    }
}
