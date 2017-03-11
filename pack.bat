@echo Off
set config=%1
if "%config%" == "" (
   set config=Release
)

set version=
if not "%PackageVersion%" == "" (
   set version=-Version %PackageVersion%
)

set nuget=
if "%nuget%" == "" (
	set nuget=.nuget\nuget
)

REM Package
mkdir Build
::call merge.bat
%nuget% pack "OData2PocoLib\OData2Poco.nuspec" -verbosity detailed -o Build -p Configuration=%config% %version%
%nuget% pack "OData2Poco.CommandLine\OData2Poco.CommandLine.nuspec" -verbosity detailed -o Build -p Configuration=%config% %version%
::if not "%errorlevel%"=="0" goto failure
pause
 
:success
exit 0

:failure
exit -1

pause
