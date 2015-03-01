<#
.Synopsis
   Gets Octopus SMTP Configuration
.DESCRIPTION
   Gets Octopus SMTP Configuration
.EXAMPLE
   Get-OctopusSMTPConfig
.LINK
   Github project: https://github.com/Dalmirog/OctopusDeploy-Powershell-module
#>
function Get-OctopusSMTPConfig
{    
    Begin
    {
        $c = New-OctopusConnection
    }
    Process
    {
        $r = Invoke-WebRequest "$env:OctopusURL/api/smtpconfiguration" -Method Get -Headers $c.header -ContentType Json -UseBasicParsing

    }
    End
    {
        return ($r.content | ConvertFrom-Json)
    }
}