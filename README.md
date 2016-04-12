# odata2poco
Generate "plain-old" CLR objects (POCO) Classes from OData Service
OData2Poco is a class library supporting .NET 4.5 or higher with T4 Template to generate  plain-old CLR objects (POCO) from OData feeds that implement both V1-3 and V4 OData protocol, based on the metadata of the service stored on the  server. 
POCO classes can be used in typed RESTful client OData services

Project Site:
https://odata2poco.codeplex.com

Features of OData2PocoLib:
   - Generate POCO classes corresponding to the Entities defined in the MetaDataString. Csharp is only supported in this version.
   - Support http(s) with/without basic authentication : user and password
   - Support metadata stored in files.
   - Convert Data type of EDMX to the corresponding CLR data types.
   - Support Entites , complex data type ,Collections and Enum.
   - Excluding navigation properties.
   - Use List<T> For Collections.
   - Support OData service version 1..4
   - Add comments to  the properties of the class which is primary key and mandatory  in source entities.
   - Save metadata to a file.
   - T4 template is included.
   - Extract http header (for http service).
   - Can be downloaded from nuget.
   -Tested with OData feeds. 

The current version is generating C Sharp classes. 

.NET Requirements:
OData2Poco requires .Net 4.5 or higher.  

Dependency

    Microsoft.Data.Edm  version="5.7.0"  or higher
    Microsoft.OData.Edm  version="6.15.0" or higher 
note: The generated POCO classes code need not these EDM dependency libraries when POCO is used in your project.

Nuget Package
OData2Poco is packaged to nuget site

How to use
Download the library from nuget site
Install-Package OData2Poco

Example 1:
Simply, one line of code and you get CS code
 var code = new O2P(url).Generate();
 
 Example 2 :
 for basic authentication
     var code = new O2P(url,user,password).Generate();

T4 Template
you can use the accompanied T4 template and fill values for url (for authentication, user and password )

Give it a try
you can use for test the open published OData service:
for V4: http://services.odata.org/V4/OData/OData.svc
for V3: http://services.odata.org/V3/OData/OData.svc

 
