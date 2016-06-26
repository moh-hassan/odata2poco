
# OData2Poco.CommandLine #

##  Version 2.0.0 ##
**Release Date:**  

**What is news:**

- New feature: add option -k  to add KeyAttribute to the property 
- New feature: add option -q to add RequiredAttribute to the property 
- New feature: add option -t to  add TableAttribute to the class 
- New feature: add option - n to add Navigation properties to the class
- New feature: add option to generate nullable data types
- Return exitcode -1 for http  exceptions errors 401, 404
- Add more unit test



## Version 1.4.0 ##
**Release Date:** Sunday, May 1, 2016

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


