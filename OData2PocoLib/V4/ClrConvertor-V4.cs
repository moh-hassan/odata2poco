using System.Collections.Generic;
using Microsoft.OData.Edm;

namespace OData2Poco.V4
{
    //----------------------------------------convertor v4
    internal partial class Poco
    {
        internal readonly Dictionary<EdmPrimitiveTypeKind, string> ClrDictionary =
            new Dictionary<EdmPrimitiveTypeKind, string>
            {
            {EdmPrimitiveTypeKind.Int32, "int"},
            {EdmPrimitiveTypeKind.String, "string"},
            {EdmPrimitiveTypeKind.Binary, "byte[]"},
            {EdmPrimitiveTypeKind.Decimal, "decimal"},
            {EdmPrimitiveTypeKind.Int16, "short"},
            {EdmPrimitiveTypeKind.Single, "float"},
            {EdmPrimitiveTypeKind.Boolean, "bool"},
            {EdmPrimitiveTypeKind.Double, "double"},
            {EdmPrimitiveTypeKind.Guid, "Guid"},
            {EdmPrimitiveTypeKind.Byte, "byte"},
            {EdmPrimitiveTypeKind.Int64, "long"},
            {EdmPrimitiveTypeKind.SByte, "sbyte"},
            {EdmPrimitiveTypeKind.Stream, "Stream"},
            {EdmPrimitiveTypeKind.Geography, "Geography"},
            {EdmPrimitiveTypeKind.GeographyPoint, "GeographyPoint"},
            {EdmPrimitiveTypeKind.GeographyLineString, "GeographyLineString"},
            {EdmPrimitiveTypeKind.GeographyPolygon, "GeographyMultiPolygon"},
            {EdmPrimitiveTypeKind.GeographyCollection, "GeographyCollection"},
            {EdmPrimitiveTypeKind.GeographyMultiPolygon, "GeographyMultiPolygon"},
            {EdmPrimitiveTypeKind.GeographyMultiLineString, "GeographyMultiLineString"},
            {EdmPrimitiveTypeKind.GeographyMultiPoint, "GeographyMultiPoint"},
            {EdmPrimitiveTypeKind.Geometry, "Geometry"},
            {EdmPrimitiveTypeKind.GeometryPoint, "GeometryPoint"},
            {EdmPrimitiveTypeKind.GeometryLineString, "GeometryLineString"},
            {EdmPrimitiveTypeKind.GeometryPolygon, "GeometryPolygon"},
            {EdmPrimitiveTypeKind.GeometryCollection, "GeometryCollection"},
            {EdmPrimitiveTypeKind.GeometryMultiPolygon, "GeometryMultiPolygon"},
            {EdmPrimitiveTypeKind.GeometryMultiLineString, "GeometryMultiLineString"},
            {EdmPrimitiveTypeKind.GeometryMultiPoint, "GeometryMultiPoint"},
            {EdmPrimitiveTypeKind.DateTimeOffset, "DateTimeOffset"},
            {EdmPrimitiveTypeKind.Duration, "TimeSpan"},
            {EdmPrimitiveTypeKind.Date, "Microsoft.OData.Edm.Date"}, //DateTime not supported
            {EdmPrimitiveTypeKind.TimeOfDay, "Microsoft.OData.Edm.TimeOfDay"}
        };
    }
}