$scripts = Get-ChildItem $PSScriptRoot\scripts -Filter "*.ps1"

foreach ($script in $scripts){

#Write-Output "Loading - $script"

. $script.FullName
}

#Creating Environment variables
$env:OctopusURI = "http://localhost"
$env:OctopusAPIKey = "API-7CH6XN0HHOU7DDEEUGKUFUR1K"
