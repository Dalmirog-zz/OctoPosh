#This script grabs info from the build config file at the root of the solution and feeds info to the app.config file of a project

param(
    [string]$ConfigFile
)

$config = Get-Content $ConfigFile -Verbose | ConvertFrom-Json

$ConnectionString = $config.ConnectionString

$AppConfigFile = resolve-path "$PSScriptRoot\..\App.config"

[xml]$content = Get-Content $AppConfigFile

$content.configuration.connectionStrings.add | ?{$_.name -eq "OctopusDatabase"} | %{$_.connectionString = $ConnectionString}

$content.Save($AppConfigFile)

Write-Output "Content of the app.config file after script:"

Get-Content $AppConfigFile
