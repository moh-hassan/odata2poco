param ([string]$version)
$root= Resolve-Path $PSScriptRoot\..\Build
$choco_pkg= Resolve-Path $PSScriptRoot\..\chocolatey
$fname= "$root\schecksum256-V$version.txt"

$s = {
param ($root)
ls $root |? {$_.Extension -in ".nupkg" , ".exe"} |
 %{$_.FullName} |
 %{Get-FileHash $_ -Algorithm SHA256} |
 Select-Object -Property @{
                 name='FileName'
                 expression= { ( Split-Path -path $_.Path -Leaf)}                 
               }, 
               @{
                 name='Size'
                 expression= { "{0:N0} [bytes]" -f ((Get-Item $_.Path).Length)}                 
               },
               Hash 
 }

 Set-Content -Path $fname -Value "CheckSum 256 of Release: $version"  
 $s1 = & $s $root
 $s2 = & $s $choco_pkg
 $source = $s1 +$s2
 $source | Format-List | Out-String |Add-Content $fname

Get-Content $fname

           
              
               

 

