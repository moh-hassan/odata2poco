// Copyright (c) Mohamed Hassan & Contributors. All rights reserved. See License.md in the project root for license information.

namespace OData2Poco;

using CustAttributes;
using TextTransform;

/// <summary>
///     Generate c# code
/// </summary>
public sealed class PocoClassGeneratorCs : IPocoClassGenerator
{
    internal string _header;
    private readonly string _nl = Environment.NewLine;
    private readonly IPocoGenerator _pocoGen;

    //container for all classes
    /// <summary>
    ///     Constructor
    /// </summary>
    /// <param name="pocoGen"></param>
    /// <param name="setting"></param>
    private PocoClassGeneratorCs(IPocoGenerator pocoGen, PocoSetting setting)
    {
        PocoSetting = setting;
        _pocoGen = pocoGen;
        ClassList = pocoGen.GeneratePocoList();
        CodeText = string.Empty;
        _header = string.Empty;
        PocoSetting.Validate();
    }

    public bool BlankSpaceBeforeProperties { get; set; } = true;
    public string LangName { get; set; } = "csharp";
    public List<ClassTemplate> ClassList { get; set; }
    public PocoSetting PocoSetting { get; set; }
    private string? CodeText { get; set; }

    //key is fullName: <namespace.className>
    public ClassTemplate? this[string key] => ClassList.Find(x => x.FullName == key);

    public static PocoClassGeneratorCs GenerateCsPocoClass(IPocoGenerator pocoGen, PocoSetting? setting)
    {
        pocoGen = pocoGen ?? throw new ArgumentNullException(nameof(pocoGen));
        setting ??= new PocoSetting();
        //add jsonproperty to properties/classes that are renamed
        setting.Attributes.Add("original"); //v3.2

        //initialize AttributeFactory to use pocosetting.Attributes
        AttributeFactory.Default.Init(setting);

        PocoClassGeneratorCs generator = new(pocoGen, setting);
        var generatorClassList = generator.ClassList;

        //change case
        if (setting.EntityNameCase != CaseEnum.None || setting.RenameMap is not null)
        {
            ModelChangeCase.RenameClasses(generatorClassList, setting.EntityNameCase, setting.RenameMap);
        }

        //check reserved keywords
        ModelManager.RenameReservedWords(generatorClassList);
        generator._header = generator.GetHeader();
        return generator;
    }

    /// <summary>
    ///     Generate C# code for all POCO classes in the model
    /// </summary>
    /// <returns></returns>
    public string GeneratePoco()
    {
        if (ClassList.Count == 0)
        {
            return string.Empty;
        }

        var ns = ClassList.Select(x => x.NameSpace).Distinct()
            .OrderBy(x => x).ToList();
        FluentCsTextTemplate template = new(PocoSetting)
        {
            Header = _header
        };

        template.WriteLine(UsingAssembly(ns));
        foreach (var s in ns)
        {
            //Use a user supplied namespace prefix combined with the schema namepace or just the schema namespace

            var @namespace = PrefixNamespace(s);
            template.StartNamespace(@namespace);
            var pocoModel = ClassList.Where(x => x.NameSpace == s);
            foreach (var item in pocoModel)
            {
                template.WriteLine(ClassToString(item));
            }

            template.EndNamespace();
        }

        return template.ToString();
    }

    public override string ToString()
    {
        if (string.IsNullOrEmpty(CodeText))
        {
            CodeText = GeneratePoco();
        }

        return CodeText ?? string.Empty;
    }

    /// <summary>
    ///     Generate C# code for a given  Entity using FluentCsTextTemplate
    /// </summary>
    /// <param name="ent"> Class  to generate code</param>
    /// <param name="includeNamespace"></param>
    /// <returns></returns>
    internal string ClassToString(ClassTemplate ent, bool includeNamespace = false)
    {
        FluentCsTextTemplate csTemplate = new(PocoSetting);
        var comment = ent.GetComment();
        var visibility = PocoSetting.TypeVisibility ? "internal" : "public";
        if (comment.Length > 0)
        {
            csTemplate.PushIndent("\t").WriteLine("// " + ent.GetComment()).PopIndent();
        }

        //for enum
        if (ent.IsEnum)
        {
            var elements = string.Join($",{_nl}", ent.EnumElements);
            var flagAttribute = ent.IsFlags ? "[Flags] " : string.Empty;
            var enumString = $"\t{flagAttribute}{visibility} enum {ent.Name}{_nl}\t {{{_nl} {elements} {_nl}\t}}";
            return enumString;
        }

        foreach (var item in ent.GetAllAttributes()) //not depend on pocosetting
        {
            csTemplate.PushIndent("\t").WriteLine(item).PopIndent();
        }

        var baseClass = !string.IsNullOrEmpty(ent.BaseType) && PocoSetting.UseInheritance
            ? ReducedBaseTyp(ent) //ent.BaseType
            : PocoSetting.Inherit;
        csTemplate.StartClass(ent.Name, baseClass, visibility, ent.IsAbstrct);

        //add constructor
        if (PocoSetting.WithConstructor != Ctor.None)
        {
            var ctor = PropertyGenerator.GenerateFullConstructor(ent, PocoSetting);
            if (!string.IsNullOrEmpty(ctor))
                csTemplate.WriteLine(ctor);
        }

        //add properties
        csTemplate.WriteLine(PropertyGenerator.GenerateProperties(ent, PocoSetting));

        csTemplate.EndClass();
        if (includeNamespace)
        {
            csTemplate.EndNamespace();
        }

        CodeText = csTemplate.ToString();
        return CodeText;
    }

    internal string PrefixNamespace(string name)
    {
        var namespc = name;
        if (!string.IsNullOrWhiteSpace(PocoSetting.NamespacePrefix))
        {
            namespc = PocoSetting.NamespacePrefix + "." + name;
        }

        return namespc;
    }

    internal string ReducedBaseTyp(ClassTemplate ct)
    {
        var ns = $"{ct.NameSpace}.";
        var reducedName = ct.BaseType;
        if (ct.BaseType.StartsWith(ns))
        {
            reducedName = ct.BaseType.Replace(ns, string.Empty);
        }

        return reducedName;
    }

    private string GetHeader()
    {
        return CodeHeader.GetHeader(_pocoGen);
    }

    private string UsingAssembly(List<string> nameSpaces)
    {
        FluentCsTextTemplate h = new(PocoSetting);
        AssemblyManager assemblyManager = new(PocoSetting, ClassList);
        var assemblyList = assemblyManager._assemplyReference;
        foreach (var entry in assemblyList)
        {
            h.UsingNamespace(entry);
        }

        //add also namespaces of the built-in schema namespaces
        if (nameSpaces.Count > 1)
        {
            nameSpaces.ForEach(x =>
            {
                var namespc = PrefixNamespace(x);
                h.UsingNamespace(namespc);
            });
        }

        return h.ToString();
    }
}
