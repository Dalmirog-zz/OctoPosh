param(
    [string]$ConfigFile
)

$appConfigFile = resolve-path "$PSScriptRoot\..\appConfig.json"

$config = Get-Content $ConfigFile -Verbose | ConvertFrom-Json

$appConfigJson = get-content $appConfigFile | ConvertFrom-Json 

$appConfigJson.OctopusAdmin = $Config.OctopusAdmin
$appConfigJson.OctopusPassword = $Config.OctopusPassword
$appConfigJson.OctopuswebListenPort = $config.OctopuswebListenPort

try
{
    $appConfigJson | ConvertTo-Json | Set-Content $appConfigFile    
}

catch [System.Exception]
{
    Write-Host "Other exception"
}

Write-Output "Content of [$($AppConfigFile)] file after script:"
Get-Content $AppConfigFile

