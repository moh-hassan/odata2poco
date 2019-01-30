# Welcome to OData2Poco
**OData2Poco** is a code generation tool for generating plain-old CLR objects (POCO) from OData feeds. 
POCO classes can be used in a typed RESTful client OData services and code generation can be controlled by setting many options.

OData2Poco is available in three flavers:

- A Console tool: OData2Poco.CommandLine (a.k.a o2pgen).
- A .Net Core Global tool  **dotnet-o2pgen** support netcoreapp2.1.
- A class library: with T4 template.

[![NuGet Version](https://img.shields.io/nuget/v/OData2Poco.svg?label=Nuget%20Version)](https://www.nuget.org/packages/OData2Poco.CommandLine)
[![Chocolatey](https://img.shields.io/chocolatey/v/odata2poco-commandline.svg?label=Chocolatey%20Version)](https://chocolatey.org/packages/odata2poco-commandline)
[![Global Tool NuGet Version](https://img.shields.io/nuget/v/OData2Poco.svg?label=Global%20Tool%20Version)](https://www.nuget.org/packages/OData2Poco.CommandLine)

## Continuous integration
|Build server                |Platform     |Build status                                                |
|----------------------------|-------------|------------------------------------------------------------|
|AppVeyor                    |Windows      |[![Build status](https://ci.appveyor.com/api/projects/status/sjaqqu70ex31n8se?svg=true)](https://ci.appveyor.com/project/moh-hassan/odata2poco)|
|Travis                      |Linux / OS X |[![Build Status](https://travis-ci.org/moh-hassan/odata2poco.svg?branch=master)](https://travis-ci.org/moh-hassan/odata2poco)|

## Nightly build v3.0.0-Rc1
[Download V3.0.0:](https://github.com/moh-hassan/odata2poco/releases)

# **The new Features in V3.0.0:** #

-  New: Support NetStandard2.0 and net45
-  New: .Net Core Global tool  **dotnet-o2pgen** support netcoreapp2.1
-  New: Generating VB.Net code
-  New: Support Colored Console.
-  New: Add attributes 
  -  dm DataMember / DataContract
  -  db to add Key/Table/Required attributes.
  -  display attribute
  -  proto attribute to suport Proto Buffer
-  Obsolete: Removing these options:
   - option -k is replaced by the option -a key 
   - option -json is replaced by -a json
   - option -table is replaced by -a tab
   - option -required is replaced by -a req
- New: More than one attribute can be passed in one option: -a key json req
- Updata to Odata version 7+
-   Re-factoring Attributes to be added as a plugin based on standard interface.
-   Maintenance: Migrating to vs2017,  the new SDK project style and c#7+.

----------


**Features of OData2Poco**
   
- Generate POCO classes corresponding to the Entities defined in the XML MetaData stored in OData Feeds. *     
- Generation   is based on the Metadata of the service stored on the  server/ or  EDMX xml files.
- Support http(s) with/without basic authentication   : user and password.
- Convert Data type of EDMX to the corresponding CLR data types.
- Support Entites, complex data type, Collections  and navigation properties.
- Support OData service version V1..V4
- Code generation is controlled by setting different options:   
   - Add Key, Required Attributes to the properties. 
   - Add JsonProperty Attribute to the properties.
   - Add Table Attribute to the class.
   - Adding virtual modifier to the properties.
   - Convert name of properties to camelCase or PasCase
   - Add nullable datatypes, e.g. int?.
   - Generate (or not) navigation properties.
   - Generated class follows inhertance hierarchy of OData feed (unless switched-off).
   - Generated class can inherit from a common BaseClass/interface.
   - Define namespace to overwrite the namespace of the model.

- Add primary key/mandatory comments to the properties of the class.   .
- Save metadata and generated code to a user defined file name.
- Support .NET 4.5 or higher
- Support Windows or Linux (Mono)
- Packaged as a Class library and CommandLine tool (one executable file o2pgen.exe) and can be installed from nuget web site.
- Generating CSharp POCO classes. Other languages may be supported in the near future based on the community needs.
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


## .NET Requirements

OData2Poco requires .Net 4.5 or higher.  

**Dependency of Odata2Poco Class Library**

Microsoft.Data.Edm  version="5.7.0"  or higher

Microsoft.OData.Edm  version="6.15.0" or higher 

**Note:** The generated POCO classes code need not these EDM dependency libraries when POCO is used in your project.

**Release Notes**

- v 2.2.1 March 21, 2017 (last release)
- v 2.2.0 March 11, 2017
- v 2.1.0 February 28, 2017
- v 2.0.0 June 27, 2016
- v 1.3.0 April 10, 2016

**Latest Changes**

v2.2.1: Support Nullable Data type: DateTime (issue #3), DateTimeOffset, TimeSpan, Guid.

[All Changes](ReleaseNotes.md)

 **Try it:**
 
       o2pgen -r http://services.odata.org/V4/Northwind/Northwind.svc/
 **How to use**

Read the documentation:[Wiki](https://github.com/moh-hassan/odata2poco/wiki)

## Acknowledgements: 

**Thank you [JetBrains](https://www.jetbrains.com "JetBrain") for [Resharper](https://www.jetbrains.com/resharper/ "Resharper") open source license**

![](http://download-codeplex.sec.s-msft.com/Download?ProjectName=odata2poco&DownloadId=1569779)
