// Copyright (c) Mohamed Hassan & Contributors. All rights reserved. See License.md in the project root for license information.

namespace OData2Poco;

public interface IPocoGenerator
{
    MetaDataInfo MetaData { get; set; }
    List<ClassTemplate> GeneratePocoList();
}
