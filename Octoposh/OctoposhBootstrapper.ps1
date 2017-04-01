##This script will get executed everytime the module gets imported

$env:OctoposhModulePath = $PSScriptRoot
$env:NugetRepositoryURL = "https://packages.nuget.org/api/v2" #"C:\Github\OctoPosh\Octoposh.Tests\TestAssets\Repository"