// Copyright (c) Mohamed Hassan & Contributors. All rights reserved. See License.md in the project root for license information.

namespace OData2Poco.CustAttributes;

public interface INamedAttribute
{
    string Name { get; set; }
    string Scope { get; set; }
    bool IsUserDefined { get; set; }
    bool IsValid { get; set; }
    List<string> GetAttributes(PropertyTemplate propertyTemplate);
    List<string> GetAttributes(ClassTemplate classTemplate);
}
