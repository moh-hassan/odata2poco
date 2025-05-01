###############################################################################
### Script to start mitmdump with authentication. 
###############################################################################

$folderPath = "$PSScriptRoot\..\build\tool"
$zipUrl = "https://downloads.mitmproxy.org/12.0.0/mitmproxy-12.0.0-windows-x86_64.zip" 
$zipFile = "$folderPath\mitmproxy.zip"
$appExe = "$folderPath\mitmdump.exe"
$port=8888

New-Item -ItemType Directory -Path $folderPath -Force > $null

if (-Not (Test-Path $zipFile)) {
    Write-Host "Downloading mitmdump zip file..."
    #$ProgressPreference = 'SilentlyContinue'
    Invoke-WebRequest -Uri $zipUrl -OutFile $zipFile  -UseBasicParsing
    Write-Host "Download complete. Extracting files..."
    Expand-Archive -Path $zipFile -DestinationPath $folderPath -Force
 } 

# Check if mitmdump.exe is already running
if (Get-Process -Name "mitmdump" -ErrorAction SilentlyContinue) {
    Write-Host "mitmdump is already running. Skipping process start."
} else {
    $process = Start-Process -FilePath "$folderPath\mitmdump.exe" -ArgumentList "-p $port --proxyauth `"user:password`" --set block_global=false" -WindowStyle hidden -PassThru
    Start-Sleep -Seconds 3
    Write-Host "mitmdump started on port $port with process id $($process.Id)"
}
