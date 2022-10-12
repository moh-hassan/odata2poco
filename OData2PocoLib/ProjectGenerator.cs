// Copyright (c) Mohamed Hassan & Contributors. All rights reserved. See License.md in the project root for license information.

#pragma warning disable  CS0414
using System.Text;

namespace OData2Poco;

public class ProjectGenerator
{
    private readonly string _projectCode = @"
<Project Sdk=""Microsoft.NET.Sdk"">
        <PropertyGroup>
           <TargetFrameworks>net45;net461;netstandard2.0</TargetFrameworks>       
        </PropertyGroup>
        <ItemGroup Condition=""$(TargetFramework.StartsWith('net4')) "">
{0}
        </ItemGroup>  
        <ItemGroup>
{1}
        </ItemGroup>
        <ItemGroup Condition=""'$(TargetFramework)' == 'netstandard2.0' "">
{2}
        </ItemGroup>  
</Project>
";

    public ProjectGenerator(List<string> attributes)
    {
        Attributes = attributes;
    }

    private List<string> Attributes { get; }

    public string GetProjectCode()
    {
        var code = string.Format(_projectCode, GetReferenceFullNet(), GetPackageCommon(), GetPackageNetStandard());
        return code;
    }

    public StringBuilder GetPackageCommon()
    {
        var packageList = new StringBuilder();
        packageList.Append("\t\t\t<PackageReference Include=\"Microsoft.Spatial\" Version=\"7.5.4\" />\r\n");

        if (Attributes.Any(x => x == "json"))
            packageList.Append("\t\t\t<PackageReference Include=\"Newtonsoft.Json\" Version=\"12.0.1\" />\r\n");
        if (Attributes.Any(x => x == "proto"))
            packageList.Append("\t\t\t<PackageReference Include=\"protobuf-net\" Version=\"2.4.0\" />\r\n");
        return packageList;
    }

    public StringBuilder GetReferenceFullNet()

    {
        var packageList = new StringBuilder();

        if (Attributes.Any(x => x == "key" || x == "req" || x == "tab" || x == "display" || x == "db"))
            packageList.Append("\t\t\t<Reference Include=\"System.ComponentModel.DataAnnotations\"/>\r\n");
        if (Attributes.Any(x => x == "dm"))
            packageList.Append("\t\t\t<Reference Include=\"System.Runtime.Serialization\"/>\r\n");
        return packageList;
    }

    public StringBuilder GetPackageNetStandard()
    {
        var packageList = new StringBuilder();

        if (Attributes.Any(x => x == "key" || x == "req" || x == "tab" || x == "display" || x == "db"))
            packageList.Append(
                "\t\t\t <PackageReference Include=\"System.ComponentModel.Annotations\" Version=\"4.4.0\"/>\r\n");
        return packageList;
    }
}