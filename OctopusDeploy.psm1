$scripts = Get-ChildItem $PSScriptRoot\scripts -Filter "*.ps1"

foreach ($script in $scripts){

#Write-Output "Loading - $script"

. $script.FullName
}