// Copyright (c) Mohamed Hassan & Contributors. All rights reserved. See License.md in the project root for license information.

namespace OData2Poco.V3;

using Microsoft.Data.Edm;

internal partial class Poco
{
    //----------------------------------------convertor  v3

    internal readonly Dictionary<EdmPrimitiveTypeKind, string> ClrDictionary = new()
    {
        {
            EdmPrimitiveTypeKind.Int32, "int"
        },
        {
            EdmPrimitiveTypeKind.String, "string"
        },
        {
            EdmPrimitiveTypeKind.Binary, "byte[]"
        },
        {
            EdmPrimitiveTypeKind.Decimal, "decimal"
        },
        {
            EdmPrimitiveTypeKind.Int16, "short"
        },
        {
            EdmPrimitiveTypeKind.Single, "float"
        },
        {
            EdmPrimitiveTypeKind.Boolean, "bool"
        },
        {
            EdmPrimitiveTypeKind.Double, "double"
        },
        {
            EdmPrimitiveTypeKind.Guid, "Guid"
        },
        {
            EdmPrimitiveTypeKind.Byte, "byte"
        },
        {
            EdmPrimitiveTypeKind.Int64, "long"
        },
        {
            EdmPrimitiveTypeKind.SByte, "sbyte"
        },
        {
            EdmPrimitiveTypeKind.Stream, "Stream"
        },
        {
            EdmPrimitiveTypeKind.Geography, "Geography"
        },
        {
            EdmPrimitiveTypeKind.GeographyPoint, "GeographyPoint"
        },
        {
            EdmPrimitiveTypeKind.GeographyLineString, "GeographyLineString"
        },
        {
            EdmPrimitiveTypeKind.GeographyPolygon, "GeographyMultiPolygon"
        },
        {
            EdmPrimitiveTypeKind.GeographyCollection, "GeographyCollection"
        },
        {
            EdmPrimitiveTypeKind.GeographyMultiPolygon, "GeographyMultiPolygon"
        },
        {
            EdmPrimitiveTypeKind.GeographyMultiLineString, "GeographyMultiLineString"
        },
        {
            EdmPrimitiveTypeKind.GeographyMultiPoint, "GeographyMultiPoint"
        },
        {
            EdmPrimitiveTypeKind.Geometry, "Geometry"
        },
        {
            EdmPrimitiveTypeKind.GeometryPoint, "GeometryPoint"
        },
        {
            EdmPrimitiveTypeKind.GeometryLineString, "GeometryLineString"
        },
        {
            EdmPrimitiveTypeKind.GeometryPolygon, "GeometryPolygon"
        },
        {
            EdmPrimitiveTypeKind.GeometryCollection, "GeometryCollection"
        },
        {
            EdmPrimitiveTypeKind.GeometryMultiPolygon, "GeometryMultiPolygon"
        },
        {
            EdmPrimitiveTypeKind.GeometryMultiLineString, "GeometryMultiLineString"
        },
        {
            EdmPrimitiveTypeKind.GeometryMultiPoint, "GeometryMultiPoint"
        },
        {
            EdmPrimitiveTypeKind.DateTimeOffset, "DateTimeOffset"
        },
        {
            EdmPrimitiveTypeKind.DateTime, "DateTime"
        },
        {
            EdmPrimitiveTypeKind.Time, "TimeSpan"
        }
        // {EdmPrimitiveTypeKind.None, "dynamic"},
    };
}
