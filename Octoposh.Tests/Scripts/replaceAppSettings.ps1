param(
    [string]$ConfigFile
)

$config = Get-Content $ConfigFile -Verbose | ConvertFrom-Json

$OctopusBindingPort= $config.OctopuswebListenPort
$OctopusAdmin = $config.OctopusAdmin.ToString()
$OctopusPassword = $config.OctopusPassword.ToString()

$AppConfigFile = resolve-path "$PSScriptRoot\..\App.config" #Relative to the location of the .cake script, NOT to the location of this ps1 script.

[xml]$content = Get-Content $AppConfigFile

$content.configuration.appSettings.add | Where-Object{$_.key -eq "OctopusAdmin"} | ForEach-Object{$_.value = $OctopusAdmin}
$content.configuration.appSettings.add | Where-Object{$_.key -eq "OctopusPassword"} | ForEach-Object{$_.value = $OctopusPassword}

$content.configuration.appSettings.add | Where-Object{$_.key -eq "OctopusBindingPort"} | ForEach-Object{$_.value = $OctopusBindingPort}


try
{
    $content.Save($AppConfigFile)
}

catch [System.Exception]
{
    Write-Host "Other exception"
}

Write-Output "Content of [$($AppConfigFile)] file after script:"
Get-Content $AppConfigFile