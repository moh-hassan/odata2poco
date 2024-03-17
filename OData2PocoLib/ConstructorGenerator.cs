// Copyright (c) Mohamed Hassan & Contributors. All rights reserved. See License.md in the project root for license information.

namespace OData2Poco;

using System.Text;
using Extensions;
using TextTransform;

internal partial class PropertyGenerator
{
    internal string VariableName => Name.ToCamelCase();

    internal string VariableDeclaration
        => new StringBuilder()
            .Append(ReducedPropertyTypeName)
            .Append(' ')
            .Append(VariableName)
            .ToString();

    //todo use this.name when using camel case
    internal string AssignmentDeclaration
        => new StringBuilder()
            .Append($"\t{Name}")
            .Append(" = ")
            .Append(VariableName)
            .Append(';')
            .ToString();

    public static string GenerateProperties(ClassTemplate classTemplate, PocoSetting? pocoSetting)
    {
        //validate parameters
        if (classTemplate == null)
            return string.Empty;
        if (classTemplate.Properties.Count == 0)
            return string.Empty;
        pocoSetting ??= new PocoSetting();
        var csTemplate = new FluentCsTextTemplate();
        // AddNavigation and AddEager are mutually exclusive
        var properties = pocoSetting.AddNavigation || pocoSetting.AddEager
            ? classTemplate.Properties
            : classTemplate.Properties.Where(p => !p.IsNavigate);
        foreach (var p in properties)
        {
            PropertyGenerator pp = new(p, pocoSetting);
            foreach (var item in pp.GetAllAttributes().Where(item => !string.IsNullOrEmpty(item)))
            {
                csTemplate.WriteLine(item);
            }

            csTemplate.WriteLine(pp.Declaration);
            var blankSpaceBeforeProperties = true;
            if (blankSpaceBeforeProperties)
            {
                csTemplate.WriteLine(string.Empty); //empty line
            }
        }

        return csTemplate.ToString();
    }

    public static IList<PropertyTemplate> GetPropertiesForConstructor(IList<PropertyTemplate> propertyList) => propertyList.Where(p => p.InConstructor).ToList();

    public static string GenerateFullConstructor(
        ClassTemplate classTemplate,
        PocoSetting? pocoSetting = null)
    {
        pocoSetting ??= new PocoSetting();
        if (classTemplate is null)
            return string.Empty;
        if (pocoSetting.WithConstructor == Ctor.None)
            return string.Empty;
        var className = classTemplate.Name;
        var propertyList = classTemplate.Properties;
        if (propertyList is [])
            return string.Empty;
        var propertyTemplateList = GetPropertiesForConstructor(propertyList);
        if (propertyTemplateList is [])
            return string.Empty;
        List<string> assignmentList = [];
        FluentCsTextTemplate textTemplate = new();
        foreach (var prop in propertyTemplateList)
        {
            var pg = new PropertyGenerator(prop, pocoSetting);
            assignmentList.Add(pg.AssignmentDeclaration);
        }

        var parameterLessConstructor = $"public {className} () {{}}\r\n";
        textTemplate.WriteLine(parameterLessConstructor)
            .Write("public ")
            .Write(className)
            .WriteLine(GeneratePrimaryConstructor(classTemplate, pocoSetting))
            .WriteLine("{")
            .WriteList(assignmentList, Environment.NewLine)
            .WriteLine(string.Empty)
            .WriteLine("}");
        return textTemplate.ToString();
    }

    internal static string GeneratePrimaryConstructor(
        ClassTemplate classTemplate,
        PocoSetting? pocoSetting = null)
    {
        var propertyList = classTemplate.Properties;
        if (propertyList is [])
            return string.Empty;
        pocoSetting ??= new PocoSetting();
        var propertyTemplateList = GetPropertiesForConstructor(propertyList);
        if (propertyTemplateList is [])
            return string.Empty;
        List<string> variableList = [];
        FluentCsTextTemplate template = new();
        foreach (var prop in propertyTemplateList)
        {
            var pg = new PropertyGenerator(prop, pocoSetting);
            variableList.Add(pg.VariableDeclaration);
        }

        template
            .Write(" (")
            .WriteList(variableList, ", ")
            .Write(")");
        return template.ToString();
    }
}

public enum Ctor
{
    None,    //No constructor
    Full,    // full constructor
}
