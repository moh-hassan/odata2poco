**Introduction**

OData2Poco.CommandLine (o2pgen.exe) enables the developer to easily generate POCO C# Classes from URL of OData feeds from within the development environment using the Package Manager.

o2pgen.exe is only one executable file including all dll dependency libraries.
o2pgen support all features available in OData2Poco class library.

**How to use**

Generating POCO C# classes from OData feeds is two step

***step1:***

Install-Package OData2Poco.CommandLine
the tool o2pgen.exe will be installed in the folder o2p in the root of the project

***step2:***

In the Package Manager type :
<your_path>\o2pgen <options>

***example***

for the project named ODataDemo
odatademo\o2p\o2pgen -r http://services.odata.org/V4/OData/OData.svc -f odatademo\northwind.cs

**Options:**

-r, --url Required. URL of OData feed.
-u, --user User name for authentication.
-p, --password password for authentication.
-f, --filename (Default: poco.cs) filename to save generated c# code.
-m, --meta-file  filename to save XML metadata.
-v, --verbose (Default: False) Prints C# code to the standard output.
-d, --header (Default: False) List http header of the service
-l, --list (Default: False) List POCO classes to standard output.
--help Display this help screen.


