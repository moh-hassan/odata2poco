#pragma warning disable  CS0414  
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OData2Poco
{
    public class ProjectGenerator
    {
        Dictionary<string, string> ReferenceFullNet = new Dictionary<string, string>();
        Dictionary<string, string> PackageNetStandard = new Dictionary<string, string>();
        Dictionary<string, string> PackageCommon = new Dictionary<string, string>();

        //0 ReferenceFullNet, 1 PackageCommon , 2 PackageNetStandard
        string ProjectCode = @"
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
        List<string> Attributes { get; set; }
        public ProjectGenerator(List<string> attributes)
        {
            Attributes = attributes;
        }
     public    string GetProjectCode()
        {
            var code = string.Format(ProjectCode, GetReferenceFullNet(), GetPackageCommon(), GetPackageNetStandard());
            return code;
        }

     public StringBuilder GetPackageCommon()
     {
         var packageList = new StringBuilder();
            packageList.AppendFormat("\t\t\t<PackageReference Include=\"Microsoft.Spatial\" Version=\"7.5.4\" />\r\n");

            if (Attributes.Any(x => x == "json"))
             packageList.AppendFormat("\t\t\t<PackageReference Include=\"Newtonsoft.Json\" Version=\"12.0.1\" />\r\n");
         if (Attributes.Any(x => x == "proto"))
             packageList.AppendFormat("\t\t\t<PackageReference Include=\"protobuf-net\" Version=\"2.4.0\" />\r\n");
         return packageList;
     }
     public StringBuilder GetReferenceFullNet()
      
        {
            var packageList = new StringBuilder();
            //packageList.AppendFormat("<PackageReference Include=\"Microsoft.Spatial\" Version=\"7.5.4\" />\r\n");

            if (Attributes.Any(x => x == "key" || x=="req"|| x=="tab"|| x=="display"|| x=="db"))
                packageList.AppendFormat("\t\t\t<Reference Include=\"System.ComponentModel.DataAnnotations\"/>\r\n");
            if (Attributes.Any(x => x == "dm"))
                packageList.AppendFormat("\t\t\t<Reference Include=\"System.Runtime.Serialization\"/>\r\n");
            return packageList;
        }

     public StringBuilder GetPackageNetStandard()
     {
         var packageList = new StringBuilder();
         
         if (Attributes.Any(x => x == "key" || x=="req"|| x=="tab"|| x=="display"|| x=="db"))
             packageList.AppendFormat("\t\t\t <PackageReference Include=\"System.ComponentModel.Annotations\" Version=\"4.4.0\"/>\r\n");
         return packageList;
     }
    }
}

/*
 build.cmd

 <Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
      <TargetFrameworks>net45;net461;netstandard2.0</TargetFrameworks>
      <!--<TargetFramework>net45</TargetFramework>-->
  </PropertyGroup>
 <ItemGroup Condition="$(TargetFramework.StartsWith('net4')) ">
    <Reference Include="System.Runtime.Serialization" />  
    <Reference Include="System.ComponentModel.DataAnnotations" />
 </ItemGroup>  
   <ItemGroup>
     <PackageReference Include="Newtonsoft.Json" Version="12.0.1 " />
     <PackageReference Include="Microsoft.Spatial" Version="7.5.4 " />
     <PackageReference Include="protobuf-net" Version="2.4.0 " />
   </ItemGroup>
  <ItemGroup Condition="'$(TargetFramework)' == 'netstandard2.0' ">
    <PackageReference Include="System.ComponentModel.Annotations" Version="4.4.0 " />
  </ItemGroup>  
</Project>
 */
