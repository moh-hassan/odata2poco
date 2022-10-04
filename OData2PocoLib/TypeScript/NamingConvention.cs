// Copyright 2016-2022 Mohamed Hassan & Contributors. All rights reserved. See License.md in the project root for license information.

using OData2Poco.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OData2Poco.TypeScript
{
    internal class NamingConvention
    {       
        public PocoSetting _setting { get; }       
        public string ClassName { get; }
        public string BaseType { get; } = "";
        public NamingConvention(ClassTemplate ct, PocoSetting setting)
        {
            _setting = setting;
            ClassName = setting.UseFullName ? ct.FullName.RemoveDot() : ct.Name;
            if (!string.IsNullOrEmpty(ct.BaseType))
                BaseType = setting.UseFullName
                    ? ct.BaseType.RemoveDot()
                    : ct.BaseType.Reduce();
        }
        public string GetPropertyType(string propType)
        {
            return _setting.UseFullName
                ? propType.ToTypeScript(_setting).RemoveDot()
                : propType.ToTypeScript(_setting).Reduce();
        }
    }
}
