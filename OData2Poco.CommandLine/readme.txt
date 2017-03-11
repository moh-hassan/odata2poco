Thanks for installing the CommandLine tool (o2gen)
O2Pgen version 2.2.0
o2gen is a CommandLine tool for generating c# POCO classes from OData feeds. Project site: http://odata2poco.codeplex.com

Usage: o2pgen [options]  
  
  -r, --url           Required. URL of OData feed.
  -u, --user          User name for authentication.
  -p, --password      password for authentication.
  -f, --filename      (Default: poco.cs) filename to save generated c# code.
  -x, --metafile      Xml filename to save metadata.
  -v, --verbose       (Default: False) Prints C# code to standard output.
  -d, --header        (Default: False) List  http header of the service
  -l, --list          (Default: False) List POCO classes to standard output.
  -k, --key           (Default: False) Add Key attribute [Key]
  -t, --table         (Default: False) Add Table attribute
  -q, --required      (Default: False) Add Required attribute
  -n, --navigation    (Default: False) Add navigation properties
  -e, --eager         (Default: False) Add non virtual navigation Properties for Eager Loading
  -b, --nullable      (Default: False) Add nullable data types
  -i, --inherit       for class inheritance from  BaseClass and/or interfaces
  -m, --namespace     A namespace prefix for the OData namespace
  -c, --case          (Default: none) Type pas or camel to Convert Property Name to PascalCase or 
                      CamelCase
  -a, --attribute     Type all attributes separated by one or more space. 
                      Allowed words are:key required json table.
  -j, --Json          (Default: False) Add JsonProperty Attribute, 
                      example: [JsonProperty(PropertyName = "email")]
  --help              Display this help screen.


 example
 myprojectfolder\o2p\o2pgen -r http://services.odata.org/V4/OData/OData.svc -f myprojectfolder\northwind.cs
 generate c# POCO clases and write to the file myprojectfolder\northwind.cs

