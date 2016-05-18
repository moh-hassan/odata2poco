#define odataV3

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Xml;


#if odataV3
using Microsoft.Data.Edm;
using Microsoft.Data.Edm.Csdl;
#else
using Microsoft.OData.Edm;
using Microsoft.OData.Edm.Csdl;
#endif

#if odataV3
namespace OData2Poco.V3
#else
namespace OData2Poco.V4
#endif
{
    /// <summary>
    /// Process metadataString and generate list of   classes
    /// </summary>
    internal partial class Poco : IPocoGenerator
    {
       
        public MetaDataInfo MetaData { get; set; }

        public string MetaDataAsString
        {
            get { return MetaData.MetaDataAsString; }  
             
        }

        public string MetaDataVersion
        {
            get
            {
                return MetaData.MetaDataVersion; 
            }
        }

        public string ServiceUrl
        {
            get
            {
                return MetaData.ServiceUrl; 
            }
        }

        //internal Poco(string metaData, string serviceUrl)
        //{
        //    MetaDataAsString = metaData;
        //    ServiceUrl = serviceUrl;
        //    //MetaDataVersion = Helper.GetMetadataVersion(metaData);
        //}
        internal Poco(MetaDataInfo metaData)
        {
            MetaData = metaData;
            
        }
        private IEnumerable<IEdmSchemaType> SchemaElements
        {
            get
            {
                var tr = new StringReader(MetaDataAsString);
                var model2 = EdmxReader.Parse(XmlReader.Create(tr));
                var schemaElements = model2.SchemaElements.OfType<IEdmSchemaType>();
                return schemaElements;
            }
        }

        public IEnumerable<IEdmEntitySet> EntitySets
        {
            get
            {
                var tr = new StringReader(MetaDataAsString);
                var model2 = EdmxReader.Parse(XmlReader.Create(tr));
#if odataV3
                  var entitySets = model2.EntityContainers().First().EntitySets();
#else
                var entitySets = model2.EntityContainer.EntitySets();
#endif
                return entitySets;
            }
        }
      
        public string GetEntitySetName(string entityName)
        {
            //Console.WriteLine("name {0}",entityName);
            if (entityName == null) return "un";
#if odataV3
            var result = EntitySets.FirstOrDefault(m => m.ElementType.Name == entityName);
#else
            var result = EntitySets.FirstOrDefault(m => m.EntityType().Name == entityName);
#endif
            if (result != null)
            {
                //Console.WriteLine(result.Name);
                return result.Name;
            }
            return "";
        }
        
        private List<string> GetEnumElements(IEdmSchemaType type)
        {
            List<string> enumList = new List<string>();

            if (type.TypeKind == EdmTypeKind.Enum)
            {
                var enumType = type as IEdmEnumType;
                if (enumType != null)
                {
                    var list2 = enumType.Members;

                    foreach (var item in list2)
                    {
                        Debug.WriteLine("GetEnumElements- name: [{0}] ", (object)item.Name);
                        enumList.Add(item.Name);
                    }

                }

            }
            return enumList;
        }

       /// <summary>
        /// Fill List with class name and properties of corresponding entitie to be used for generating code
       /// </summary>
       /// <returns></returns>
        public List<ClassTemplate> GeneratePocoList()
        {
            List<ClassTemplate> list = new List<ClassTemplate>();
            var schemaElements = SchemaElements;

            foreach (var type in schemaElements)
            {
                ClassTemplate ct = GeneratePocoClass(type);
                if (ct.IsEnum) ct.EnumElements = GetEnumElements(type); //fill enum elements for enum type
                list.Add(ct);

            }

            return list;
        }


        private ClassTemplate GeneratePocoClass(IEdmSchemaType ent)
        {

            if (ent == null) return null;
            //for debuging
            var debugString = Helper.Dump(ent);
            //v1.0.0-rc3 , enum support
            var enumType = ent as IEdmEnumType;
            ClassTemplate classTemplate = new ClassTemplate
               {
                   Name = ent.Name,
                   ToDebugString = debugString,
                   IsEnum = (enumType != null)

               };

            //for enum type , stop here , no more information needed
            if (classTemplate.IsEnum) return classTemplate;

            //fill setname
            //v1.4
            classTemplate.EntitySetName = GetEntitySetName(ent.Name);

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
                //@@@ v1.0.0-rc3  
                if (classTemplate.Navigation.Exists(x => x == property.PropName)) property.IsNavigate = true;

                if (classTemplate.Keys.Exists(x => x == property.PropName)) property.IsKey = true;
                var comment = (property.IsKey ? "PrimaryKey" : String.Empty)
                                + (property.IsNullable ? String.Empty : " not null");
                if (!string.IsNullOrEmpty(comment)) property.PropComment = "//" + comment;

            }
            classTemplate.Properties.AddRange(entityProperties);
            return classTemplate;
        }

        private List<string> GetNavigation(IEdmSchemaType ent)
        {
            var list = new List<string>();
            var entityType = ent as IEdmEntityType;
            if (entityType != null)
            {
                var nav = entityType.DeclaredNavigationProperties().ToList();
                list.AddRange(nav.Select(key => key.Name));
            }

            return list;
        }
        private List<string> GetKeys(IEdmSchemaType ent)
        {
            var list = new List<string>();
            var entityType = ent as IEdmEntityType;
            if (entityType != null)
            {
                var keys = entityType.DeclaredKey;
                if (keys != null)
                    list.AddRange(keys.Select(key => key.Name));
            }

            return list;
        }

        private List<PropertyTemplate> GetClassProperties(IEdmSchemaType ent)
        {

            //stop here for enum
            var enumType = ent as IEdmEnumType;
            if (enumType != null) return null;

            var structuredType = ent as IEdmStructuredType;
            var properties = structuredType.Properties();

            var list = properties.Select(property => new PropertyTemplate()
            {
                ToTrace = property.ToTraceString(),
                IsNullable = property.Type.IsNullable,
                PropName = property.Name,
                PropType = GetClrTypeName(property.Type),
                ToDebugString = Helper.Dump(property)
            }).ToList();

            return list;
        }

        //fill all properties/name of the class template

        private string GetClrTypeName(IEdmTypeReference edmTypeReference)
        {
            string clrTypeName = edmTypeReference.ToString();
            IEdmType edmType = edmTypeReference.Definition;


            if (edmTypeReference.IsPrimitive()) return EdmToClr(edmType as IEdmPrimitiveType);

            //@@@ v1.0.0-rc2
            if (edmTypeReference.IsEnum())
            {
                var ent = edmType as IEdmEnumType;
                if (ent != null) return ent.Name;
            }

            if (edmTypeReference.IsComplex())
            {
                var edmComplexType = edmType as IEdmComplexType;
                if (edmComplexType != null) return edmComplexType.Name;
            }

            if (edmTypeReference.IsEntity())
            {
                var ent = edmType as IEdmEntityType;
                if (ent != null) return ent.Name;
            }

            if (edmTypeReference.IsCollection())
            {

                IEdmCollectionType edmCollectionType = edmType as IEdmCollectionType;
                if (edmCollectionType != null)
                {
                    IEdmTypeReference elementTypeReference = edmCollectionType.ElementType;
                    IEdmPrimitiveType primitiveElementType = elementTypeReference.Definition as IEdmPrimitiveType;
                    if (primitiveElementType == null)
                    {
                        IEdmSchemaElement schemaElement = elementTypeReference.Definition as IEdmSchemaElement;
                        if (schemaElement != null)
                        {
                            clrTypeName = schemaElement.Name;
                            //@@@ 1.0.0-rc2
                            // clrTypeName = string.Format("ICollection<{0}>", clrTypeName);
                            clrTypeName = string.Format("List<{0}>", clrTypeName); //to support RestSharp
                        }
                        return clrTypeName;
                    }

                    clrTypeName = EdmToClr(primitiveElementType);
                    clrTypeName = string.Format("List<{0}>", clrTypeName);
                }
                return clrTypeName;
            }//IsCollection
            return clrTypeName;
        }


        private string EdmToClr(IEdmPrimitiveType type)
        {
            var kind = type.PrimitiveKind;

            if (ClrDictionary.ContainsKey(kind))
                return ClrDictionary[kind];
            return kind.ToString();
        }
    }
}
