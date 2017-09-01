##This script will get executed everytime the module gets imported

$env:OctoposhModulePath = $PSScriptRoot

#URL that will be used by Octoposh to download Octo.exe. Do not modify unless you know what you are doing :)
$env:NugetRepositoryURL = "https://packages.nuget.org/api/v2"