# Welcome to OData2Poco
**OData2Poco** is a class library with T4 Template to generate  plain-old CLR objects (**POCO**) from OData feeds . 
POCO classes can be used in typed RESTful client OData services.

A new **OData2Poco.CommandLine (o2pgen)** Console Application is available. 

[![Build Status](https://travis-ci.org/moh-hassan/odata2poco.svg?branch=master)](https://travis-ci.org/moh-hassan/odata2poco)
[![Build status](https://ci.appveyor.com/api/projects/status/sjaqqu70ex31n8se?svg=true)](https://ci.appveyor.com/project/moh-hassan/odata2poco)

**Project Site**: [https://odata2poco.codeplex.com/](https://odata2poco.codeplex.com/)

**Features of OData2Poco**
   
- Generate POCO classes corresponding to the Entities defined in the MetaDataString. **C#** is only supported in this version.     
- Generation   is based on the metadata of the service stored on the  server/ or
 EDMX xml files.
- Support http(s) with/without basic authentication   : user and password
- Convert Data type of EDMX to the corresponding CLR data types
- Support Entites, complex data type, Collections and Enum.
- Exclude navigation properties.
- Use List<T> For Collections.
- Support OData service version 1..4
- Add comments to  the properties of the class which is a primary key / mandatory  in source entities.
- Save metadata to a file.
- T4 template is included.
- Extract http header (for http service).
- Can be installed from nuget.
- Support .NET 4.5 or higher
- Tested with OData feeds.
- CommandLine tool (one executable file o2pgen.exe) is provided.
 
There is a plan to add  extra features .
 I'm waiting the feedback from the community for bug fix and extra features.
 
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
Current version:  v1.3.0

 **How to use**
Install the library from nuget site :
Install-Package OData2Poco

Example 1:
Simply, one line of code and you get CS code

     var code = new O2P(url).Generate();

 
 Example 2 :
 for basic authentication

     var code = new O2P(url,user,password).Generate();

**T4 Template**
you can use the accompanied T4 template and fill values for url (for authentication, user and password )


**Give it a try**
you can use for test the open published OData service,

V4: http://services.odata.org/V4/OData/OData.svc

V3: http://services.odata.org/V3/OData/OData.svc
# Acknowledgements: #
# Thank you [JetBrains](https://www.jetbrains.com "JetBrain") for [Resharper](https://www.jetbrains.com/resharper/ "Resharper") open source license #

![](http://download-codeplex.sec.s-msft.com/Download?ProjectName=odata2poco&DownloadId=1569779)