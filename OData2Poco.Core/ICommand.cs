// Copyright (c) Mohamed Hassan & Contributors. All rights reserved. See License.md in the project root for license information.

// ReSharper disable UnusedMemberInSuper.Global
namespace OData2Poco.CommandLine;

internal interface IPocoCommand
{
    Task Execute();
}