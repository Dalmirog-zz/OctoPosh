<#
.Synopsis
   Gets Octopus SMTP Configuration
.DESCRIPTION
   Gets Octopus SMTP Configuration
.EXAMPLE
   Get-OctopusSMTPConfig

   Get Octopus SMTP Configuration
.LINK
   Github project: https://github.com/Dalmirog/OctoPosh
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