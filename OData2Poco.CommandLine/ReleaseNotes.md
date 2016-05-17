
# OData2Poco.CommandLine (o2pgen) #

##  Version 1.4.1 ##
**Release Date:**  May 17, 2016
- fix : Issue#1: popup error screen is raised for not_found and not_authorized http Exception error. 
	        corrected: Application return exit code -1 and handling  http excptions 


## Version 1.4.0 ##
**Release Date:** May 1, 2016

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


