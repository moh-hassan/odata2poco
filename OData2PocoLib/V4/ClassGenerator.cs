//#define odataV3

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#if odataV3
using Microsoft.Data.Edm;
#else
using Microsoft.OData.Edm;
#endif



#if odataV3
namespace OData2PocoLib.V3
#else
namespace OData2PocoLib.V4
#endif

{
    public partial class Poco
    {

        //string CollectionTemplate
        //  {
        //      get { return "ICollection<{0}>"; }
        //  }

        private List<string> GenerateProperties(IEdmSchemaType ent)
        {
            List<string> entityProperties = new List<string>();
            //List<string> AllProperties = new List<string>();
            Console.WriteLine("GenerateType " + ent.Name);
            try
            {
                //sb = new StringBuilder();
                ClassName = ent.Name;


                IEdmStructuredType structuredType = ent as IEdmStructuredType;
                //byme
                var entProperties = structuredType.Properties(); //.StructuralProperties();
                //Console.WriteLine(structuredType.FullTypeName());

                // WritePropertiesForStructuredType(tt);
                var properties = entProperties.Select(p => new
               {
                   PropertyType = GetClrTypeName(p.Type),
                   PropertyName = p.Name
               }).ToList();


                entityProperties.AddRange(properties.Select(property => string.Format("\tpublic {0} {1} {{get;set;}}", property.PropertyType, property.PropertyName)));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
            return entityProperties;
        }


        //internal string ClassToString()
        //{
        //    var sbClass = new StringBuilder();
        //    sbClass.AppendFormat("class {0}\n", Name);
        //    sbClass.AppendLine("{");
        //    foreach (var p in EntityProperties)
        //    {
        //        sbClass.AppendLine(p);
        //    }
        //    sbClass.AppendLine("}");
        //    return sbClass.ToString();
        //}
        private string ClassToString(IEdmSchemaType ent)
        {
            var entityProperties = GenerateProperties(ent);
            var sbClass = new StringBuilder();
            sbClass.AppendFormat("class {0}\n", ClassName);
            sbClass.AppendLine("{");
            foreach (var p in entityProperties)
            {
                sbClass.AppendLine(p);
            }
            sbClass.AppendLine("}");
            return sbClass.ToString();
        }

        //internal string GetSourceOrReturnTypeName(IEdmTypeReference typeReference)
        //{
        //    IEdmCollectionType edmCollectionType = typeReference.Definition as IEdmCollectionType;
        //    bool addNullableTemplate = true;
        //    if (edmCollectionType != null)
        //    {
        //        typeReference = edmCollectionType.ElementType;
        //        addNullableTemplate = false;
        //    }

        //    return typeReference.ToString();
        //    //return Utils.GetClrTypeName(typeReference, this.context.UseDataServiceCollection, this, this.context,
        //    //    addNullableTemplate);
        //}




        //ok primitive
        //IEdmPrimitiveType -IEdmComplexType-IEdmEnumType - 
        public string GetClrTypeName(IEdmTypeReference edmTypeReference)
        {


            string clrTypeName = edmTypeReference.ToString();
            IEdmType edmType = edmTypeReference.Definition;

            if (edmTypeReference.IsPrimitive()) return EdmToClr((IEdmPrimitiveType)edmType);

            if (edmTypeReference.IsComplex()) return ((IEdmComplexType)edmType).Name;

            if (edmTypeReference.IsEntity()) return ((IEdmEntityType)edmType).Name;

            if (edmTypeReference.IsCollection())
            {
                IEdmCollectionType edmCollectionType = (IEdmCollectionType)edmType;
                IEdmTypeReference elementTypeReference = edmCollectionType.ElementType;
                IEdmPrimitiveType primitiveElementType = elementTypeReference.Definition as IEdmPrimitiveType;
                if (primitiveElementType == null)
                {
                    IEdmSchemaElement schemaElement = elementTypeReference.Definition as IEdmSchemaElement;
                    if (schemaElement != null)
                    {
                        clrTypeName = schemaElement.Name;
                        clrTypeName = string.Format("ICollection<{0}>", clrTypeName);
                    }
                    return clrTypeName;
                }
                clrTypeName = primitiveElementType.ToString();
                Console.WriteLine("^^^^^^^ " + clrTypeName);
            }
            return clrTypeName;
        }


        public string EdmToClr(IEdmPrimitiveType type)
        {
            EdmPrimitiveTypeKind kind = type.PrimitiveKind;

            if (_clrDictionary.ContainsKey(kind))
                return _clrDictionary[kind];
            return kind.ToString();
        }

    }//
}//

