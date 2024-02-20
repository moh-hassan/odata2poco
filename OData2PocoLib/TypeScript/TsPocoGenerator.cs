// Copyright (c) Mohamed Hassan & Contributors. All rights reserved. See License.md in the project root for license information.

namespace OData2Poco.TypeScript;

using Extensions;
using TextTransform;

internal class TsPocoGenerator : IPocoClassGeneratorMultiFiles
{
    protected IPocoGenerator _pocoGen;

    public TsPocoGenerator(IPocoGenerator pocoGen, PocoSetting setting)
    {
        PocoSetting = setting;
        _pocoGen = pocoGen;
        ClassList = pocoGen.GeneratePocoList();
        Template = new FluentTextTemplate();
        Template.WriteLine(GetHeader());
        ModelStore = [];
        PocoSetting.Validate();
    }

    public List<ClassTemplate> ClassList { get; set; }

    private PocoSetting PocoSetting { get; }
    private FluentTextTemplate Template { get; }
    private PocoStore ModelStore { get; }

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
        {
            if (PocoSetting.MultiFiles)
            {
                WriteClassesToPocoStore(group);
            }
            else
            {
                WriteClasses(group.ToList(), group.Key);
            }
        }
    }

    private void WriteClasses(List<ClassTemplate> classList, string ns)
    {
        BeginNamespace(ns);
        foreach (var ct in classList)
        {
            TsClassBuilder classBuilder = new(ct, PocoSetting);
            var code = classBuilder.WriteClassOrEnum(ct).ToString();
            Template.WriteLine(code);
        }

        EndNamespace(ns);
        Template.SaveToPocoStore(ModelStore);
    }

    private void WriteClassesToPocoStore(IEnumerable<ClassTemplate> classList)
    {
        var list = classList.ToList();
        list.Sort();
        if (!PocoSetting.MultiFiles)
        {
            return;
        }

        foreach (var ct in list)
        {
            TsClassBuilder classBuilder = new(ct, PocoSetting);
            var code = classBuilder.WriteClassOrEnum(ct).ToString();
            FluentTextTemplate t = new();
            t.WriteLine(GetHeader())
               .WriteLine(ct.GetImports(list, PocoSetting).ToString())
               .WriteLine(code)
               .SaveToPocoStor(ModelStore, ct.Name, ct.NameSpace, ct.FullName);
        }
    }

    private void BeginNamespace(string? ns)
    {
        if (PocoSetting.MultiFiles || string.IsNullOrEmpty(ns))
        {
            return;
        }

        Template.WriteLine($"export namespace {ns.RemoveDot()} {{");
        Template.PushIndent("\t");
    }

    private void EndNamespace(string? ns)
    {
        if (PocoSetting.MultiFiles || string.IsNullOrEmpty(ns))
        {
            return;
        }

        Template.PopIndent();
        Template.WriteLine("}");
    }

    private string GetHeader()
    {
        return CodeHeader.GetHeader(_pocoGen, false);
    }
}
