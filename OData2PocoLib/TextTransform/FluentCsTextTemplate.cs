﻿// Copyright (c) Mohamed Hassan & Contributors. All rights reserved. See License.md in the project root for license information.

namespace OData2Poco.TextTransform;

/// <summary>
///     Specialized CSharp TextTemplate for generating C# code
/// </summary>
internal class FluentCsTextTemplate : FluentTextTemplate<FluentCsTextTemplate>
{
    public FluentCsTextTemplate()
    {
        KeyWord = "class";
    }

    public FluentCsTextTemplate(PocoSetting setting)
    {
        KeyWord = setting.GeneratorType == GeneratorType.Record ? "record" : "class";
    }

    public string KeyWord { get; set; }

    public static implicit operator string(FluentCsTextTemplate ft)
    {
        return ft.ToString();
    }

    public FluentCsTextTemplate LeftBrace()
    {
        WriteLine("{");
        return this;
    }

    public FluentCsTextTemplate RightBrace()
    {
        WriteLine("}");
        return this;
    }

    public FluentCsTextTemplate UsingNamespace(string name)
    {
        WriteLine("using {0};", name);
        return this;
    }

    public FluentCsTextTemplate StartNamespace(string name)
    {
        WriteLine("namespace {0}", name);
        LeftBrace();
        return this;
    }

    public FluentCsTextTemplate EndNamespace()
    {
        RightBrace();
        return this;
    }

    public FluentCsTextTemplate WriteLineComment(string str)
    {
        WriteComment(str);
        NewLine();
        return this;
    }

    public FluentCsTextTemplate WriteComment(string str)
    {
        if (string.IsNullOrEmpty(str))
        {
            return this;
        }

        Write(str.Trim().StartsWith("//") ? str : $"//{str}");
        return this;
    }

    public FluentCsTextTemplate StartClass(ClassTemplate ct)
    {
        return StartClass(ct.Name, ct.GetComment(), ct.BaseType, abstractClass: ct.IsAbstrct);
    }

    public FluentCsTextTemplate EndClass()
    {
        PopIndent();
        RightBrace();
        PopIndent();
        return this;
    }

    internal FluentCsTextTemplate StartClass(
        string name,
        string inherit = "",
        string visibility = "public",
        bool abstractClass = false,
        string primaryCtor = "")
    {
        //syntax: 'public abstract partial class MyClass <primaryCtor>: parent'
        PushTabIndent() // ident one tab
            .Write(DeclareClass(name, inherit, visibility, abstractClass, primaryCtor))
            .NewLine()
            .LeftBrace()
            .PushSpaceIndent(); //push tab  for the next write block
        return this;
    }

    private string DeclareClass(
        string name,
        string inherit = "",
        string visibility = "public",
        bool abstractClass = false,
        string primaryCtor = "")
    {
        var abstractKeyword = abstractClass ? " abstract" : string.Empty;
        const string PartialKeyword = " partial";
        var baseClass = string.IsNullOrEmpty(inherit) ? string.Empty : $" : {inherit}";
        return $"{visibility}{abstractKeyword}{PartialKeyword} {KeyWord} {name}{primaryCtor}{baseClass}";
    }
}
