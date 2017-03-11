
<<<<<<< HEAD
# OData2Poco.CommandLine (o2pgen) #

##  Version 2.2.0 ##
**Release Date:**   
**What is news:**
- New: Coverting Property Name to camelCase/PasCase using the option  -c, --case camel/pascal.  
- New: Generating JsonProperty Attribute for the property using the option -j, --Json, example: [JsonProperty(PropertyName = "email")]
- New: Adding the option -a, --attribute  to enter all attributes separated by one or more space. Allowed values are:key required json table.
  (equivqlant to -k -q -j -t options )
- Change: the option metafile name changed to x (instead of m)  to be different than the namespace option (m)
- New: Show the option values in the console to help review the options you typed (in verbose mode).




##  Version 2.1.0 ##
**Release Date:**  March 1, 2017
**What is news:**
- New : add option -i, --inherit   for class inheritance from  BaseClass and/or interfaces
- New : add option -m, --namespace     A namespace prefix for the OData namespace 
- New : add option -e, --eager         Add non virtual navigation Properties for Eager Loading

  


##  Version 2.0.0 ##
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


