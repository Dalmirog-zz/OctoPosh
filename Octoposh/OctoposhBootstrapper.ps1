##This script will get executed everytime the module gets imported

$env:OctoposhModulePath = $PSScriptRoot
$env:NugetRepositoryURL = "C:\Github\OctoPosh\Octoposh.Tests\TestAssets\Repository" #"https://packages.nuget.org/api/v2"