

# OData2Poco.CommandLine 

##   (Console: o2pgen / .Net Core: dotnet-o2pgen) 

##  Version 3.1.0  
**Release Date:**   March 4, 2019

**What is news:**

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

##  Version 3.0.0  
**Release Date:**   Jan 30, 2019

**What is news:**

  - New: Support NetStandard2.0 and net45
  - New: .Net Core Global tool  dotnet-o2pgen
 - New: Support Colored Console.
 - New: Add attributes Datamaember (dm) ,Db to add Key/Table/Required attributes,Display attribute  and proto attribute to suport Proto Buffer
 - obsolete: Removing the opptions -k -json -table -required and replaced by one option -a to generate attributes
  - Maintainance: Migrating to vs2017,  the new SDK project style and c#7+.
  - Refactoring attributes to add plugin attributes based on standard interface.

##  Version 2.3.0  

**Release Date:**   Dec 21, 2017

**What is news:**
 - Add inheritance support by default.
   Generated class follows inhertance hierarchy of OData feed (unless switched-off by -i option)

##  Version 2.2.1  
**Release Date:**   
**What is news:**
- Support Nullable Data type: DateTime (issue #3), DateTimeOffset, TimeSpan, Guid

##  Version 2.2.0  

**Release Date:**   March 11, 2017

**What is news:**

- New: Coverting Property Name to camelCase/PasCase using the option  -c, --case camel/pascal (issue #1).  
- New: Generating JsonProperty Attribute for the property using the option -j, --Json, example: [JsonProperty(PropertyName = "email")] (issue #1)
- New: Adding the option -a, --attribute  to enter all attributes separated by one or more space. Allowed values are:key required json table.
  (equivqlant to -k -q -j -t options )
- Change: the option metafile name changed to x (instead of m)  to be different than the namespace option (m)
- New: Show the option values in the console to help review the options you typed (in verbose mode).


##  Version 2.1.0 ##
**Release Date:**  February 28, 2017

**What is news:**

- New : add option -i, --inherit   for class inheritance from  BaseClass and/or interfaces (issue #2)
- New : add option -m, --namespace     A namespace prefix for the OData namespace  (issue #2)
- New : add option -e, --eager         Add non virtual navigation Properties for Eager Loading (issue #2)


##  Version 2.0.0 ##
**Release Date:** June 27, 2016

**What is news:**

- New: add option -k  to add KeyAttribute to the property 
- New: add option -q to add RequiredAttribute to the property 
- New: add option -t to  add TableAttribute to the class 
- New: add option - n to add Navigation properties to the class
- New: add option to generate nullable data types
- Return exitcode -1 for http  exceptions errors 401, 404
- Add more unit test

##  Version 1.4.1 ##

**Release Date:**  May 17, 2016

- fix : Issue: popup error screen is raised for not_found and not_authorized http Exception error. 
	    corrected: Application return exit code -1 and handling  http excptions 



## Version 1.4.0 ##

**Release Date:**  May 1, 2016

First Release.

Available Options:
-r, --url Required. URL of OData feed.
-u, --user User name for authentication.
-p, --password password for authentication.
-f, --filename (Default: poco.cs) filename to save generated c# code.
-m, --meta-file  filename to save XML metadata.
-v, --verbose (Default: False) Prints C# code to the standard output.
-d, --header (Default: False) List http header of the service
-l, --list (Default: False) List POCO classes to standard output.
--help Display this help screen.


