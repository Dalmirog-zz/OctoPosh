function Get-OctopusResource([string]$uri, [object]$header) {
    
    return Invoke-RestMethod -Method Get -Uri "$env:OctopusURL/$uri" -Headers $header
}