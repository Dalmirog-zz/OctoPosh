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
                   Position=0)]
        [object[]]$Resource,

        # Forces resource delete.
        [switch]$Force

    )

    Begin
    {
        $c = New-OctopusConnection        
    }
    Process
    {
        Foreach ($r in $Resource){

            if(!($Force)){

                If (!(Get-UserConfirmation -message "Are you sure you want to delete this resource? `n`n$($Resource.name)`n")){
                    Throw "Canceled by user"
                }

            }

            switch ($Resource)
            {
                {$_.getType() -eq [Octopus.Client.Model.ProjectGroupResource]} {$r = $c.repository.ProjectGroups.Delete($_)}
                {$_.getType() -eq [Octopus.Client.Model.ProjectResource]} {$r = $c.repository.Projects.Delete($_)}
                {$_.getType() -eq [Octopus.Client.Model.EnvironmentResource]} {$r = $c.repository.Environments.Delete($_)}
                {$_.getType() -eq [Octopus.Client.Model.DeploymentResource]} {$r = $c.repository.Deployments.Delete($_)}          
                Default{Throw "Invalid object type: $($_.getType()) `nRun 'Get-OctopusResourceModel -ListAvailable' to get a list of the object types accepted by this cmdlet"}
            }

            $counter = 0

            #Silly loop to make sure the task gets done
            Do{
                $task = $c.repository.Tasks.Get($r.Links.Self)
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