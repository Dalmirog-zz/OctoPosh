<#
.Synopsis
   Executes the Octopus server-side retention policy.
.DESCRIPTION
   Executes the Octopus server-side retention policy.
.EXAMPLE
   Start-OctopusRetentionPolicy

   Executes the Octopus server-side retention policy and returns a TaskResource object
.EXAMPLE
   Start-OctopusRetentionPolicy -force -wait -Timeout 1

   Executes the Octopus server-side retention policy and waits until the task is not on states "Queued" or "Executing"
.LINK
   Github project: https://github.com/Dalmirog/Octoposh
   Advanced Cmdlet Usage: https://github.com/Dalmirog/OctoPosh/wiki/Advanced-Examples
   QA and Cmdlet request: https://gitter.im/Dalmirog/OctoPosh#initial
#>
function Start-OctopusRetentionPolicy
{
    [CmdletBinding()]
    Param
    (
        # Forces health check task start.
        [switch]$Force,

        # Waits until the task is not on states "Queued" or "Executing"
        [switch]$Wait,

        # Timeout for -Wait parameter in minutes. Default timeout is 2 minutes
        [double]$Timeout = 2
    )

    Begin
    {
        $c = New-OctopusConnection
        $list = @()
    }
    Process
    {
        If(!($Force)){
            If (!(Get-UserConfirmation -message "Are you sure you want to start executing the Retention Policy on your Octopus server ($env:OctopusURL)?")){
                Throw 'Canceled by user'
            }
        }

        Write-Verbose "[$($MyInvocation.MyCommand)] Starting retention Policy task on $env:OctopusURL"
        $task = $c.repository.RetentionPolicies.ApplyNow()

        If($wait){

            $StartTime = Get-Date

            Do{
                $CurrentTime = Get-date
                    
                $task = Get-OctopusTask -ID $task.id
                    
                Start-Sleep -Seconds 2

                Write-Verbose "[$($MyInvocation.MyCommand)] Task $($Task.id) status: $($task.state)"

            }Until (($task.state -notin ('Queued','executing')) -or ($CurrentTime -gt $StartTime.AddMinutes($Timeout)))

            Write-Verbose "[$($MyInvocation.MyCommand)] Task finished with status: $($task.state.tostring().toupper())"
        }
    }
    End
    {
        If(!$Task){
            $Task = $null
        }

        return $Task
    }
}