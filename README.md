# Welcome to OData2Poco
**OData2Poco** is a class library with T4 Template to generate  plain-old CLR objects (**POCO**) from OData feeds . 
POCO classes can be used in typed RESTful client OData services.

Also, **OData2Poco.CommandLine (o2pgen)** Console Application is available to generate POCO with different options. 

[![Build Status](https://travis-ci.org/moh-hassan/odata2poco.svg?branch=master)](https://travis-ci.org/moh-hassan/odata2poco)
[![Build status](https://ci.appveyor.com/api/projects/status/sjaqqu70ex31n8se?svg=true)](https://ci.appveyor.com/project/moh-hassan/odata2poco)

**Project Site**: [https://odata2poco.codeplex.com/](https://odata2poco.codeplex.com/)

**Features of OData2Poco**
   
- Generate POCO classes corresponding to the Entities defined in the MetaDataString. **C#** is only supported in this version.     
- Generation   is based on the Metadata of the service stored on the  server/ or  EDMX xml files.
- Support http(s) with/without basic authentication   : user and password
- Convert Data type of EDMX to the corresponding CLR data types
- Support Entites, complex data type, Collections  and navigation properties.
- Support OData service version 1..4
- Add comments to  the properties of the class which is a primary key / mandatory  in source entities.
- Code generation is controlled by setting different options.
- Save metadata to a file.
- T4 template is included.
- Support .NET 4.5 or higher
- Packaged as a Class library and CommandLine tool (one executable file o2pgen.exe).Packages can be installed from nuget site.
 

 
**Download** OData2Poco from [codeplex.com](http://odata2poco.codeplex.com) or install using [nuget](https://www.nuget.org/packages/OData2Poco/)
![enter image description here](http://download-codeplex.sec.s-msft.com/Download?ProjectName=odata2poco&DownloadId=1562964)

**OData2Poco.CommandLine (o2pgen) console application**

![](http://download-codeplex.sec.s-msft.com/Download?ProjectName=odata2poco&DownloadId=1567512)

**.NET Requirements**
OData2Poco requires .Net 4.5 or higher.  

**Dependency**
Microsoft.Data.Edm  version="5.7.0"  or higher
Microsoft.OData.Edm  version="6.15.0" or higher 
note: The generated POCO classes code need not these EDM dependency libraries when POCO is used in your project.

**Release Notes**

- v 2.1.0 March 1, 2017
- v 2.0.0 June 27, 2016
- v 1.3.0 April 10, 2016

**Latest Changes**

- Merged pull request #2 "Add option for non-virtual navigation properties, namespace and inherit BaseClass/Interface", thanks to @rsouthworth.

 **How to use**

Read the documentation:[http://odata2poco.codeplex.com/documentation](http://odata2poco.codeplex.com/documentation "documentation")


**Give it a try**

You can use for test the open published OData service,

V4: http://services.odata.org/V4/OData/OData.svc

V3: http://services.odata.org/V3/OData/OData.svc
# Acknowledgements: #
# Thank you [JetBrains](https://www.jetbrains.com "JetBrain") for [Resharper](https://www.jetbrains.com/resharper/ "Resharper") open source license #

![](http://download-codeplex.sec.s-msft.com/Download?ProjectName=odata2poco&DownloadId=1569779)