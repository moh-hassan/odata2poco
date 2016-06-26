
<<<<<<< HEAD
# OData2Poco.CommandLine (o2pgen) #

##  Version 1.4.1 ##
**Release Date:**  May 17, 2016
- fix : Issue#1: popup error screen is raised for not_found and not_authorized http Exception error. 
	        corrected: Application return exit code -1 and handling  http excptions 


## Version 1.4.0 ##
**Release Date:** May 1, 2016
=======
# OData2Poco.CommandLine #

##  Version 1.5.0 ##
**Release Date:**  

**What is news:**

- New feature: add option -k  to add KeyAttribute to the property 
- New feature: add option -q to add RequiredAttribute to the property 
- New feature: add option -t to  add TableAttribute to the class 
- New feature: add option - n to add Navigation properties to the class
- Return exitcode -1 for http  exceptions errors 401, 404
- Add more unit test



## Version 1.4.0 ##
**Release Date:** Sunday, May 1, 2016
>>>>>>> develop

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


