
Thanks for installing o2gen :-)
O2Pgen version 2.0.0
o2gen is a CommandLine tool for generating c# POCO classes from OData feeds. Project site: http://odata2poco.codeplex.com

Usage: o2pgen [options]  
  -r, --url         Required. URL of OData feed.
  -u, --user        User name for authentication.
  -p, --password    password for authentication.
  -f, --filename    (Default: poco.cs) filename to save generated c# code.
  -m, --metafile    Xml filename to save metadata.
  -v, --verbose     (Default: False) Prints C# code to standard output.
  -d, --header      (Default: False) List  http header of the service
  -l, --list        (Default: False) List POCO classes to standard output.
  -k, --key         (Default: False) Add Key attribute [Key]
  -t, --table       (Default: False) Add Table attribute
  -q, --required    (Default: False) Add Required attribute
  -n, --Navigation  (Default: False) Add Navigation Properties
  -b, --Nullable    (Default: False) Add Nullable Data Types
  -e  --Eager       (Default: False) Add Non Virtual, eager loading, Navigation Properties 
  -i  --Inherit     A base class and/or interfaces to inherit/implement 
  -m  --namespace   A namespace prefix for the OData namespace
  --help            Display this help screen.


 example
 myprojectfolder\o2p\o2pgen -r http://services.odata.org/V4/OData/OData.svc -f myprojectfolder\northwind.cs
 generate c# POCO clases and write to the file myprojectfolder\northwind.cs

