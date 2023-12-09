// Copyright (c) Mohamed Hassan & Contributors. All rights reserved. See License.md in the project root for license information.

using OData2Poco.Extensions;
using OData2Poco.TextTransform;

namespace OData2Poco.TypeScript;

internal class TsPocoGenerator : IPocoClassGeneratorMultiFiles
{
    protected IPocoGenerator PocoGen;

    public TsPocoGenerator(IPocoGenerator pocoGen, PocoSetting setting)
    {
        PocoSetting = setting;
        PocoGen = pocoGen;
        ClassList = pocoGen.GeneratePocoList();
        Template = new FluentTextTemplate();
        Template.WriteLine(GetHeader());
        ModelStore = [];
        PocoSetting.Validate();
    }

    public PocoSetting PocoSetting { get; set; }
    public List<ClassTemplate> ClassList { get; set; }
    public FluentTextTemplate Template { get; }
    public PocoStore ModelStore { get; set; }

    public PocoStore GeneratePoco()
    {
        BuildModel();
        return ModelStore;
    }

    private void BuildModel()
    {
        ClassList.Sort();
        var groups = ClassList.GroupBy(x => x.NameSpace);
        foreach (var group in groups)
            if (PocoSetting.MultiFiles)
                WriteClassesToPocoStore(group);
            else
                WriteClasses(group.ToList(), group.Key);
    }

    private void WriteClasses(List<ClassTemplate> classList, string ns)
    {
        BeginNamespace(ns);
        foreach (var ct in classList)
        {
            var classBuilder = new TsClassBuilder(ct, PocoSetting);
            var code = classBuilder.WriteClassOrEnum(ct).ToString();
            Template.WriteLine(code);
        }

        EndNamespace(ns);
        Template.SaveToPocoStore(ModelStore);
    }

    private void WriteClassesToPocoStore(IEnumerable<ClassTemplate> classList)
    {
        classList.ToList().Sort();
        if (!PocoSetting.MultiFiles) return;
        foreach (var ct in classList)
        {
            var classBuilder = new TsClassBuilder(ct, PocoSetting);
            var code = classBuilder.WriteClassOrEnum(ct).ToString();
            FluentTextTemplate t = new();
            t.WriteLine(GetHeader())
                .WriteLine(ct.GetImports(classList, PocoSetting).ToString())
                .WriteLine(code)
                .SaveToPocoStor(ModelStore, ct.Name, ct.NameSpace, ct.FullName);
        }
    }

    private void BeginNamespace(string? ns)
    {
        if (PocoSetting.MultiFiles || string.IsNullOrEmpty(ns))
            return;
        Template.WriteLine($"export namespace {ns.RemoveDot()} {{");
        Template.PushIndent("\t");
    }

    private void EndNamespace(string? ns)
    {
        if (PocoSetting.MultiFiles || string.IsNullOrEmpty(ns))
            return;
        Template.PopIndent();
        Template.WriteLine("}");
    }

    private string GetHeader()
    {
        return CodeHeader.GetHeader(PocoGen, false);
    }
}