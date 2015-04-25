function Put-OctopusResource([string]$uri, [object]$resource) {
    Write-Host "[PUT]: $uri"
    Invoke-RestMethod -Method Put -Uri "$env:OctopusURL/$uri" -Body $($resource | ConvertTo-Json -Depth 10) -Headers $c.header
}