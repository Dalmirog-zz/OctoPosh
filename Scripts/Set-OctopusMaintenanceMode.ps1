<#
.Synopsis
   Puts Octopus on/off maintenance mode
.DESCRIPTION
   Puts Octopus on/off maintenance mode
.EXAMPLE
   Set-OctopusMaintenanceMode -On
.EXAMPLE
   Set-OctopusMaintenanceMode -Off
.LINK
   Github project: https://github.com/Dalmirog/OctopusDeploy-Powershell-module
#>
function Set-OctopusMaintenanceMode
{
    [CmdletBinding()]
    Param
    (
        # Sets Octopus maintenance mode on
        [Parameter(Mandatory=$true,ParameterSetName='On')]
        [switch]$On,

        # Sets Octopus maintenance mode off
        [Parameter(Mandatory=$true,ParameterSetName='Off')]
        [switch]$Off
    )

    Begin
    {
        $c = New-OctopusConnection
    }
    Process
    {
        
        
        If ($on){$MaintenanceMode = "True"}

        else {$MaintenanceMode = "False"}
 
        $body = @{IsInMaintenanceMode=$MaintenanceMode} | ConvertTo-Json
 
        $r = Invoke-WebRequest -Uri "$Env:OctopusURL/api/maintenanceconfiguration" -Method PUT -Headers $c.header -Body $body
    }
    End
    {
        If ($r.statuscode -eq 200) {Return $true}

        else {Return $false}
    }
}