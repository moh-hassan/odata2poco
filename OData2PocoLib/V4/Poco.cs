//#define odataV3
//v4
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using OData2Poco.InfraStructure.Logging;
#if odataV3
using Microsoft.Data.Edm.Library.Values;
using Microsoft.Data.Edm;
using Microsoft.Data.Edm.Csdl;
#else
using Microsoft.OData.Edm;
//using Microsoft.OData.Edm.Validation;
using Microsoft.OData.Edm.Csdl;
using ISchemaType = Microsoft.OData.Edm.IEdmSchemaType;


#if !EDM7
using Microsoft.OData.Edm.Library.Values;
#endif

#endif

#if odataV3
namespace OData2Poco.V3
#else
namespace OData2Poco.V4
#endif
{
    /// <summary>
    ///     Process metadataString and generate list of   classes
    /// </summary>
    internal partial class Poco : IPocoGenerator
    {
        //private readonly ColoredConsole _logger = PocoLogger.Default;
        private readonly PocoSetting _setting;
        public MetaDataInfo MetaData { get; set; }
        public string MetaDataAsString => MetaData.MetaDataAsString;
        public string MetaDataVersion => MetaData.MetaDataVersion;
        public string ServiceUrl => MetaData.ServiceUrl;
        private IEnumerable<IEdmEntitySet> EntitySets { get; set; }
        internal Poco(MetaDataInfo metaData, PocoSetting setting)
        {
            _setting = setting;
            MetaData = metaData;

        }


#if odataV3
        private IEdmModel LoadModelFromString(string xmlString)
        {
            var tr = new StringReader(MetaDataAsString);
            var model2 = EdmxReader.Parse(XmlReader.Create(tr));
            return model2;
        }
#else

        //support Odata.Edm v7+
        private IEdmModel LoadModelFromString(string xmlString)
        {
            IEdmModel model2;
            //Microsoft.OData.Edm" v7+
            //breaking change in Odata.Edm in v7+
            var tr = new StringReader(xmlString);
            var reader = XmlReader.Create(tr);

            //IEnumerable<EdmError> errors = null;
            try
            {
#if EDM7
                CsdlReader
#else
                EdmxReader
#endif
                  .TryParse(reader, true, out model2, out _);
                // IgnoreUnexpectedElementsAndAttributes
                //  .TryParse(reader, true, out model2, out errors);
            }
            finally
            {
                ((IDisposable)reader).Dispose();
            }
            return model2;
        }
#endif

        private string GetEntitySetName(string entityName)
        {
            //Console.WriteLine("name {0}",entityName);
            if (entityName == null) return "un";
#if odataV3
            var result = EntitySets.FirstOrDefault(m => m.ElementType.Name == entityName);
#else
            var result = EntitySets.FirstOrDefault(m => m.EntityType().Name == entityName);
#endif
            return result != null ? result.Name : string.Empty;
        }

        private List<string> GetEnumElements(IEdmSchemaType type, out bool isFlags)
        {
            var enumList = new List<string>();
            isFlags = false;
            if (type.TypeKind == EdmTypeKind.Enum)
                if (type is IEdmEnumType enumType)
                {
                    var list2 = enumType.Members;
                    isFlags = enumType.IsFlags;
                    foreach (var item in list2)
                    {

#if odataV3
                        var enumValue = ((EdmIntegerConstant)item.Value).Value;
                        var enumElement = $"\t\t{item.Name}={enumValue}";
#else
#if EDM7
                        //issue12 reserved keyword
                        var enumElement = $"\t\t{item.Name.ChangeReservedWord()}={item.Value.Value}";
                        //var enumElement = $"\t\t{item.Name}={item.Value.Value}";
#else
                        var enumValue = ((EdmIntegerConstant)item.Value).Value;
                        var  enumElement = $"\t\t{item.Name.ChangeReservedWord()}={enumValue}";
                       //var  enumElement = $"\t\t{item.Name}={enumValue}";
#endif
#endif
                        //enumList.Add(item.Name); // v2.3.0
                        enumList.Add(enumElement); //issue #7 complete enum name /value
                    }
                }

            return enumList;
        }

        /// <summary>
        ///     Fill List with class name and properties of corresponding entitie to be used for generating code
        /// </summary>
        /// <returns></returns>
        public List<ClassTemplate> GeneratePocoList()
        {
            var list = new List<ClassTemplate>();
            var model2 = LoadModelFromString(MetaDataAsString);
            var schemaElements = model2.SchemaElements.OfType<IEdmSchemaType>();
            //var schemaElements = SchemaElements;

#if odataV3
            EntitySets = model2.EntityContainers().First().EntitySets();
#else
            EntitySets = model2.EntityContainer.EntitySets();
#endif

            foreach (var type in schemaElements)
            {
                var ct = GeneratePocoClass(type);
                if (ct.IsEnum)
                {
                    ct.EnumElements = GetEnumElements(type, out var isFlags); //fill enum elements for enumtype
                    ct.IsFlags = isFlags;
                }

                list.Add(ct);
            }
            return list;
        }


        internal ClassTemplate GeneratePocoClass(IEdmSchemaType ent)
        {
            if (ent == null) return null;
            var className = ent.Name;
            var classTemplate = new ClassTemplate
            {
                Name = className,
                OriginalName = className,
                IsEnum = ent is IEdmEnumType
            };

            //for enum type , stop here , no more information needed
            if (classTemplate.IsEnum) return classTemplate;

            classTemplate.EntitySetName = GetEntitySetName(ent.Name);

            // Set base type if _setting.UseInheritance == true
            if (_setting.UseInheritance && ent is IEdmEntityType entityType)
            {
                var baseEntityType = entityType.BaseEntityType();
                if (baseEntityType != null) classTemplate.BaseType = baseEntityType.Name;
            }

            //fill keys 
            var list = GetKeys(ent);
            if (list != null) classTemplate.Keys.AddRange(list);

            //fill navigation properties
            var list2 = GetNavigation(ent);
            if (list2 != null) classTemplate.Navigation.AddRange(list2);

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
                if (!string.IsNullOrEmpty(comment)) property.PropComment = $"//{comment}";
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
            if (ent is IEdmEnumType) return null;
            var structuredType = ent as IEdmStructuredType;
            var properties = structuredType.Properties();
            if (_setting.UseInheritance)
            {
#if odataV3
                properties = properties.Where(x => ((IEdmSchemaType)x.DeclaringType).FullName() == ent.FullName());
#else
                properties = properties.Where(x => x.DeclaringType.FullTypeName() == ent.FullTypeName());
#endif
            }

            //add serial for properties to support protbuf v3.0
            var serial = 1;
            var list = properties.Select(property => new PropertyTemplate
            {
                IsNullable = property.Type.IsNullable,
                PropName = property.Name,
                PropType = GetClrTypeName(property.Type),
                Serial = serial++
            }).ToList();

            return list;
        }
        //-----------

        //------------
        //fill all properties/name of the class template
        private string GetClrTypeName(IEdmTypeReference edmTypeReference)
        {
            var clrTypeName = edmTypeReference.ToString();
            var edmType = edmTypeReference.Definition;
            if (edmTypeReference.IsPrimitive()) return EdmToClr(edmType as IEdmPrimitiveType);
            if (edmTypeReference.IsEnum())
            {
                if (edmType is IEdmEnumType ent) return ent.Name;
            }

            if (edmTypeReference.IsComplex())
            {
                if (edmType is IEdmComplexType edmComplexType) return edmComplexType.Name;
            }

            if (edmTypeReference.IsEntity())
            {
                if (edmType is IEdmEntityType ent) return ent.Name;
            }

            if (!edmTypeReference.IsCollection()) return clrTypeName;
            if (!(edmType is IEdmCollectionType edmCollectionType)) return clrTypeName;
            var elementTypeReference = edmCollectionType.ElementType;
            var primitiveElementType = elementTypeReference.Definition as IEdmPrimitiveType;
            if (primitiveElementType == null)
            {
                if (!(elementTypeReference.Definition is IEdmSchemaElement schemaElement)) return clrTypeName;
                clrTypeName = schemaElement.Name;
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
    }
}