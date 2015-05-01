<##
.Synopsis
   Creates a new Octopus Resource out of a resource Object
.DESCRIPTION
   Creates a new Octopus Resource out of a resource Object
.EXAMPLE
   $pg = Get-OctopusResourceModel -Resource ProjectGroup

   $pg.name = "NewProjectGroup"

   New-OctopusResource -Resource $pg

   Creates a new Project Group called "NewProjectGroup" on Octopus
.LINK
   Github project: https://github.com/Dalmirog/OctopusDeploy-Powershell-module
#>
function New-OctopusResource
{
    [CmdletBinding()]
    Param
    (
        # Resource Object
        [Parameter(Mandatory=$true,
                   ValueFromPipeline=$true,
                   Position=0)]
        $Resource
    )

    Begin
    {
        $c = New-OctopusConnection
    }
    Process
    {
        switch ($Resource)
        {
            {$_.getType() -eq [Octopus.Client.Model.ProjectGroupResource]} {$c.repository.ProjectGroups.Create($_)}
            {$_.getType() -eq [Octopus.Client.Model.ProjectResource]} {$c.repository.Projects.Create($_)}
            {$_.getType() -eq [Octopus.Client.Model.EnvironmentResource]} {$c.repository.Environments.Create($_)}
            Default{Throw "Invalid object type: $($_.getType()) `nRun 'Get-OctopusResourceModel -ListAvailable' to get a list of the object types accepted by this cmdlet"}
        }


    }
    End
    {
    }
}


