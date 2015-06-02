<#
.Synopsis
   Returns an empty object from the Octopus Model that can later on be piped down to New-OctopusResource
.DESCRIPTION
   Returns an empty object from the Octopus Model that can later on be piped down to New-OctopusResource
.EXAMPLE
   $EnvironmentObj = Get-OctopusResourceModel -Resource "Environment"

   Creates an instance of an Environment Resource object 
.EXAMPLE
   $ProjectObj = Get-OctopusResourceModel -Resource "Project"

   Create Project Resource object instance
.EXAMPLE
   $pg = Get-OctopusResourceModel -Resource ProjectGroup

   Create Project Group resource object instance
.LINK
   Github project: https://github.com/Dalmirog/Octoposh
   Advanced Cmdlet Usage: https://github.com/Dalmirog/OctoPosh/wiki/Advanced-Examples
   QA and Cmdlet request: https://gitter.im/Dalmirog/OctoPosh#initial
#>
function Get-OctopusResourceModel
{
    [CmdletBinding(DefaultParameterSetName="GetResource")]
    Param
    (
        # Resource object model
        [ValidateSet('Environment','Project','ProjectGroup','NugetFeed','LibraryVariableSet')]
        [parameter(ParameterSetName='GetResource')] 
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
        }      
    }
    End
    {
        return $o
    }
}