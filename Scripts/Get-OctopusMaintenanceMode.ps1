<#
.Synopsis
   Gets current Octopus maintenance mode status
.DESCRIPTION
   Gets current Octopus maintenance mode status
.EXAMPLE
   Get-OctopusMaintenanceMode
   
   Get the current status of the Octopus maintenance mode
.LINK
   Github project: https://github.com/Dalmirog/Octoposh
   Wiki: https://github.com/Dalmirog/OctoPosh/wiki
   QA and Cmdlet request: https://gitter.im/Dalmirog/OctoPosh#initial
#>
function Get-OctopusMaintenanceMode
{
    Begin
    {
        $c = New-OctopusConnection
    }
    Process
    {
        Write-Verbose "[$($MyInvocation.MyCommand)] Getting current Maintenance mode from $($Env:OctopusURL)/api/maintenanceconfiguration"                                 
        $r = Invoke-WebRequest -Uri "$Env:OctopusURL/api/maintenanceconfiguration" -Method Get -Headers $c.header -UseBasicParsing -Verbose:$false
        
        If ($r.statuscode -ne 200) {Return $false}

        else {
            
            Write-Verbose "[$($MyInvocation.MyCommand)] Getting info from last change to maintenance mode"            
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