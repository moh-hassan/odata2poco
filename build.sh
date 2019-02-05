#!/usr/bin/env bash

#exit if any command fails
set -e

artifactsFolder="./artifacts"

if [ -d $artifactsFolder ]; then
  rm -R $artifactsFolder
fi

dotnet restore


# Build 
dotnet build ./OData2PocoLib -c Release  -f netstandard2.0
#dotnet build ./OData2Poco.CommandLine -c Release -f netcoreapp2.1   #net45
dotnet build OData2Poco.dotnet.o2pgen/OData2Poco.dotnet.o2pgen.csproj -f netcoreapp2.1

#Test
dotnet test OData2Poco.Tests/OData2Poco.Tests.csproj -f netcoreapp2.1
dotnet test OData2Poco.CommandLine.Test/OData2Poco.CommandLine.Test.csproj -f netcoreapp2.1

revision=${TRAVIS_JOB_ID:=1}
revision=$(printf "%04d" $revision) 

#Pack
#dotnet pack ./src/PROJECT_NAME -c Release -o ./artifacts --version-suffix=$revision
