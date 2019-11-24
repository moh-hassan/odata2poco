# Welcome to OData2Poco
**OData2Poco** is a code generation tool for generating plain-old CLR objects (POCO) from OData feeds. 
POCO classes can be used in a typed RESTful client OData services and code generation can be controlled by setting many options.

OData2Poco is available in three flavers:

- A Console tool: OData2Poco.CommandLine (a.k.a o2pgen).
- A .Net Core Global tool  **dotnet-o2pgen** support netcoreapp2.1.
- A class library: support net45/net461/netstandard2.0.

[![NuGet Version](https://img.shields.io/nuget/v/OData2Poco.CommandLine.svg?label=nuget%20Console&style=flat)](https://www.nuget.org/packages/OData2Poco.CommandLine)
[![Chocolatey](https://img.shields.io/chocolatey/v/odata2poco-commandline.svg?label=Chocolatey%20Version)](https://chocolatey.org/packages/odata2poco-commandline)
[![Global Tool NuGet Version](https://img.shields.io/nuget/v/OData2Poco.dotnet.o2pgen.svg?label=dotnet%20Global%20Tool&style=flat)](https://www.nuget.org/packages/OData2Poco.dotnet.o2pgen)

## Continuous integration
|Build server                |Platform     |Build status                                                |
|----------------------------|-------------|------------------------------------------------------------|
|AppVeyor                    |Windows      |[![Build status](https://ci.appveyor.com/api/projects/status/sjaqqu70ex31n8se?svg=true)](https://ci.appveyor.com/project/moh-hassan/odata2poco)|
|Travis                      |Linux / OS X |[![Build Status](https://travis-ci.org/moh-hassan/odata2poco.svg?branch=master)](https://travis-ci.org/moh-hassan/odata2poco)|

### OData2Poco V3.3.1 is Released on Nov 25, 2019

## What is new in V3.3.1
- Fix Issue [#28](https://github.com/moh-hassan/odata2poco/issues/28#issuecomment-557015613): Apply change-case on Navigation Properties.


## What is new in V3.3.0
- Filter model using the option [--include](https://github.com/moh-hassan/odata2poco/wiki/CommandLine-Reference#--include) 
- Change case of Classes to Camel/Pas using the option --entity-case
- Generated ReadOnly Properties if the vocabulary of the metadata include:Computed or Permissions:Read.
- Removing Vb conversion external service.


Try the new feature by the command
``` 
o2pgen -r http://services.odata.org/V4/TripPinServiceRW -v --include air* --entity-case camel
```

***The new version 3.3.1 can be downloaded from Nuget:*** 

- [Console (o2pgen)](https://www.nuget.org/packages/OData2Poco.CommandLine/3.3.1)
- [Netcore Global tool (dotnet-o2pgen)](https://www.nuget.org/packages/OData2Poco.dotnet.o2pgen)
- [Class libraryl](https://www.nuget.org/packages/OData2Poco/3.3.1)


**Features of OData2Poco**
   
- Generate POCO classes corresponding to the Entities defined in the XML MetaData stored in OData Feeds. *     
- Generation   is based on the Metadata of the service stored on the  server/ or  EDMX xml files.
- Support http(s) with/without authentication. The Supported autherizations are: basic, token and Oauth2.
- Support .NET 4.5 or higher
- Support netstandard2.0 /netcoreapp2.1
- Support Windows or Linux /OS fx (net core) 
- Packaged as a nuget package in three different packages:
 -  A Class library full framework/ netstandard2.0 for programming.
 -  A  console CommandLine tool (one executable file o2pgen.exe)
 -  Global net core. dotnet-o2pgen.
 -  o2pgen is published as a Chocolatey package. 
- Generating CSharp POCO classes and vb.net. Other languages may be added in the near future based on the community needs.
- Convert Data type of EDMX to the corresponding CLR data types.
- Support Entites, complex data type, Collections  and navigation properties.
- Support OData service version V1..V4
- Code generation is controlled by setting different options: 
  - Add the following attributes:
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

- Add primary key/mandatory comments to the properties of the class. 
- Rename class/properties that have a name match a c# reserved keyword.  .
- Save metadata and generated code to a user defined file name.
- Support colored console windows /linux /OS fx.
- Support Microsoft.OData.Edm library version 7.5+ (OData v4).
- Support Microsoft.Data.Edm library (OData v1-v3).
- MIT License. 

 **Features added in V3.2.0:** 

- New: Support Windows NTLM authentication(Thanks to @lobster2012-user for help). 
- New: Support Microsoft Digest authentication.
- New: Support Integrated Windows Authentication, enabling users to log in with their Windows credentials (Issue#19).
- New: Add jsonProperty(originalName) to properties that are renamed because its name is the same as its enclosing type.
- New: Show/hide model warning due to renaming properties/classes whose name is a reserved  keyword.
- New: Support abstract class.
- New: support complex type inheritance
- New: Add attribute [MaxLength] for max length of string/byte[] properties.
- Fix: Convert EDM.TIME in Odata v3 to TimeSpan  (Issue#18).
- Fix: Support multi schema (Issue#20).
- Fix: Support multi containers in OData  v3.

 
## Install

**OData2Poco.CommandLine (o2pgen) console application:** 

From [Nuget Gallery](https://www.nuget.org/packages/OData2Poco.CommandLine):

       Install-Package OData2Poco.CommandLine 

From [Chocolatey Gallery](https://chocolatey.org/packages/odata2poco-commandline):

     choco install odata2poco-commandline

**OData2Poco.dotnet.o2pgen (dotnet-o2pgen) NetCore Global tool:** 

From [Nuget Gallery](https://www.nuget.org/packages/OData2Poco.dotnet.o2pgen):

       dotnet tool install --global OData2Poco.dotnet.o2pgen  

**OData2Poco Class library:** 

From [Nuget Gallery](https://www.nuget.org/packages/OData2Poco/)

         Install-Package OData2Poco


**Release Notes** [Review](https://github.com/moh-hassan/odata2poco/blob/master/ReleaseNotes.md)


 **Try it:**
 dotnet Global Tool: 
 Install from nuget gallary, run the command:

       dotnet o2pgen -r http://services.odata.org/V4/Northwind/Northwind.svc/
	   For help type: dotnet o2pgen --help

Consol net45 tool:

       o2pgen -r http://services.odata.org/V4/Northwind/Northwind.svc/
 
 Note: The same options are available for dotnet Global tool or Console tool

 **Documentation**

Read the:[Wiki](https://github.com/moh-hassan/odata2poco/wiki)

## Acknowledgements: 

**Thank you [JetBrains](https://www.jetbrains.com "JetBrain") for [Resharper](https://www.jetbrains.com/resharper/ "Resharper") open source license**

![](art/resharper2.jpg)
