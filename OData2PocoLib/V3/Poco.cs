// Copyright (c) Mohamed Hassan & Contributors. All rights reserved. See License.md in the project root for license information.

using System.Xml;
using Microsoft.Data.Edm;
using Microsoft.Data.Edm.Csdl;
using Microsoft.Data.Edm.Library.Values;
using Microsoft.Data.Edm.Validation;
using OData2Poco.InfraStructure.Logging;
// ReSharper disable CollectionNeverQueried.Local
#if !NETCOREAPP
// ReSharper disable NullCoalescingConditionIsAlwaysNotNullAccordingToAPIContract
#endif

namespace OData2Poco.V3;

/// <summary>
///     Process metadataString and generate list of   classes
/// </summary>
internal partial class Poco : IPocoGenerator
{
    private readonly ILog _logger = PocoLogger.Default;
    private readonly PocoSetting _setting;

    internal Poco(MetaDataInfo metaData, PocoSetting setting)
    {
        _setting = setting;
        MetaData = metaData;
        SchemaErrors = new List<string>();
        EntitySets = new List<IEdmEntitySet>();
        Model = LoadModelFromString();
    }

    public string MetaDataAsString => MetaData.MetaDataAsString;
    private IEnumerable<IEdmEntitySet> EntitySets { get; set; }
    private List<string> SchemaErrors { get; }
    internal IEdmModel Model;
    public MetaDataInfo MetaData { get; set; }

    /// <summary>
    ///     Fill List with class name and properties of corresponding entitie to be used for generating code
    /// </summary>
    /// <returns></returns>
    public List<ClassTemplate> GeneratePocoList()
    {
        var list = new List<ClassTemplate>();
        var schemaElements = GetSchemaElements(Model);
        var id = 1;
        foreach (var type in schemaElements.ToList())
        {
            var ct = GeneratePocoClass(type, id++);
            if (ct == null) continue;
            list.Add(ct);
        }

        //filter model
        if (_setting.Include?.Count > 0)
            list = list.FilterList(_setting.Include).ToList();
        return list;
    }

    private IEdmModel LoadModelFromString()
    {
        var tr = new StringReader(MetaDataAsString);
        var xmlReader = XmlReader.Create(tr);
        var flag = EdmxReader.TryParse(xmlReader, out Model, out var errors);
        List<string> messages = errors
            .Select(a => $"{a.ErrorCode}: {a.ErrorMessage} {a.ErrorLocation}").ToList();
        string errorText = messages.Any()
            ? $"Encountered the following errors (total: {messages.Count}) when parsing the EDMX document:\n" + string.Join(Environment.NewLine, messages)
            : string.Empty;
        if (!flag)
        {
            throw new InvalidOperationException($"Model can't be generated.\n{errorText}\n");
        }

        EntitySets = GetEntitySets(Model);
        if (_setting.ShowWarning && errors.Any())
            _logger.Info($"{errorText}\nXml Parser errors are ignored.");
        return Model;
    }

    private List<string> GetEnumElements(IEdmSchemaType type, out bool isFlags)
    {
        var enumList = new List<string>();
        isFlags = false;
        if (type.TypeKind != EdmTypeKind.Enum) return enumList;
        if (!(type is IEdmEnumType enumType)) return enumList;
        var list2 = enumType.Members;
        isFlags = enumType.IsFlags;
        foreach (var item in list2)
        {
            var enumValue = ((EdmIntegerConstant)item.Value).Value;
            var enumElement = $"\t\t{item.Name}={enumValue}";

            //enumList.Add(item.Name); // v2.3.0
            enumList.Add(enumElement); //issue #7 complete enum name /value
        }

        return enumList;
    }

    private string GetEntitySetName(IEdmSchemaType ct)
    {
        if (ct.TypeKind != EdmTypeKind.Entity)
            return string.Empty;

        var entitySet = EntitySets
            .Where(m => m.ElementType.FullName() == ct.FullName())
            .DefaultIfEmpty().First();
        return entitySet != null ? entitySet.Name : ct.Name;
    }

    internal IEnumerable<IEdmEntitySet> GetEntitySets(IEdmModel model)
    {
        var entitySets = model.EntityContainers()
            .SelectMany(c => c.Elements)
            .Where(x => x.ContainerElementKind == EdmContainerElementKind.EntitySet)
            .Select(element => (IEdmEntitySet)element);
        return entitySets;
    }

    internal ClassTemplate? GeneratePocoClass(IEdmSchemaType ent, int id)
    {
        var className = ent.Name;
        var classTemplate = new ClassTemplate(id)
        {
            Name = className,
            OriginalName = className,
            IsEnum = ent is IEdmEnumType,
            NameSpace = ent.Namespace
        };

        switch (ent)
        {
            case IEdmEnumType enumType:
                {
                    classTemplate.EnumElements =
                        GetEnumElements(enumType, out var isFlags); //fill enum elements for enumtype
                    classTemplate.IsFlags = isFlags;
                    return classTemplate;
                }

            case IEdmEntityType entityType:
                {
                    classTemplate.IsOpen = entityType.IsOpen;
                    classTemplate.IsEntity = true;
                    classTemplate.IsAbstrct = entityType.IsAbstract;
                    // Set base type by default
                    var baseEntityType = entityType.BaseEntityType();
                    if (baseEntityType != null) classTemplate.BaseType = baseEntityType.FullName(); //.Name
                    classTemplate.EntitySetName = GetEntitySetName(ent);
                    break;
                }
            //parent of complex types
            case IEdmComplexType complexType:
                {
                    classTemplate.IsOpen = complexType.IsOpen;
                    classTemplate.IsComplex = true;
                    classTemplate.IsAbstrct = complexType.IsAbstract;
                    if (complexType.BaseType != null)
                        classTemplate.BaseType = complexType.BaseType.ToString() ?? string.Empty;
                    break;
                }
            default:
                return null;
        }

        //fill keys 
        var list = GetKeys(ent);
        if (list.Any()) classTemplate.Keys.AddRange(list);

        //fill navigation properties
        var list2 = GetNavigation(ent);
        if (list2.Any()) classTemplate.Navigation.AddRange(list2);

        var entityProperties = GetClassProperties(ent);

        //set the key ,comment
        foreach (var property in entityProperties)
        {
            property.ClassName = className;
            property.OriginalName = property.PropName;
            if (classTemplate.Navigation.Exists(x => x == property.PropName)) property.IsNavigate = true;

            if (classTemplate.Keys.Exists(x => x == property.PropName)) property.IsKey = true;
            var comment = (property.IsKey ? "PrimaryKey" : string.Empty)
                          + (property.IsNullable ? string.Empty : " not null");
            if (!string.IsNullOrEmpty(comment)) property.PropComment = comment;
        }

        classTemplate.Properties.AddRange(entityProperties);
        return classTemplate;
    }

    private List<string> GetNavigation(IEdmSchemaType ent)
    {
        var list = new List<string>();
        if (!(ent is IEdmEntityType entityType)) return list;
        var nav = entityType.DeclaredNavigationProperties().ToList();
        list.AddRange(nav.Select(key => key.Name));
        return list;
    }

    private List<string> GetKeys(IEdmSchemaType ent)
    {
        var list = new List<string>();
        if (!(ent is IEdmEntityType entityType)) return list;
        var keys = entityType.DeclaredKey;
        if (keys != null)
            list.AddRange(keys.Select(key => key.Name));
        return list;
    }

    private List<PropertyTemplate> GetClassProperties(IEdmSchemaType ent)
    {
        //stop here if enum
        if (ent is IEdmEnumType)
            return Enumerable.Empty<PropertyTemplate>().ToList();
        var structuredType = ent as IEdmStructuredType;
        var properties = structuredType.Properties();
        if (_setting is { UseInheritance: true })
            properties = properties.Where(x => ((IEdmSchemaType)x.DeclaringType).FullName() == ent.FullName());

        //add serial for properties to support protbuf v3.0
        var serial = 1;
        var list = properties.Select(property => new PropertyTemplate
        {
            IsNullable = (!property.Type.IsPrimitive() && !property.Type.IsEnum()) || property.Type.IsNullable,
            PropName = property.Name,
            PropType = GetClrTypeName(property.Type),
            Serial = serial++,
            ClassNameSpace = ent.Namespace,
            MaxLength = GetMaxLength(property),
            IsReadOnly = Model.IsReadOnly(property)
        }).ToList();

        return list;
    }

    private int? GetMaxLength(IEdmProperty property)
    {
        int? maxLength = null;
        switch (property.Type.PrimitiveKind())
        {
            case EdmPrimitiveTypeKind.String:
                maxLength = property.Type.AsString().MaxLength;
                break;
            case EdmPrimitiveTypeKind.Binary:
                maxLength = property.Type.AsBinary().MaxLength;
                break;
        }

        //property.Type.AsDecimal().Precision
        return maxLength ?? 0;
    }

    //fill all properties/name of the class template
    private string GetClrTypeName(IEdmTypeReference edmTypeReference)
    {
        if (_setting.ShowWarning)
            CheckError(edmTypeReference);
        var clrTypeName = edmTypeReference.ToString() ?? "UNDEFINED";
        var edmType = edmTypeReference.Definition;
        if (edmTypeReference.IsPrimitive() && edmType != null)
            return EdmToClr((IEdmPrimitiveType)edmType);

        if (edmTypeReference.IsEnum() && edmType is IEdmEnumType ent)
            return ent.FullName();

        if (edmTypeReference.IsComplex() && edmType is IEdmComplexType edmComplexType)
            return edmComplexType.FullName();

        if (edmTypeReference.IsEntity() && edmType is IEdmEntityType ent2)
            return ent2.FullName();

        if (edmTypeReference.IsEntityReference() && edmType is IEdmEntityType ent1)
            return ent1.FullName();

        if (!edmTypeReference.IsCollection()) return clrTypeName;
        if (!(edmType is IEdmCollectionType edmCollectionType)) return clrTypeName;
        var elementTypeReference = edmCollectionType.ElementType;
        if (!(elementTypeReference.Definition is IEdmPrimitiveType primitiveElementType))
        {
            if (!(elementTypeReference.Definition is IEdmSchemaElement schemaElement))
                return clrTypeName;
            clrTypeName = schemaElement.FullName();
            clrTypeName = $"List<{clrTypeName}>";
            return clrTypeName;
        }

        clrTypeName = EdmToClr(primitiveElementType);
        clrTypeName = $"List<{clrTypeName}>";
        return clrTypeName;
    }

    private string EdmToClr(IEdmPrimitiveType type)
    {
        var kind = type.PrimitiveKind;
        return ClrDictionary.ContainsKey(kind)
            ? ClrDictionary[kind]
            : kind.ToString();
    }

    private void CheckError(IEdmTypeReference edmTypeReference)
    {
        var edmType = edmTypeReference.Definition;
        if (!edmType.Errors().Any()) return;
        var error = edmType.Errors().Select(x => $"Location: {x.ErrorLocation}, {x.ErrorMessage}").FirstOrDefault();
        _logger.Trace($"edmTypeReference Error: {error.Dump()}");
        _logger.Warn($"Invalid Type Reference: {edmType}");
        SchemaErrors.Add($"Invalid Type Reference: {edmType}");
    }

    #region Helper Methods

    internal IEdmSchemaType? GetSchemaType(string name, string nameSpace)
    {
        return (IEdmSchemaType?)Model.SchemaElements
            .FirstOrDefault(x => x.Name == name && x.Namespace == nameSpace);
    }

    internal IEdmSchemaType? GetSchemaType(string fullName)
    {
        return (IEdmSchemaType?)Model.SchemaElements
            .FirstOrDefault(x => x.FullName() == fullName);
    }

    private IEnumerable<IEdmSchemaType> GetSchemaElements(IEdmModel model)
    {
        return model.SchemaElements.OfType<IEdmSchemaType>();
    }

    internal IEnumerable<IEdmSchemaType> GetSchemaElements(
        Func<IEnumerable<IEdmSchemaType>, IEnumerable<IEdmSchemaType>> func)
    {
        var elements = Model.SchemaElements.OfType<IEdmSchemaType>();
        return func(elements);
    }

    #endregion
}