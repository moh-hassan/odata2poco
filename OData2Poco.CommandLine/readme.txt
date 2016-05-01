
Thanks for installing o2gen :-)
O2Pgen version 1.4.0
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
  --help            Display this help screen.


 example
 myprojectfolder\o2p\o2pgen -r http://services.odata.org/V4/OData/OData.svc -f myprojectfolder\northwind.cs
 generate c# POCO clases and write to the file myprojectfolder\northwind.cs

