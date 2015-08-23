<#
.Synopsis
   This cmdlet creates instances of Octopus Resource Objects
.DESCRIPTION
   This cmdlet creates instances of Octopus Resource Objects
.EXAMPLE
   $EnvironmentObj = Get-OctopusResourceModel -Resource Environment

   Create an Environment Resource object 
.EXAMPLE
   $ProjectObj = Get-OctopusResourceModel -Resource Project

   Create Project resource object
.EXAMPLE
   $pg = Get-OctopusResourceModel -Resource ProjectGroup

   Create a Project Group resource object
.LINK
   Github project: https://github.com/Dalmirog/Octoposh
   Wiki: https://github.com/Dalmirog/OctoPosh/wiki
   QA and Cmdlet request: https://gitter.im/Dalmirog/OctoPosh#initial
#>
function Get-OctopusResourceModel
{
    [CmdletBinding()]
    Param
    (
        # Resource object model
        [ValidateSet('Environment','Project','ProjectGroup','NugetFeed','LibraryVariableSet','Machine','Lifecycle')]         
        [string]$Resource
    )

    Begin
    {
        $c = New-OctopusConnection
    }
    Process
    {
        Switch ($Resource){ 
            'Environment' {$o = New-Object Octopus.Client.Model.EnvironmentResource}
            'Project' {$o = New-Object Octopus.Client.Model.ProjectResource}
            'ProjectGroup' {$o = New-Object Octopus.Client.Model.ProjectGroupResource}
            'NugetFeed' {$o = New-Object Octopus.Client.Model.FeedResource}
            'LibraryVariableSet' {$o = New-Object Octopus.Client.Model.LibraryVariableSetResource}
            'Machine' {$o = New-Object Octopus.Client.Model.MachineResource}
            'Lifecycle' {$o = New-Object Octopus.Client.Model.LifecycleResource}
        }      
    }
    End
    {
        return $o
    }
}