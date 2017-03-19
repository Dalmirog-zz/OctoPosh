param(
    [string]$ConfigFile
)

$config = Get-Content $ConfigFile -Verbose | ConvertFrom-Json

$OctopusBindingPort= $config.OctopuswebListenPort
$OctopusAPIKey = $config.OctopusAPIKey.ToString()

$AppConfigFile = resolve-path "$PSScriptRoot\..\App.config" #Relative to the location of the .cake script, NOT to the location of this ps1 script.

[xml]$content = Get-Content $AppConfigFile

$content.configuration.appSettings.add | ?{$_.key -eq "OctopusAPIKey"} | %{$_.value = $OctopusAPIKey}
$content.configuration.appSettings.add | ?{$_.key -eq "OctopusBindingPort"} | %{$_.value = $OctopusBindingPort}

$content.Save($AppConfigFile)

Write-Output "Content of the app.config file after script:"
Get-Content $AppConfigFile
