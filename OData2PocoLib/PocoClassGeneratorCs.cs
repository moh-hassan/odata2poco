// Copyright (c) Mohamed Hassan & Contributors. All rights reserved. See License.md in the project root for license information.

using OData2Poco.CustAttributes;
using OData2Poco.TextTransform;

namespace OData2Poco;

/// <summary>
///     Generate c# code
/// </summary>
internal class PocoClassGeneratorCs : IPocoClassGenerator
{
    private readonly string _nl = Environment.NewLine;
    private readonly IPocoGenerator _pocoGen;
    internal string Header;

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
        CodeText = "";
        Header = "";
        PocoSetting.Validate();
    }

    public bool BlankSpaceBeforeProperties { get; set; } = true;
    public string LangName { get; set; } = "csharp";

    private string? CodeText { get; set; }

    //key is fullName: <namespace.className>
    public ClassTemplate? this[string key] => ClassList.FirstOrDefault(x => x.FullName == key);
    public List<ClassTemplate> ClassList { get; set; }
    public PocoSetting PocoSetting { get; set; }

    /// <summary>
    ///     Generate C# code for all POCO classes in the model
    /// </summary>
    /// <returns></returns>
    public string GeneratePoco()
    {
        if (!ClassList.Any())
            return "";
        var ns = ClassList.Select(x => x.NameSpace).Distinct()
            .OrderBy(x => x).ToList();
        var template = new FluentCsTextTemplate(PocoSetting) { Header = Header };

        template.WriteLine(UsingAssembly(ns));
        foreach (var s in ns)
        {
            //Use a user supplied namespace prefix combined with the schema namepace or just the schema namespace

            var namespc = PrefixNamespace(s);
            template.StartNamespace(namespc);
            var pocoModel = ClassList.Where(x => x.NameSpace == s);
            foreach (var item in pocoModel) template.WriteLine(ClassToString(item));
            template.EndNamespace();
        }

        return template.ToString();
    }

    public static PocoClassGeneratorCs GenerateCsPocoClass(IPocoGenerator pocoGen, PocoSetting? setting)
    {
        setting ??= new PocoSetting();
        //add jsonproperty to properties/classes that are renamed
        setting.Attributes.Add("original"); //v3.2


        //initialize AttributeFactory to use pocosetting.Attributes
        AttributeFactory.Default.Init(setting);

        var generator = new PocoClassGeneratorCs(pocoGen, setting);
        var generatorClassList = generator.ClassList;

        //change case
        if (setting.EntityNameCase != CaseEnum.None || setting.RenameMap is not null)
            ModelChangeCase.RenameClasses(generatorClassList, setting.EntityNameCase, setting.RenameMap);

        //check reserved keywords
        ModelManager.RenameReservedWords(generatorClassList);
        generator.Header = generator.GetHeader();
        return generator;
    }

    public override string ToString()
    {
        if (string.IsNullOrEmpty(CodeText)) CodeText = GeneratePoco();
        return CodeText ?? string.Empty;
    }

    /// <summary>
    ///     Generte C# code for a given  Entity using FluentCsTextTemplate
    /// </summary>
    /// <param name="ent"> Class  to generate code</param>
    /// <param name="includeNamespace"></param>
    /// <returns></returns>
    internal string ClassToString(ClassTemplate ent, bool includeNamespace = false)
    {
        var csTemplate = new FluentCsTextTemplate(PocoSetting);

        ////for enum
        if (ent.IsEnum)
        {
            var elements = string.Join($",{_nl}", ent.EnumElements.ToArray());
            var flagAttribute = ent.IsFlags ? "[Flags] " : "";
            var enumString = $"\t{flagAttribute}public enum {ent.Name}{_nl}\t {{{_nl} {elements} {_nl}\t}}";
            return enumString;
        }


        foreach (var item in ent.GetAllAttributes()) //not depend on pocosetting
            csTemplate.PushIndent("\t").WriteLine(item).PopIndent();
        var baseClass = !string.IsNullOrEmpty(ent.BaseType) && PocoSetting.UseInheritance
            ? ReducedBaseTyp(ent) //ent.BaseType 
            : PocoSetting.Inherit;

        csTemplate.StartClass(ent.Name, baseClass, abstractClass: ent.IsAbstrct);

        foreach (var p in ent.Properties)
        {
            var pp = new PropertyGenerator(p, PocoSetting);


            if (p.IsNavigate && !PocoSetting.AddNavigation && !PocoSetting.AddEager)
                continue;


            foreach (var item in pp.GetAllAttributes())
                if (!string.IsNullOrEmpty(item))
                    csTemplate.WriteLine(item);
            csTemplate.WriteLine(pp.Declaration);

            if (BlankSpaceBeforeProperties)
                csTemplate.WriteLine(""); //empty line
        }

        csTemplate.EndClass();
        if (includeNamespace) csTemplate.EndNamespace(); //"}" for namespace
        CodeText = csTemplate.ToString();
        return CodeText;
    }

    internal string PrefixNamespace(string name)
    {
        var namespc = name;
        if (!string.IsNullOrWhiteSpace(PocoSetting.NamespacePrefix))
            namespc = PocoSetting.NamespacePrefix + "." + name;

        return namespc;
    }

    internal string ReducedBaseTyp(ClassTemplate ct)
    {
        var ns = $"{ct.NameSpace}."; //
        var reducedName = ct.BaseType;
        if (ct.BaseType.StartsWith(ns))
            reducedName = ct.BaseType.Replace(ns, "");
        return reducedName;
    }

    private string GetHeader()
    {
        return CodeHeader.GetHeader(_pocoGen);
    }

    private string UsingAssembly(List<string> nameSpaces)
    {
        var h = new FluentCsTextTemplate(PocoSetting);
        var assemblyManager = new AssemplyManager(PocoSetting, ClassList);
        var asemplyList = assemblyManager.AssemplyReference;
        foreach (var entry in asemplyList) h.UsingNamespace(entry);
        //add also namespaces of the built-in schema namespaces
        if (nameSpaces.Count > 1)
            nameSpaces.ForEach(x =>
            {
                var namespc = PrefixNamespace(x);
                h.UsingNamespace(namespc);
            });
        return h.ToString();
    }
}