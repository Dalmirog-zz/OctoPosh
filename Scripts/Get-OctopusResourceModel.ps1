<#
.Synopsis
   Returns an empty object from the Octopus Model that can later on be piped down to New-OctopusResource
.DESCRIPTION
   Returns an empty object from the Octopus Model that can later on be piped down to New-OctopusResource
.EXAMPLE
   $EnvironmentObj = Get-OctopusResourceModel -Resource "Environment"
.EXAMPLE
   $ProjectObj = Get-OctopusResourceModel -Resource "Project"
.LINK
   Github project: https://github.com/Dalmirog/OctopusDeploy-Powershell-module
#>
function Get-OctopusResourceModel
{
    [CmdletBinding()]
    Param
    (
        # Resource object model
        [ValidateSet("Environment","Project","ProjectGroup")] 
        [string]$Resource,
                
        [switch]$ListAvailable
    )

    Begin
    {
        $c = New-OctopusConnection
    }
    Process
    {
      If(!($ListAvailable)){
          Switch ($Resource){ 
                "Environment" {$o = New-Object Octopus.Client.Model.EnvironmentResource}
                "Project" {$o = New-Object Octopus.Client.Model.ProjectResource}            
                "ProjectGroup" {$o = New-Object Octopus.Client.Model.ProjectGroupResource}            
          }
      }
      Else{

        "Octopus.Client.Model.EnvironmentResource",
        "Octopus.Client.Model.ProjectResource",
        "Octopus.Client.Model.ProjectGroupResource"

        break      
      }     
    }
    End
    {
        return $o
    }
}