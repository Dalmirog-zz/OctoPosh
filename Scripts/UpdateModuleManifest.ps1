[CmdletBinding()]
Param(
    [ValidateNotNullOrEmpty()]
    [string]$ManifestPath,

    [ValidateNotNullOrEmpty()]
    [Version]$Version
)

If(!(Test-Path $ManifestPath)){
    throw "Module manifest not found at: $ManifestPath"
}

Write-Output "Attempting to update [$ManifestPath] with version [$Version]"

(Get-Content $ManifestPath) | %{$_ -replace '^ModuleVersion.+', ("ModuleVersion = " + "'$version'")} | Set-Content  ($ManifestPath) -Verbose