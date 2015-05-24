<#
.Synopsis
   Starts a Backup task on the Octopus Instance
.DESCRIPTION
   Starts a Backup task on the Octopus Instance
.EXAMPLE
   Start-OctopusBackup   

   Starts a Backup task on the Octopus Instance
.EXAMPLE
   Start-OctopusBackup -force -wait -Message "My Custom Backup"  

   Starts a Backup task with a custom description, without getting prompted and waits until it finishes 
.LINK
   Github project: https://github.com/Dalmirog/Octoposh
   Advanced Cmdlet Usage: https://github.com/Dalmirog/OctoPosh/wiki/Advanced-Examples
   QA and Cmdlet request: https://gitter.im/Dalmirog/OctoPosh#initial
#>
function Start-OctopusBackup
{
    [CmdletBinding()]
    Param
    (
        #The message that will show up on the Octopus task
        [string]$Message,

        # Forces backup task start.
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