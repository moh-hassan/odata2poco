// Copyright (c) Mohamed Hassan & Contributors. All rights reserved. See License.md in the project root for license information.

namespace OData2Poco;
// ReSharper disable UnusedMemberInSuper.Global
public interface IPocoClassGeneratorMultiFiles
{
    PocoStore GeneratePoco();
}

public interface IPocoClassGenerator
{
    PocoSetting PocoSetting { get; set; }
    List<ClassTemplate> ClassList { get; set; }
    string GeneratePoco();
}