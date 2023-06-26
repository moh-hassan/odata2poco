// Copyright (c) Mohamed Hassan & Contributors. All rights reserved. See License.md in the project root for license information.

namespace OData2Poco;


/// <summary>
///     Setting options to control the code generation
/// </summary>
public class PocoSetting : IValidator
{

    /// <summary>
    ///     Set nullabable ? to the type of property
    ///     Example int? , double?
    /// </summary>
    public bool AddNullableDataType { get; set; }

    /// <summary>
    ///     Add Navigation properties as virtual properties
    /// </summary>
    public bool AddNavigation { get; set; }

    /// <summary>
    ///     Add Navigation properties as non virtual properties for eager loading
    /// </summary>
    public bool AddEager { get; set; }

    /// <summary>
    ///     The language of code generation, Defalt is CS
    ///     Current supported language is C# only
    /// </summary>
    public Language Lang { get; set; }

    /// <summary>
    ///     Gets or sets the string to use after the colon of a class name for the base class to inherit and/or interfaces to
    ///     implement
    /// </summary>
    /// <value>
    ///     Base class and/or interfaces to implement
    /// </value>
    public string Inherit { get; set; }

    /// <summary>
    ///     Indicates whether or not to generate classes that follow the inheritance hierarchy of the ODATA types. Default is
    ///     true. Disable by setting Inherit to a non-null value.
    /// </summary>
    public bool UseInheritance => string.IsNullOrEmpty(Inherit);

    /// <summary>
    ///     Gets or sets a namespace prefix.
    /// </summary>
    /// <value>
    ///     The namespace.
    /// </value>
    public string NamespacePrefix { get; set; }

    /// <summary>
    ///     Gets or sets a NameCase: Pas/Camel/None for case conversion.
    /// </summary>
    public CaseEnum NameCase { get; set; }

    /// Gets or sets Entity NameCase: Pas/Camel/None for case conversion.
    /// </summary>
    public CaseEnum EntityNameCase { get; set; }

    /// <summary>
    ///     Explicit rename map.
    /// </summary>
    public RenameMap? RenameMap { get; set; }

    //add attributes: key,req,dm,tab,json,proto,display and db
    public List<string> Attributes { get; set; }
    public bool MultiFiles { get; set; }
    public string ModuleName { get; set; } = null!;
    public bool AddReference { get; set; }
    public GeneratorType GeneratorType { get; set; }
    public List<string>? Include { get; set; }
    public bool ReadWrite { get; set; } //all properties are read/write 
    public bool EnableNullableReferenceTypes { get; set; } //all properties are read/write

    public bool InitOnly { get; set; }

    // public bool AsRecord { get; set; } //create record type c# 9 feature
    public string OpenApiFileName { get; set; } = "";

    public string CodeFilename { get; set; }

    //set name of generated class using FullName vs Name
    public bool UseFullName { get; set; }
    public bool ShowWarning { get; set; }
    public string AtributeDefs { get; set; }

    /// <summary>
    ///     Initialization
    /// </summary>
    public PocoSetting()
    {
        Lang = Language.CS;
        NamespacePrefix = string.Empty;
        Inherit = "";
        NameCase = CaseEnum.None;
        Attributes = new List<string>();
        Include = new List<string>();
        CodeFilename = "UnDefined.txt";
        AtributeDefs = string.Empty;
    }
    public void Validate()
    {
        if (Lang == Language.None)
            Lang = Language.CS;

        if (GeneratorType == GeneratorType.None)
            GeneratorType = Lang == Language.CS
                ? GeneratorType.Class
                : GeneratorType.Interface;

        //multi files is not supported in c#, it's planned.
        if (Lang == Language.CS && MultiFiles)
        {
            Console.WriteLine("Multi-files is not supported in c#");
            MultiFiles = false;
        }

        //for type script
        if (string.IsNullOrEmpty(CodeFilename))
            CodeFilename = MultiFiles ? "Model" : $"poco.{Lang.ToString().ToLower()}";
    }
}


public interface IValidator
{
    // ReSharper disable once UnusedMemberInSuper.Global
    void Validate();
}