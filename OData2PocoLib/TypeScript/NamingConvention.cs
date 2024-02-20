// Copyright (c) Mohamed Hassan & Contributors. All rights reserved. See License.md in the project root for license information.

namespace OData2Poco.TypeScript;

using Extensions;

internal class NamingConvention
{
    public NamingConvention(ClassTemplate ct, PocoSetting setting)
    {
        Setting = setting;
        ClassName = setting.UseFullName ? ct.FullName.RemoveDot() : ct.Name;
        if (!string.IsNullOrEmpty(ct.BaseType))
        {
            BaseType = setting.UseFullName
                ? ct.BaseType.RemoveDot()
                : ct.BaseType.Reduce();
        }
    }

    public PocoSetting Setting { get; }
    public string ClassName { get; }
    public string BaseType { get; } = string.Empty;

    public string GetPropertyType(string propType)
    {
        return Setting.UseFullName
            ? propType.ToTypeScript().RemoveDot()
            : propType.ToTypeScript().Reduce();
    }
}
