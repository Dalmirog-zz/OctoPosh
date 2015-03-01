<#
.Synopsis
   Gets current Octopus maintenance mode status
.DESCRIPTION
   Gets current Octopus maintenance mode status
.EXAMPLE
   Get-OctopusMaintenanceMode   
.LINK
   Github project: https://github.com/Dalmirog/OctopusDeploy-Powershell-module
#>
function Get-OctopusMaintenanceMode
{
    Begin
    {
        $c = New-OctopusConnection
    }
    Process
    {                         
        $r = Invoke-WebRequest -Uri "$Env:OctopusURL/api/maintenanceconfiguration" -Method Get -Headers $c.header -UseBasicParsing

        If ($r.statuscode -ne 200) {Return $false}

        else {
        
            $ev = $c.repository.Events.FindOne({param($event) if (($event.category -eq "Modified") -and ($event.Message -eq "Maintenance Configuration was changed.")) {$true}})

            If($ev.IdentityEstablishedWith -eq "Session cookie"){$authmethod = "Web"}
            elseif ($ev.IdentityEstablishedWith -like "API key*") {$authmethod = "APIKey"}

            $o = [PsCustomObject]@{

                IsInMaintenanceMode = ($r | ConvertFrom-Json).Isinmaintenancemode
                LastModifiedBy = $ev.UserName
                LastModifiedDate = ($ev.Occurred).datetime
                LastModifiedAuth = $authmethod
                }
        }
    }
    End
    {
        return $o
    }
}