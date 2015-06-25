<##
.Synopsis
   Creates a new Octopus Resource out of a resource Object.

   This is an advanced cmdlet and all its examples involve multiple lines of code. Please check the advanced examples for a better reference: https://github.com/Dalmirog/OctoPosh/wiki/Advanced-Examples
.DESCRIPTION
   Creates a new Octopus Resource out of a resource Object.

   This is an advanced cmdlet and all its examples involve multiple lines of code. Please check the advanced examples for a better reference: https://github.com/Dalmirog/OctoPosh/wiki/Advanced-Examples
.EXAMPLE
   $pg = Get-OctopusResourceModel -Resource ProjectGroup ; $pg.name = "NewProjectGroup" ; New-OctopusResource -Resource $pg   

   Create a new Project Group called "NewProjectGroup" on Octopus
.LINK
   Github project: https://github.com/Dalmirog/Octoposh
   Wiki: https://github.com/Dalmirog/OctoPosh/wiki
   QA and Cmdlet request: https://gitter.im/Dalmirog/OctoPosh#initial
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
            {$_.getType() -eq [Octopus.Client.Model.ProjectGroupResource]} {$res = 'ProjectGroups'}
            {$_.getType() -eq [Octopus.Client.Model.ProjectResource]} {$res = 'Projects'}
            {$_.getType() -eq [Octopus.Client.Model.EnvironmentResource]} {$res = 'Environments'}
            {$_.getType() -eq [Octopus.Client.Model.FeedResource]} {$res = 'Feeds'}
            {$_.getType() -eq [Octopus.Client.Model.LibraryVariableSetResource]} {$res = 'LibraryVariableSets'}
            {$_.getType() -eq [Octopus.Client.Model.MachineResource]} {$res = 'Machines'}
            Default{Throw "Invalid object type: $($_.getType()) `nRun 'Get-OctopusResourceModel -ListAvailable' to get a list of the object types accepted by this cmdlet"}
        }
        Write-Verbose "[$($MyInvocation.MyCommand)] Creating an $($resource.GetType()) object"
        $newres = $c.repository.$res.Create($resource)

    }
    End
    {
        return $newres
    }
}


