<#
.Synopsis
   Starts a "Calamari Update" taks on a machine or set of machines inside of an environment
.DESCRIPTION
   Starts a "Calamari Update" taks on a machine or set of machines inside of an environment
.EXAMPLE
   Start-OctopusCalamariUpdate -Environment "Staging"

   Starts a "Calamari Update" task on the environment "Staging"
.EXAMPLE
   Start-OctopusCalamariUpdate -MachineName "NY-DB1"

   Starts a "Calamari Update" task on the machine "NY-DB1"
.LINK
   Github project: https://github.com/Dalmirog/Octoposh
   Wiki: https://github.com/Dalmirog/OctoPosh/wiki
   QA and Cmdlet request: https://gitter.im/Dalmirog/OctoPosh#initial
#>
function Start-OctopusCalamariUpdate
{
    [CmdletBinding(DefaultParameterSetName='Environment')]    
    Param
    (
        # The name of the environment/s on which you wanna start a health check. A task will start for each Environment listed on this parameter
        [Parameter(Mandatory=$true,
                   ValueFromPipelineByPropertyName=$true,
                   ParameterSetName='Environment')]
        [string[]]$EnvironmentName,

        [Parameter(Mandatory=$true,
                   ValueFromPipelineByPropertyName=$true,
                   ParameterSetName='Machine')]
        [string[]]$MachineName,

        # The message that will show up on the Octopus task. If a value is not passed to this parameter, a default message will be used
        [string]$Message,

        # Forces cmdlet to continue without prompting
        [switch]$Force,

        # Waits until the task is not on states "Queued" or "Executing"
        [switch]$Wait,

        # Timeout for [Wait] parameter in minutes. Default timeout is 2 minutes
        [double]$Timeout = 2
    )

    Begin
    {
        $c = New-OctopusConnection
        $machines = @()
        $list = @()
    }
    Process
    {
        If($PSCmdlet.ParameterSetName -eq 'Environment'){
            foreach ($environment in $EnvironmentName){
            
                Write-Verbose "[$($MyInvocation.MyCommand)] Getting machines from Environment: $environment"

                $m = Get-OctopusMachine -EnvironmentName $Environment -ResourceOnly
                
                Write-Verbose "[$($MyInvocation.MyCommand)] Found $($m.count) machines in environment $environment"                

                $Machines += $m
            }
        }

        Else{
            $machines = Get-OctopusMachine -MachineName $MachineName -ResourceOnly
        }

        "Checking health in "
        $machines.name
       


        
    }   
    End
    {
    }
}