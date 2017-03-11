@echo Off
set ilmerge=ILMerge\tools\ilmerge.exe
::set SolutionDir=.
set TargetDir=OData2Poco.CommandLine\bin\Release
set TargetName=o2pgen
md OData2Poco.CommandLine\bin\o2p
set outdir=OData2Poco.CommandLine\bin\o2p
echo deleting old version: %outdir%\o2pgen.*
del %outdir%\o2pgen.*
::dir %outdir%\o2pgen.exe
echo start merging binaries in folder:
echo %TargetDir%
%ilmerge% /out:"%outdir%\o2pgen.exe" "%TargetDir%\%TargetName%.exe" "%TargetDir%\*.dll" /target:exe /targetplatform:v4,C:\Windows\Microsoft.NET\Framework\v4.0.30319 /wildcards 
echo Show information of: %outdir%\o2pgen.exe
dir %outdir%\o2pgen.exe
pause
 