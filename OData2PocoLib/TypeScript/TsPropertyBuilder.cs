// Copyright 2016-2022 Mohamed Hassan & Contributors. All rights reserved. See License.md in the project root for license information.

using OData2Poco.Extensions;

namespace OData2Poco.TypeScript
{
    internal sealed class TsPropertyBuilder : SimpleTemplate<TsPropertyBuilder>
    {
        private PropertyTemplate Property { get; }
        private PocoSetting Setting { get; }
        readonly TsClassBuilder _tsClassBuilder;
        readonly NamingConvention _namingConvention;
        public TsPropertyBuilder(PropertyTemplate property, TsClassBuilder tsClassBuilder)
        {
            Property = property;
            _tsClassBuilder = tsClassBuilder;
            Setting = tsClassBuilder.Setting;
            _namingConvention = tsClassBuilder._namingConvention;
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
            var visible = Setting.GeneratorType == GeneratorType.Interface ? "" : "public ";
            return AddText(visible);
        }
        private TsPropertyBuilder PreComment()
        {
            var preComment = Property.IsNavigate && !Setting.AddNavigation ? "// " : "";
            return AddText(preComment);
        }
        private TsPropertyBuilder TailComment()
        {
            return AddText(Property.TailComment());
        }

        private TsPropertyBuilder PropertyName()
        {
            var propName = Property.PropName.ChangeCase(Setting.NameCase);
            if (Setting.EnableNullableReferenceTypes)
                propName = propName.ToNullable(Property.IsNullable);
            else if (Setting.AddNullableDataType && Property.IsNullable)
                propName = propName.ToNullable(Property.IsNullable);
            return AddText(propName);
        }
        private TsPropertyBuilder PropertyType()
        {            
            var type = _namingConvention.GetPropertyType(Property.PropType); 
             
            return AddText(type);
        }

        public static implicit operator string(TsPropertyBuilder builder) =>
            builder.ToString();
        public override string ToString() => Generate();
    }
}

