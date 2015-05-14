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
#>
function Remove-OctopusResource
{
    [CmdletBinding()]
    Param
    (
        # Octopus resource object
        [Parameter(Mandatory=$true,
                   ValueFromPipelineByPropertyName=$true,
                   ParameterSetName = "Delete",                   
                   Position=0)]
        [object[]]$Resource,

        # Forces resource delete.
        [parameter(ParameterSetName = "ListAcceptedTypes")]
        [switch]$AcceptedTypes,

        # Forces resource delete.
        [switch]$Force

        

    )

    Begin
    {
        If($AcceptedTypes){
            "Octopus.Client.Model.ProjectGroupResource"
            "Octopus.Client.Model.ProjectResource"
            "Octopus.Client.Model.EnvironmentResource"
            "Octopus.Client.Model.DeploymentResource"
            "Octopus.Client.Model.MachineResource"
        }
        $c = New-OctopusConnection        
    }
    Process
    {
        Foreach ($r in $Resource){

            if(!($Force)){

                If (!(Get-UserConfirmation -message "Are you sure you want to delete this resource? `n`n [$($R.GetType().tostring())] $($R.name)`n")){
                    Throw "Canceled by user"
                }

            }

            switch ($R)
            {
                {$_.getType() -eq [Octopus.Client.Model.ProjectGroupResource]} {$ResourceType = "ProjectGroups"}
                {$_.getType() -eq [Octopus.Client.Model.ProjectResource]} {$ResourceType = "Projets"}
                {$_.getType() -eq [Octopus.Client.Model.EnvironmentResource]} {$ResourceType = "Environments"}
                {$_.getType() -eq [Octopus.Client.Model.DeploymentResource]} {$ResourceType = "Deployments"}
                {$_.getType() -eq [Octopus.Client.Model.MachineResource]} {$ResourceType = "Machines"}          
                Default{Throw "Invalid object type: $($_.getType()) `nRun 'Remove-OctopusResource -AcceptedTypes' to get a list of the object types accepted by this cmdlet"}
            }

            Write-Verbose "Deleting [$($R.GetType().tostring())] $($R.name)"

            $t = $c.repository.$ResourceType.Delete($Resource)

            $counter = 0

            #Silly loop to make sure the task gets done
            Do{
                $task = $c.repository.Tasks.Get($t.Links.Self)
                $counter++
                Start-Sleep -Seconds 2
              }
            Until (($task.state -notin ("Queued","executing")) -or ($counter -eq 5))
            
        }
    }
    End
    {
        return $task
    }
}