<#
.Synopsis
   Gets Octopus Tasks resources.

   This cmdlet can be used to track the status of tasks such as Health checks, Backups, Deployments, etc. See parameter "Name" to see all the kind of tasks available.
.DESCRIPTION
   Long description
.EXAMPLE
   Get-OctopusTask Name Backup

   Get all the Backup tasks of the server
.EXAMPLE
   Get-OctopusTask -name Health -status failed -After (Get-date).adddays(-7)

   Get all the Health check tasks that failed over the past 7 days
.EXAMPLE
   Get-OctopusTask -TaskID "ServerTAsks-1234"

   Get the server task with the id "ServerTasks-1234"
.EXAMPLE
   Get-OctopusTask -TaskID "ServerTAsks-1234" -GetTaskDetail

   Get a more detailed version of the Task "ServerTasks-1234". Among other things, this detailed version allows you to get the names and status of all the steps involved in the task.
.EXAMPLE
   Get-OctopusTask -Name Backup -After 01/01/2015

   Get all the Backup tasks 01/01/2015
.LINK
   Github project: https://github.com/Dalmirog/Octoposh
   Wiki: https://github.com/Dalmirog/OctoPosh/wiki
   QA and Cmdlet request: https://gitter.im/Dalmirog/OctoPosh#initial
#>
function Get-OctopusTask
{
    [CmdletBinding()]
    Param
    (
        # ID of task you want to get
        [Alias('ID')]
        [parameter(ParameterSetName = 'TaskId')]
        [ValidateNotNullOrEmpty()]
        [String[]]$TaskID,
        
        # Name of the task. Accepted values are: 'Backup','Delete','Health','Retention','Deploy','Upgrade','AdhocScript','TestEmail'
        [ValidateSet('Backup','Delete','Health','Retention','Deploy','Upgrade','AdhocScript','TestEmail')]        
        [String[]]$Name = '*',

        # Document related to this task.
        [Alias('DocumentID')]        
        [string]$ResourceID = '*',

        # Get a more detailed version of the Task. Among other things, this detailed version allows you to get the names and status of all the steps involved in the task.
        [switch]$GetTaskDetail,

        # Status of the task.
        [Alias('Status')]
        [ValidateSet('Success','TimedOut','Failed','Canceled','Executing','Canceling')]        
        [string[]]$State = '*',

        # Before date
        [System.DateTimeOffset]$Before = [System.DateTimeOffset]::MaxValue,
        
        # After date
        [System.DateTimeOffset]$After = [System.DateTimeOffset]::MinValue
    )

    Begin
    {
        $c = New-OctopusConnection
        $list = @()
    }
    Process
    {
        If($TaskID){
            Write-Verbose "[$($MyInvocation.MyCommand)] Getting tasks by ID"

            $tasks = @()

            foreach($t in $TaskID){
                Write-Verbose "[$($MyInvocation.MyCommand)] Getting task id: $t"
                $task = $c.repository.Tasks.Get($t)
                If($task -eq $null){
                    Write-Error "No tasks found with ID $t"
                }
                else {$tasks += $task}
            }           
            
        }

        elseif(($Name -ne '*') -or ($ResourceID -ne '*') -or ($State -ne '*') -or ($Before -ne [System.DateTimeOffset]::MaxValue) -or ($After -ne [System.DateTimeOffset]::MinValue)) {
            
            Write-Verbose "[$($MyInvocation.MyCommand)] Getting task by: `nName: $name`nResourceID: $resourceid`nState: $state`nBefore: $before`nAfter: $after"
            
            $tasks = $c.repository.Tasks.FindMany({
                param($t) 
                if( 
                    (($t.name -like $Name) -or ($t.name -in $name) ) -and 
                    (($t.state -like $State) -or ($t.state -in $State) )-and 
                    (($t.Arguments.values -contains $ResourceID) -or ($t.Arguments -like $ResourceID)) -and 
                    ($t.StartTime -ge $After) -and 
                    ($t.LastupdatedTime -le $Before)
                ) {$true}})        
            }

        else{
            Write-Verbose "[$($MyInvocation.MyCommand)] Getting all tasks"
            $tasks = $c.repository.Tasks.FindAll()
        }
                
        Write-Verbose "[$($MyInvocation.MyCommand)] Tasks found: $($Tasks.count)"
        If($GetTaskDetail){
            Write-Verbose "[$($MyInvocation.MyCommand)] GetTaskDetail switch was passed. Getting detailed info of each task. This might make the cmdlet take up to x2 times to finish processing."
            foreach($t in $tasks){
                $list += $c.repository.Tasks.GetDetails($t)
            }
        }
        Else{
            $list += $tasks       
        }
    }
    End
    {        
        If($list.count -eq 0){
            $list = $null
        }
        return $List
    }
}