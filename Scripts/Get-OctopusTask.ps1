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
        #ID of task
        [Alias("ID")]
        [parameter(ParameterSetName = "TaskId")]
        [String[]]$TaskID,
        
        # Name of the task
        [ValidateSet("Backup","Delete","Health","Retention","Deploy","Upgrade","AdhocScript","TestEmail")]        
        [String[]]$Name,

        #Document related to this task.
        [Alias("RelatedDocumentID")]        
        [string]$DocumentID,

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

        If($ResourceOnly){
            $list = $tasks
        }

        Else{
            Foreach ($task in $tasks){

                $list += $obj
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