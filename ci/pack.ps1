msbuild ./OData2Poco.CommandLine/OData2Poco.CommandLine.csproj  /t:pack  -p:configuration=release  -p:NoBuild=true
msbuild ./OData2Poco.dotnet.o2pgen/OData2Poco.dotnet.o2pgen.csproj /t:pack  -p:configuration=release  -p:NoBuild=true
dir .\build
