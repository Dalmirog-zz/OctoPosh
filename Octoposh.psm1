$scripts = Get-ChildItem $PSScriptRoot\scripts -Filter "*.ps1"

Add-Type -Path "$PSScriptRoot\bin\Newtonsoft.Json.dll"
Add-Type -Path "$PSScriptRoot\bin\Octopus.Client.dll"
Add-Type -Path "$PSScriptRoot\bin\Octopus.Platform.dll"

foreach ($script in $scripts){
. $script.FullName
}
