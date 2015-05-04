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
   Github project: https://github.com/Dalmirog/Octoposh
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
        [switch]$Off,

        #Such ugly params, I should have just 1 with 2 options
        # Forces action
        [switch]$Force
    )

    Begin
    {
        $c = New-OctopusConnection
    }
    Process
    {        
        If ($on){$MaintenanceMode = "True"}

        else {$MaintenanceMode = "False"}

        If(!($Force)){
            If (!(Get-UserConfirmation -message "Are you sure you want to set maintenance mode for $Env:OctopusURL to: $MaintenanceMode ?")){
                Throw "Canceled by user"
            }
        }
 
        $body = @{IsInMaintenanceMode=$MaintenanceMode} | ConvertTo-Json
 
        $r = Invoke-WebRequest -Uri "$Env:OctopusURL/api/maintenanceconfiguration" -Method PUT -Headers $c.header -Body $body -UseBasicParsing
    }

    End
    {
        If ($r.statuscode -eq 200) {Return $true}

        else {Return $false}
    }
}