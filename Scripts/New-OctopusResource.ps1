<##
.Synopsis
   Short description
.DESCRIPTION
   Long description
.EXAMPLE
   Example of how to use this cmdlet
.EXAMPLE
   Another example of how to use this cmdlet
#>
function New-OctopusResource
{
    [CmdletBinding()]
    Param
    (
        # Param1 help description
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
        }


    }
    End
    {
    }
}


