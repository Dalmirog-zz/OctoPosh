<#
.Synopsis
   Puts Octopus on/off maintenance mode
.DESCRIPTION
   Puts Octopus on/off maintenance mode
.EXAMPLE
   Set-OctopusMaintenanceMode -On

   Set Maintenance mode ON
.EXAMPLE
   Set-OctopusMaintenanceMode -Off

   Set Maintenance mode OFF
.LINK
   Github project: https://github.com/Dalmirog/Octoposh
   Wiki: https://github.com/Dalmirog/OctoPosh/wiki
   QA and Cmdlet request: https://gitter.im/Dalmirog/OctoPosh#initial
#>
function Set-OctopusMaintenanceMode
{
    [CmdletBinding()]
    Param
    (
        # Octopus maintenance mode switch. Accepts values "ON" or "OFF"
        [ValidateSet("ON","OFF")]
        [Parameter(Mandatory=$true)]
        [String]$Mode,

        # Forces cmdlet to continue without prompting
        [switch]$Force
    )

    Begin
    {
        $c = New-OctopusConnection
    }
    Process
    {        
        If ($Mode -eq "ON"){$MaintenanceMode = "True"}

        else {$MaintenanceMode = "False"}

        If(!($Force)){
            If (!(Get-UserConfirmation -message "Are you sure you want to set maintenance mode for $Env:OctopusURL to the status: $mode ?")){
                Throw "Canceled by user"
            }
        }
 
        $body = @{IsInMaintenanceMode=$MaintenanceMode} | ConvertTo-Json
 
        Try{
            Write-Verbose "[$($MyInvocation.MyCommand)] Turning maintenance mode $($Mode)"   
            $r = Invoke-WebRequest -Uri "$Env:OctopusURL/api/maintenanceconfiguration" -Method PUT -Headers $c.header -Body $body -UseBasicParsing -Verbose:$false
        }
        Catch{
            write-error $_
        }        
    }
    End
    {
        Write-Verbose "[$($MyInvocation.MyCommand)] HTTP request to set Maintenance mode $Mode returned code $($r.statuscode)"
        if($r.statuscode -eq 200){
            Return $True
        }
        Else{
            Return $false
        }
    }
}