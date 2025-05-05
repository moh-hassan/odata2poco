$solutionPath = "$PSScriptRoot\.."
$solutionPath="$(Resolve-Path $solutionPath)"
$buildPath= "$solutionPath\build"
$buildPath="$(Resolve-Path $buildPath)"
$zipPath = Join-Path $buildPath "TestResultsAll.zip"
write-host "Creating zip file with Path: $zipPath"
$sevenZipPath = "7z.exe"  
Set-Location $solutionPath

# Create ZIP while preserving full directory structure, including only TestResults folders
Start-Process -FilePath $sevenZipPath -ArgumentList @("a", "`"$zipPath`"", "-r", "-i!*\TestResults\*") -Wait -NoNewWindow 

Write-Host "ZIP file created with TestResults folders for all test projects: $zipPath" -ForegroundColor Green

