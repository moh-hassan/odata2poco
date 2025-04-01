# Welcome to OData2Poco
**OData2Poco** is a code generation tool for generating plain-old CLR objects (POCO/DTO) in c# and typescript from OData feeds. 
POCO classes can be used in a typed RESTful client OData services. Code generation can be controlled by setting many options.

----------
## OData2Poco 6.4.0 is now available!
The version v6.4.0 is released on April 1,2025 and it is now available in nuget! The new major enhancement are:
- New Feature: Avoid generating code when there is no change in the metadata since last generation (#52).
- New Feature: Add new option ```login``` which support Cookie-Based Authentication (Session-Based Authentication). The new feature help SAP users for login to Cookie-Based Authentication Odata service.

Read how to use the [new features](https://github.com/moh-hassan/odata2poco/wiki/v6_4_0)

-----

[![NuGet Version](https://img.shields.io/nuget/v/OData2Poco.CommandLine.svg?label=nuget%20ClassLibrary&style=flat)](https://www.nuget.org/packages/OData2Poco) </p>
[![NuGet Version](https://img.shields.io/nuget/v/OData2Poco.CommandLine.svg?label=nuget%20Console&style=flat)](https://www.nuget.org/packages/OData2Poco.CommandLine) </p>
[![Global Tool NuGet Version](https://img.shields.io/nuget/v/OData2Poco.dotnet.o2pgen.svg?label=dotnet%20Global%20Tool&style=flat)](https://www.nuget.org/packages/OData2Poco.dotnet.o2pgen) </p>
[![Chocolatey](https://img.shields.io/chocolatey/v/odata2poco-commandline.svg?label=Chocolatey%20Console)](https://chocolatey.org/packages/odata2poco-commandline)

## Continuous integration
|Branch    |Build status   |
|----------|---------------|
|Master    |[![Build status](https://ci.appveyor.com/api/projects/status/sjaqqu70ex31n8se?svg=true)](https://ci.appveyor.com/project/moh-hassan/odata2poco/branch/master)|
|Develop    |[![Build status](https://ci.appveyor.com/api/projects/status/sjaqqu70ex31n8se?svg=true)](https://ci.appveyor.com/project/moh-hassan/odata2poco/branch/develop)|
-----------

## Development packages
The developed packages can be downloaded from `myget.org`

- [OData2Poco](https://www.myget.org/feed/odata2poco/package/nuget/OData2Poco)
- [OData2Poco.CommandLine](https://www.myget.org/feed/odata2poco/package/nuget/OData2Poco.CommandLine)
- [OData2Poco.dotnet.o2pgen](https://www.myget.org/feed/odata2poco/package/nuget/OData2Poco.dotnet.o2pgen)
--------- 

## Give a Star! :star:

If you are using this project, please show your support by giving this project a star!. Thanks!

-----------
**Features of OData2Poco**
   
- Generate POCO classes corresponding to the Entities defined in the XML MetaData stored in OData Feeds. 
- Generate c# code classes, classes with init-only properties of c#8 and records.
- Generate typescript code as a single file or multi files(modules).
- Generation is based on the Metadata of the service stored on the  server/  EDMX xml files or xml string contents.
- Support http(s) with different methods of authentication. The Supported autherizations are: Basic,Bearer,token,Ntlm,Digest, Custom and Oauth2.
- Console CommandLine tool Support .NET472 or higher.
- Class library Support NET6/netstandard2.0/net461.
- Support Windows or Linux /OS fx (with mono installed) and NET6 (netcore).
- Packaged as a nuget package in three different packages:
 -  A Class library full framework/ netstandard2.0 /NET5 for programming.
 - A console CommandLine tool (one executable file o2pgen.exe)
 -  Global net core support NET6 (dotnet-o2pgen).
 -  Console tool o2pgen is published as a Chocolatey package. 
- Generating CSharp POCO classes. Other languages may be added in the near future based on the community needs.
- Convert Data type of EDMX to the corresponding CLR data types.
- Support Entites, complex data type, Collections  and navigation properties.
- Support OData service version V1..V4
- Code generation is controlled by setting different options: 
  - User defined Atributes for the classes and properties using simple text file with c# Expressions.
  - Built-in Attributes:
        - Add Key Attributes.
        - Add Required Attributes to the properties. 
        - Add JsonProperty Attribute to the properties.
        - Add Table Attribute to the class.
        - Add DataMember Attribute to the properties and DataContract Attribute to the class.
        - Add display attribute to the properties.
        - Add ProtoMember to the properties and ProtoContract to the class to suport Proto Buffer.
        - Add user defined attribute for the properties.
        
   - Adding virtual modifier to the properties.
   - Convert name of properties to camelCase or PasCase
   - Add nullable datatypes, e.g. int?.
   - Generate (or not) navigation properties.
   - Generated class follows inheritance hierarchy of OData feed (unless switched-off).
   - Generated class can inherit from a common BaseClass/interface.
   - Define namespace to overwrite the namespace of the model.
   - Filter Entities.
   - Name Mapping  of Entities and properties using json file with regex support.
- Add primary key/mandatory comments to the properties of the class. 
- Rename class/properties that have a name match a c# reserved keyword.  
- Save metadata and generated code to a user defined file name.
- Support colored console windows /linux /OS fx.
- Support Microsoft.OData.Edm library version 7.5+ (OData v4).
- Support Microsoft.Data.Edm library (OData v1-v3).

 **Features added in V3.2.0:** 

- New: Support Windows NTLM authentication(Thanks to @lobster2012-user for help). 
- New: Support Microsoft Digest authentication.
- New: Support Integrated Windows Authentication, enabling users to log in with their Windows credentials (Issue#19).
- New: Add jsonProperty(originalName) to properties that are renamed because its name is the same as its enclosing type.
- New: Show/hide model warning due to renaming properties/classes whose name is a reserved  keyword.
- New: Support abstract class.
- New: support complex type inheritance
- New: Add attribute [MaxLength] for max length of string/byte[] properties.
- Convert EDM.TIME in Odata v3 to TimeSpan.
- Support multi schema.
- Support multi containers in OData  v3.
- New feature in [v5.0.1](https://github.com/moh-hassan/odata2poco/wiki/v5_0_1)
- New feature in [v6.0](https://github.com/moh-hassan/odata2poco/wiki/v6_0_0).

 -------------
 ## OData2Poco Packages
OData2Poco is available in three flavers:
- A class library: support NET6/netstandard2.0/net461,[download](https://www.nuget.org/packages/OData2Poco).
- A Console tool: OData2Poco.CommandLine support net472 (a.k.a o2pgen), [download](https://www.nuget.org/packages/OData2Poco.CommandLine).
- A .Net Core Global tool  `dotnet-o2pgen` support NET6, [download](https://www.nuget.org/packages/OData2Poco.dotnet.o2pgen).
- Checolatey Console tool, [download](https://community.chocolatey.org/packages/odata2poco-commandline).

----------

## Install and Usage

## 1) OData2Poco.CommandLine o2pgen Console Cli

OData2Poco.CommandLine is a Console application (net472) named o2pgen.

It Can be installed from [Nuget Gallery](https://www.nuget.org/packages/OData2Poco.CommandLine):

       Install-Package OData2Poco.CommandLine 


### Executing o2pgen as MsBuild Target

Add the next xml code to the project.csproj:

```xml
<Target Name="Odata2PocoRun" BeforeTargets="CoreCompile">
         <PropertyGroup>
			<EnableCodeGeneration>true</EnableCodeGeneration>
               <o2pgen>$(PkgOData2Poco_CommandLine)\tools\o2pgen.exe</o2pgen>
               <options>-r http://services.odata.org/V4/Northwind/Northwind.svc/ -f Model\north.cs</options>
		</PropertyGroup>
		<Message Text="Executing o2pgen.exe" Importance="High" />
		<Exec Condition="$(EnableCodeGeneration)
             Command="$(o2pgen)  $(options)" />
</Target>
```

The attribute `Options` is the commandLine arguments. Modify the commandline options as you want.
For more details read [Getting Start](https://github.com/moh-hassan/odata2poco/wiki/getting-start).

### Excuting o2pgen from Package Console Manager (PCM):
In visual studio 2019 and higher, o2pgen can be run directly from PowerShell Console (PCM). Its path is set during installation.

Check application is installed:
```
PM> o2pgen --version
```
### Linux and Mac/OS x support
O2pgen cli can run on Linux and Mac/OS if Mono is installed.

-------------
## 2) OData2Poco global Console Cli (net6.0)

 Install from nuget gallary, run the command:
```
dotnet tool install --global OData2Poco.dotnet.o2pgen
```
### How to use:
Run the command:

       dotnet o2pgen -r http://services.odata.org/V4/Northwind/Northwind.svc/
	   
For help type: `dotnet o2pgen --help`

Review [Commandline option](https://github.com/moh-hassan/odata2poco/wiki/CommandLine-Reference).
 ### Executing the global tool as Msbuild Target

 You can auto run `dotnet o2pgen` from within MsBuild Target and save code in the project folder.

 Add the next Msbuild target to your project and modify command parameters as needed.
 When the property `EnableCodeGeneration` is set to `false`, no code is generated.
 The generated code is saved to file `northwind.cs` in the folder Model in the root of the project.

 ```xml
<Target Name="GenerateCode" BeforeTargets="CoreCompile">
		<PropertyGroup>
			<EnableCodeGeneration>true</EnableCodeGeneration>
		</PropertyGroup>
		<Exec  Condition="$(EnableCodeGeneration)"
		  Command="dotnet o2pgen -r http://services.odata.org/V4/Northwind/Northwind.svc/ -f $(MSBuildProjectDirectory)\Model\northwind.cs -B">
		</Exec>
	</Target>
 ```         
 
## 3) OData2Poco Class library

Support NET6/netstandard2.0/net461.
It can be installed from [Nuget Gallery](https://www.nuget.org/packages/OData2Poco/)

         Install-Package OData2Poco

Try demo Application in NET6 [Online](https://dotnetfiddle.net/LSSwIS)


-------
## 4) Checolatey Package
 
From [Chocolatey Gallery](https://chocolatey.org/packages/odata2poco-commandline):

     choco install odata2poco-commandline

------
 ## Documentation

Read the:[Wiki](https://github.com/moh-hassan/odata2poco/wiki)

-------
**License**

MIT License.

----------
## Release Notes

[Changes](https://github.com/moh-hassan/odata2poco/blob/master/ReleaseNotes.md)

## Privacy Policy
All programs or packages will not transfer any information to other networked systems unless specifically requested by the user or the person installing or operating it. 

## Code Signing Policy
This project uses free code signing provided by [SignPath.io](https://signpath.io?utm_source=foundation&utm_medium=github&utm_campaign=odata2poco), and a certificate by the [SignPath Foundation](https://signpath.org?utm_source=foundation&utm_medium=github&utm_campaign=odata2poco).

Starting with v6.2, o2pgen.exe and all odata2poco.xxx.nupkg packages are signed. Code signing is applied only to odata2poco project code in the [odata2poco repository](https://github.com/moh-hassan/odata2poco) master branch and built on AppVeyor.  The odata2poco.xxx.nupkg packages contain third-party libraries used by odata2poco, which may or may not be signed.  

## Acknowledgements: 

**Thank you [SignPath](https://signpath.io?utm_source=foundation&utm_medium=github&utm_campaign=odata2poco) for free code signing.**

<img src="art/signpath.jpg" alt="signpath" width="200"/>
