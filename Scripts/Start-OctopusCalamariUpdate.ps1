<#
.Synopsis
   Starts a "Calamari Update" taks on a machine or set of machines inside of an environment
.DESCRIPTION
   Starts a "Calamari Update" taks on a machine or set of machines inside of an environment
.EXAMPLE
   Start-OctopusCalamariUpdate -Environment "Staging"

   Starts a "Calamari Update" task for all the machines inside of the environment "Staging"
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
    [CmdletBinding(DefaultParameterSetName='Machine')]    
    Param
    (
        # The name of the environment/s on which you wanna start a health check. A task will start for each Environment listed on this parameter
        [Parameter(Mandatory=$true,                   
                   ParameterSetName='Environment')]
        [string[]]$EnvironmentName,

        [Parameter(Mandatory=$true,                   
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

                $m = Get-OctopusMachine -EnvironmentName $Environment -ResourceOnly -ErrorAction Stop
                
                Write-Verbose "[$($MyInvocation.MyCommand)] Found $($m.count) machines in environment $environment"                

                $Machines += $m
            }
        }

        Else{
            $machines = Get-OctopusMachine -MachineName $MachineName -ResourceOnly -ErrorAction Stop
        }

        If(!$message -and $PSCmdlet.ParameterSetName -eq 'Machine'){
            $message = "[API Generated] Starting 'Calamari Update' task on Machines: $($machines.name)"
        }

        elseIf(!$message -and $PSCmdlet.ParameterSetName -eq 'Environment'){
            $message = "[API Generated] Starting 'Calamari Update' task on Environments: $EnvironmentName"
        }

        If(!($Force)){
            If (!(Get-UserConfirmation -message "Are you sure you want to start a health check on the environment: $Environment")){
                Throw 'Canceled by user'
            }
        }
        If($machines.count -ne 0){
            $task = $c.repository.Tasks.ExecuteCalamariUpdate($Message,$machines.id)
        }
        If($wait){

            $StartTime = Get-Date

            Do{
                $CurrentTime = Get-date
                    
                $task = Get-OctopusTask -ID $task.id
                    
                Start-Sleep -Seconds 2

                Write-Verbose "[Get-OctopusTask] Task state is: $($task.state)"
            }Until (($task.state -notin ('Queued','executing')) -or ($CurrentTime -gt $StartTime.AddMinutes($Timeout)))

            Write-Verbose "[Get-OctopusTask] Task finished or cmdlet timed out. Last task status: $($task.state.tostring().toupper())"
            return $task
        }

        else{
            return $task
        }


        
    }   
    End
    {
    }
}