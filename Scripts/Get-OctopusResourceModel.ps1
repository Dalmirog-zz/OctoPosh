<#
.Synopsis
   Returns an empty object from the Octopus Model that can later on be piped down to New-OctopusResource
.DESCRIPTION
   Returns an empty object from the Octopus Model that can later on be piped down to New-OctopusResource
.EXAMPLE
   $EnvironmentObj = Get-OctopusResourceModel -Resource "Environment"
.EXAMPLE
   $ProjectObj = Get-OctopusResourceModel -Resource "Project"
#>
function Get-OctopusResourceModel
{
    [CmdletBinding()]
    Param
    (
        # Resource object model
        [Parameter(Mandatory=$true,                  
                   Position=0)]
        [ValidateSet("Environment","Project","ProjectGroup")] 
        $Resource
    )

    Begin
    {
        $c = New-OctopusConnection
    }
    Process
    {
      switch ($Resource){ 
            "Environment" {$o = New-Object Octopus.Client.Model.EnvironmentResource}
            "Project" {$o = New-Object Octopus.Client.Model.ProjectResource}            
            "ProjectGroup" {$o = New-Object Octopus.Client.Model.ProjectGroupResource}            
        }

    }
    End
    {
        return $o
    }
}