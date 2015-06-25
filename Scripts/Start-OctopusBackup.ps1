<#
.Synopsis
   Starts a Backup task on the Octopus Instance.

   This cmdlet will only work on Octopus 2.6 and below. On Octopus 3.0 the database technology was switched to SQL server, and the users will be in charge of engineering their own backup strategies
.DESCRIPTION
   Starts a Backup task on the Octopus Instance

   This cmdlet will only work on Octopus 2.6 and below. On Octopus 3.0 the database technology was switched to SQL server, and the users will be in charge of engineering their own backup strategies
.EXAMPLE
   Start-OctopusBackup   

   Starts a Backup task on the Octopus Instance
.EXAMPLE
   Start-OctopusBackup -force -wait -Message "My Custom Backup"  

   Starts a Backup task with a custom description, without getting prompted and waits until it finishes 
.LINK
   Github project: https://github.com/Dalmirog/Octoposh
   Wiki: https://github.com/Dalmirog/OctoPosh/wiki
   QA and Cmdlet request: https://gitter.im/Dalmirog/OctoPosh#initial
#>
function Start-OctopusBackup
{
    [CmdletBinding()]
    Param
    (
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
        $list = @()
    }
    Process
    {
        
        If(!($Force)){
            If (!(Get-UserConfirmation -message "Are you sure you want to start a backup task for $env:OctopusURL")){
                Throw 'Canceled by user'
            }
        }        
        
        Write-Verbose "[$($MyInvocation.MyCommand)] Starting Backup task for $env:OctopusURL"        

        $Task = $c.repository.Tasks.ExecuteBackup($Message)

        Write-Verbose "[$($MyInvocation.MyCommand)] Task $($Task.id) started"

        If($wait){

            Write-Verbose "[$($MyInvocation.MyCommand)] WAIT switch is ON. The command will wait until the task finishes"

            $StartTime = Get-Date

            Do{
                $CurrentTime = Get-date
                    
                $task = Get-OctopusTask -ID $task.id

                Write-Verbose "[$($MyInvocation.MyCommand)] Task $($Task.id) status: $($task.state)"
                    
                Start-Sleep -Seconds 2

            }Until (($task.state -notin ('Queued','executing')) -or ($CurrentTime -gt $StartTime.AddMinutes($Timeout)))

            Write-Verbose "[$($MyInvocation.MyCommand)] Backup task finished with status: $($task.state.tostring().toupper())"
        }

        $list += $Task       
    }   
    End
    {
        If($list.count -eq 0){
            $list = $null
        }
        return $List
    }
}