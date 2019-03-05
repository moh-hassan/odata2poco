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

### OData2Poco V3.1 is Released on March 4, 2019

# **The new Features in V3.1.0:** #

 - New: Support Oauth2 by generating token for `client_credentials` having `client_id and  client_secret`.
  - New: Support token authentication (including oauth2 with `access_token`). The token can be read from text file or json file. 
  - New: add new option `-o, --auth` to control authentication: none,basic,token and oauth2.
  - New options: `--token-endpoint` to get OAuth2 Token Endpoint, `--token-params` to get OAuth2 Token Parameters.
  - New: Renaming classes/properties that have a name match a reserved c# keywords.
  - New: Renaming properties that have a name match its class type to avoid the compiler error CS0542.
  - New: Renaming enum elements that match reserved c# keywords.
  - New: add new option '-g, --generate' to generate a project Csproj/vbproj with a multi-target framework (vs2017 SDK style).
  - Fix: issue#12: POCO compile errors.
  - Tested with Dynamics 365 for Finance and Operations and Dynamics 365 CRM.

***The new version 3.1.0.360 can be downloaded from:*** [github](https://github.com/moh-hassan/odata2poco/releases "github")

----------


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

 
## Install

**OData2Poco.CommandLine (o2pgen) console application:** 

From [Nuget Gallery](https://www.nuget.org/packages/OData2Poco.CommandLine):
 ![](http://download-codeplex.sec.s-msft.com/Download?ProjectName=odata2poco&DownloadId=1567512)

From [Chocolatey Gallery](https://chocolatey.org/packages/odata2poco-commandline):

     choco install odata2poco-commandline

**OData2Poco Class library:** 

From [Nuget Gallery](https://www.nuget.org/packages/OData2Poco/)
![enter image description here](http://download-codeplex.sec.s-msft.com/Download?ProjectName=odata2poco&DownloadId=1562964)



**Release Notes**
https://github.com/moh-hassan/odata2poco/blob/master/ReleaseNotes.md


 **Try it:**
 dotnet Global Tool: 
 Install from nuget gallary
       dotnet o2pgen -r http://services.odata.org/V4/Northwind/Northwind.svc/
	   For help type: dotnet o2pgen --help

Consol net45 tool
       o2pgen -r http://services.odata.org/V4/Northwind/Northwind.svc/
 
 Note: The same options are available for dotnet Global tool or Console tool

 **How to use**

Read the documentation:[Wiki](https://github.com/moh-hassan/odata2poco/wiki)

## Acknowledgements: 

**Thank you [JetBrains](https://www.jetbrains.com "JetBrain") for [Resharper](https://www.jetbrains.com/resharper/ "Resharper") open source license**

![](http://download-codeplex.sec.s-msft.com/Download?ProjectName=odata2poco&DownloadId=1569779)
