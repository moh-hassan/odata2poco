// Copyright (c) Mohamed Hassan & Contributors. All rights reserved. See License.md in the project root for license information.

namespace OData2Poco.TypeScript;

using Extensions;

internal sealed class TsPropertyBuilder : SimpleTemplate<TsPropertyBuilder>
{
    private readonly NamingConvention _namingConvention;

    public TsPropertyBuilder(PropertyTemplate property, TsClassBuilder tsClassBuilder)
    {
        Property = property;
        Setting = tsClassBuilder.Setting;
        _namingConvention = tsClassBuilder._namingConvention;
    }

    private PropertyTemplate Property { get; }
    private PocoSetting Setting { get; }

    public static implicit operator string(TsPropertyBuilder builder)
    {
        return builder.ToString();
    }

    public override string ToString()
    {
        return Generate();
    }

    protected override void Build()
    {
        PreComment()
            .AccessLevel()
            .PropertyName()
            .Colon()
            .PropertyType()
            .LineTerminator()
            .TailComment();
    }

    private TsPropertyBuilder AccessLevel()
    {
        var visible = Setting.GeneratorType == GeneratorType.Interface ? string.Empty : "public ";
        return AddText(visible);
    }

    private TsPropertyBuilder PreComment()
    {
        var preComment = Property.IsNavigate && !Setting.AddNavigation ? "// " : string.Empty;
        return AddText(preComment);
    }

    private void TailComment()
    {
        AddText(Property.TailComment());
    }

    private TsPropertyBuilder PropertyName()
    {
        var propName = Property.PropName.ChangeCase(Setting.NameCase);
        if (Setting.EnableNullableReferenceTypes)
        {
            propName = propName.ToNullable(Property.IsNullable);
        }
        else if (Setting.AddNullableDataType && Property.IsNullable)
        {
            propName = propName.ToNullable(Property.IsNullable);
        }

        return AddText(propName);
    }

    private TsPropertyBuilder PropertyType()
    {
        var type = _namingConvention.GetPropertyType(Property.PropType);

        return AddText(type);
    }
}
