<#
.Synopsis
   Short description
.DESCRIPTION
   Long description
.EXAMPLE
   Example of how to use this cmdlet
.EXAMPLE
   Another example of how to use this cmdlet
.LINK
   Github project: https://github.com/Dalmirog/Octoposh
#>
function Get-OctopusTask
{
    [CmdletBinding()]
    Param
    (
        #ID of task you want to get
        [Alias('ID')]
        [parameter(ParameterSetName = 'TaskId')]
        [ValidateNotNullOrEmpty()]
        [String]$TaskID,
        
        # Name of the task
        [ValidateSet('Backup','Delete','Health','Retention','Deploy','Upgrade','AdhocScript','TestEmail')]        
        [String]$Name = '*',

        #Document related to this task.
        [Alias('DocumentID')]        
        [string]$ResourceID = '*',

        #Document related to this task.
        [Alias('Status')]
        [ValidateSet('Success','TimedOut','Failed','Canceled')]        
        [string]$State = '*',

        #Before date
        [System.DateTimeOffset]$Before = [System.DateTimeOffset]::MaxValue,
        
        #After date
        [System.DateTimeOffset]$After = [System.DateTimeOffset]::MinValue,
        
        #When used, the cmdlet will only return the plain Octopus resource, withouth the extra info. This mode is used mostly from inside other cmdlets
        [switch]$ResourceOnly 
    )

    Begin
    {
        $c = New-OctopusConnection
        $list = @()
    }
    Process
    {
        If($TaskID){
            $tasks = $c.repository.Tasks.Get($TaskID)
        }

        elseif(($Name -ne '*') -or ($ResourceID -ne '*') -or ($State -ne '*') -or ($Before -ne [System.DateTimeOffset]::MaxValue) -or ($After -ne [System.DateTimeOffset]::MinValue)) {
            $tasks = $c.repository.Tasks.FindMany({param($t) if( ($t.name -like $Name)  -and ($t.state -like $State) -and (($t.Arguments.values -contains $ResourceID) -or ($t.Arguments -like $ResourceID)) -and ($t.StartTime -ge $After) -and ($t.LastupdatedTime -le $Before)
            ) {$true}})        
        }

        else{
            $tasks = $c.repository.Tasks.FindAll()
        }

        If($ResourceOnly){
            $list = $tasks
        }

        Else{
            Foreach ($task in $tasks){
                $list += $Task
                #$list += $obj
            }
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