set merge=..\..\packages\ilmerge.2.14.1208\tools\ilmerge
set bin=../bin/debug/
::%merge% /t exe /targetplatform:"v4,$env:windir\Microsoft.NET\Framework\v4.0.30319" /wildcards /out:o2pgen.exe %bin%o2pgen.exe %bin%*.dll

::%merge% /t exe /lib:"C:\Program Files\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.5" /wildcards /out:o2pgen.exe %bin%o2pgen.exe %bin%*.dll
"F:\codeplex_github\odata2poco\packages\ilmerge.2.14.1208\tools\ILMerge.exe" /out:"F:\codeplex_github\odata2poco\OData2Poco.CommandLine\bin\Debug\O2Pgen.all.exe" "F:\codeplex_github\odata2poco\OData2Poco.CommandLine\bin\Debug\O2Pgen.exe" "F:\codeplex_github\odata2poco\OData2Poco.CommandLine\bin\Debug\*.dll" /target:exe /targetplatform:v4,C:\Windows\Microsoft.NET\Framework\v4.0.30319 /wildcards 
copy "F:\codeplex_github\odata2poco\OData2Poco.CommandLine\bin\Debug\O2Pgen.all.exe"

pause
