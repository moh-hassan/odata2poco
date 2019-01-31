Thanks for installing the CommandLine tool (o2gen)
O2Pgen version 3.0.0
o2gen is a CommandLine tool for generating c# POCO classes from OData feeds. Project site: http://odata2poco.codeplex.com

Usage: o2pgen [options]  
  
  -r, --url           Required. URL of OData feed.
  -u, --user          User name for authentication.
  -p, --password      password for authentication.
  -f, --filename      (Default: poco.cs) filename to save generated c# code.
  -x, --metafile      Xml filename to save metadata.
  -v, --verbose       Prints C# code to standard output.
  -d, --header        List  http header of the service
  -l, --list          List POCO classes to standard output.
  -n, --navigation    Add navigation properties
  -e, --eager         Add non virtual navigation Properties for Eager Loading
  -b, --nullable      Add nullable data types
  -i, --inherit       for class inheritance from  BaseClass and/or interfaces
  -m, --namespace     A namespace prefix for the OData namespace
  -c, --case          (Default: none) Type pas or camel to Convert Property
                      Name to PascalCase or CamelCase
  -a, --attribute     Attributes, Allowed values: key, req,
                      json,tab,dm,proto,db,display
  --lang              (Default: cs) Type cs for CSharp, vb for VB.NET
  -k, --key           Obsolete, use -a key, Add Key attribute [Key]
  -t, --table         Obsolete, use -a tab, Add Table attribute
  -q, --required      Obsolete, use -a req, Add Required attribute
  -j, --Json          Obsolete, use -a json, Add JsonProperty Attribute,
                      example:  [JsonProperty(PropertyName = "email")]
  --help              Display this help screen.
  --version           Display version information.



 example
 myprojectfolder\o2p\o2pgen -r http://services.odata.org/V4/OData/OData.svc -f myprojectfolder\northwind.cs
 generate c# POCO clases and write to the file myprojectfolder\northwind.cs

