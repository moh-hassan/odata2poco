// Copyright 2016-2022 Mohamed Hassan & Contributors. All rights reserved. See License.md in the project root for license information.

using OData2Poco.TextTransform;

namespace OData2Poco.TypeScript
{
    public class TsClassBuilder : FluentTextTemplate<TsClassBuilder>
    {
        private const string Lb = "{";
        private const string Rb = "}";
        private readonly ClassTemplate _classTemplate;
        public PocoSetting Setting { get; set; }         
        internal NamingConvention _namingConvention;
        public TsClassBuilder(ClassTemplate ct, PocoSetting? seetting)
        {
            _classTemplate = ct; 
            Setting = seetting ?? new PocoSetting
            {
                Lang = Language.TS,
                NameCase = CaseEnum.Camel,
                GeneratorType = GeneratorType.Interface,
            };             
            _namingConvention = new NamingConvention(ct, Setting);
        }

        public TsClassBuilder WriteProperty(PropertyTemplate p)
        {
            PushIndent("\t")
            .WriteLine(new TsPropertyBuilder(p, this))
            .PopIndent();
            return this;
        }

        public TsClassBuilder WriteProperties()
        {
            foreach (var p in _classTemplate.Properties)
            {
                WriteProperty(p);
            }
            return this;
        }
        public TsClassBuilder WriteEnumElements() => WriteLineFor(_classTemplate.EnumElements);

        public TsClassBuilder WriteEnum()
        {
            // WriteLine($"export enum {_classTemplate.GlobalName(Setting)} {Lb}")
            WriteLine($"export enum {_namingConvention.ClassName} {Lb}")
            .WriteEnumElements()
            .WriteLine(Rb);
            return this;
        }
       
        public TsClassBuilder WriteClass()
        {
            var extend = string.IsNullOrEmpty(_classTemplate.BaseType)
                ? "" : $"extends {_namingConvention.BaseType}";
            var type = Setting.GeneratorType == GeneratorType.Interface
                ? "interface" : "class";           
            WriteLine($"export {type} {_namingConvention.ClassName} {extend} {Lb}")
            .WriteProperties()
            .WriteLine(Rb);
             
            return this;
        }
        public TsClassBuilder WriteClassOrEnum(ClassTemplate c) =>
            c.IsEnum ? WriteEnum() : WriteClass();
         
    }
}
