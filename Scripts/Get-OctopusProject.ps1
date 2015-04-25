<#
.Synopsis
   Gets information about Octopus Projects
.DESCRIPTION
   Gets information about Octopus Projects
.EXAMPLE
   Example of how to use this cmdlet
.EXAMPLE
   Another example of how to use this cmdlet
.LINK
   Github project: https://github.com/Dalmirog/OctopusDeploy-Powershell-module
#>
function Get-OctopusProject
{
    [CmdletBinding()]
    [Alias()]
    Param
    (
        # Environment name
        [Parameter(Position=0)]
        [string[]]$Name = $null,

        # Environment ID
        [String[]]$ID = $null
    )

    Begin
    {
        $c = New-OctopusConnection

        If(($Name -ne $null) -or ($ID -ne $null)){
            
            $Projects = $c.repository.Projects.FindMany({param($Proj) if (($Proj.name -in $name) -or ($Proj.name -like $name) -or ($Proj.id -in $Id)) {$true}})
        }

        else{
        
            $Projects = $c.repository.projects.FindAll()
        }        

        $dashboard = Get-OctopusResource "/api/dashboard/dynamic" -header $c.header
        
    }
    Process
    {

        $list = @()

        foreach ($p in $Projects){

            
            $pg = $c.repository.ProjectGroups.Get($p.projectgroupid)
            $l = $c.repository.Lifecycles.Get($p.LifeCycleId)
            
            $obj = [PSCustomObject]@{
                ProjectName = $p.name
                ProjectGroupName = $pg.name
                LifecycleName = $l.name
                AutoCreateRelease = $p.AutoCreateRelease
                Resource = $p                
            }
            
            $list += $obj

        }       


    }
    End
    {
        $list
    }
}

#Get-OctopusProject