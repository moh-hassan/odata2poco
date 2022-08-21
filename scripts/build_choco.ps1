#o2pgen.exe.sha256.txt

param ([string]$version = '0.0.0' ,
    [switch]$allow_push = $false)

$pkg_dir= Resolve-Path $PSScriptRoot\..\chocolatey
$build= Resolve-Path $PSScriptRoot\..\build
pushd $build

$sha256= Get-FileHash o2pgen.exe -Algorithm SHA256 
Set-Content -Path 'o2pgen.exe.sha256.txt' -v $sha256.Hash
#Get-Content 'o2pgen.exe.sha256.txt'

#create package
Set-Location $pkg_dir
Remove-Item  *.nupkg
choco pack --version $version

#push package

if ($allow_push) {
    choco push  --api-key $Env:api_key 
    Write-Host "choco package $ver is pushed succeffuly" -ForegroundColor Green
}
else {
    choco push  --api-key $Env:api_key -whatif
}

popd

