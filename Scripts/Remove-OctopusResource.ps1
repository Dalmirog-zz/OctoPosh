<#
.Synopsis
   Deletes Octopus Deploy resources such as Projects, Releases, Environments, etc
.DESCRIPTION
   Deletes Octopus Deploy resources such as Projects, Releases, Environments, etc
.EXAMPLE
   $ProjectResource = Get-OctopusProject -name "MyApp" ;
   Remove-OctopusResource -resource $ProjectResource

   Deletes the project called "MyApp" from the Octopus Database
.EXAMPLE
   Get-OctopusProjectGroup -name "MyProjects" | select -ExpandProperty Projects | Remove-OctopusResource

   Removes all the projects inside the Project Group "MyProjects"
.LINK
   Github project: https://github.com/Dalmirog/Octoposh
   Advanced Cmdlet Usage: https://github.com/Dalmirog/OctoPosh/wiki/Advanced-Examples
   QA and Cmdlet request: https://gitter.im/Dalmirog/OctoPosh#initial
#>
function Remove-OctopusResource
{
    [CmdletBinding()]
    Param
    (
        # Octopus resource object
        [Parameter(Mandatory=$true,
                   ValueFromPipelineByPropertyName=$true,
                   ParameterSetName = 'Delete',                   
                   Position=0)]
        [object[]]$Resource,

        # Forces resource delete.
        [parameter(ParameterSetName = 'ListAcceptedTypes')]
        [switch]$AcceptedTypes,

        # Forces resource delete.
        [switch]$Force,

        # Waits until the task is not on states "Queued" or "Executing"
        [switch]$Wait,

        # Timeout for -Wait parameter in minutes
        [double]$Timeout = 2
    )

    Begin
    {
        If($AcceptedTypes){
            'Octopus.Client.Model.ProjectGroupResource'
            'Octopus.Client.Model.ProjectResource'
            'Octopus.Client.Model.EnvironmentResource'
            'Octopus.Client.Model.DeploymentResource'
            'Octopus.Client.Model.MachineResource'
        }
        $c = New-OctopusConnection        
    }
    Process
    {
        Foreach ($r in $Resource){

            if(!($Force)){

                If (!(Get-UserConfirmation -message "Are you sure you want to delete this resource? `n`n [$($R.GetType().tostring())] $($R.name)`n")){
                    Throw 'Canceled by user'
                }

            }

            switch ($R)
            {
                {$_.getType() -eq [Octopus.Client.Model.ProjectGroupResource]} {$ResourceType = 'ProjectGroups'}
                {$_.getType() -eq [Octopus.Client.Model.ProjectResource]} {$ResourceType = 'Projects'}
                {$_.getType() -eq [Octopus.Client.Model.EnvironmentResource]} {$ResourceType = 'Environments'}
                {$_.getType() -eq [Octopus.Client.Model.DeploymentResource]} {$ResourceType = 'Deployments'}
                {$_.getType() -eq [Octopus.Client.Model.MachineResource]} {$ResourceType = 'Machines'}          
                Default{Throw "Invalid object type: $($_.getType()) `nRun 'Remove-OctopusResource -AcceptedTypes' to get a list of the object types accepted by this cmdlet"}
            }

            Write-Verbose "Deleting [$($R.GetType().tostring())] $($r.name)"

            $task = $c.repository.$ResourceType.Delete($r)

            If($wait){

                $StartTime = Get-Date

                Do{
                    $CurrentTime = Get-date
                    
                    $task = Get-OctopusTask -ID $task.id -ResourceOnly
                    
                    Start-Sleep -Seconds 2
                  }

                Until (($task.state -notin ('Queued','executing')) -or ($CurrentTime -gt $StartTime.AddMinutes($Timeout)))
            }


            
        }
    }
    End
    {
        return $task
    }
}