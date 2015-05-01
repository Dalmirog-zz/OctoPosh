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

   Creates an instance of an Project Resource object
.EXAMPLE
   $pg = Get-OctopusResourceModel -Resource ProjectGroup

   $pg.name = "NewProjectGroup"

   New-OctopusResource -Resource $pg

   Creates a new Project Group called "NewProjectGroup" on Octopus
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
        [parameter(ParameterSetName='GetResource')] 
        [string]$Resource,
        
        #Lists all the available resource types
        [parameter(ParameterSetName='ListResourceType')]         
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