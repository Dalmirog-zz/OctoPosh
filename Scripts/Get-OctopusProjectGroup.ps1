<#
.Synopsis
   Short description
.DESCRIPTION
   Gets information about Octopus Project Groups
.EXAMPLE
   Gets information about Octopus Project Groups
.EXAMPLE
   Get-OctopusProjectGroup 

   Gets all the Project Groups on the Octopus instance
.EXAMPLE
   Get-OctopusProjectGroup -name "MyProjects"

   Gets the Project Group named "MyProjects"
.EXAMPLE
   Get-OctopusProjectGroup -name "*web*"

   Gets all the Project Groups with the word "web" on their name
.EXAMPLE
   Get-OctopusProject "MyProject" | Get-OctopusProjectGroup | select -ExpandProperty projects | Get-OctopusProjects

   Gets all the projects that are on the same group as the project "MyProject"
.LINK
   Github project: https://github.com/Dalmirog/Octoposh
#>
function Get-OctopusProjectGroup
{
    [CmdletBinding()]    
    Param
    (
        # Name of the Project Group
        [alias("ProjectGroupName")]
        [Parameter(ValueFromPipelineByPropertyName=$true,Position=0)]
        [string[]]$Name
    )

    Begin
    {
        $c = New-OctopusConnection
        $list = @()
    }
    Process
    {
        #Getting Projects        
        If(!([string]::IsNullOrEmpty($Name))){
                    
            $ProjectGroups = $c.repository.ProjectGroups.FindMany({param($Pg) if (($Pg.name -in $name) -or ($Pg.name -like $name)) {$true}})
        }

        else{
        
            $ProjectGroups = $c.repository.ProjectGroups.FindAll()
        }
        
        foreach($ProjectGroup in $ProjectGroups){

            $Plist = @()
            
            $Projects = Get-OctopusResource -uri $ProjectGroup.links.projects -header $c.header
            
            foreach($project in $Projects.items){

                $p = [PSCustomObject]@{
                        ProjectName = $project.name}

                $Plist += $p
            }

            $pg = [PSCustomObject]@{
                        ProjectGroupName = $ProjectGroup.name
                        Id = $ProjectGroup.id
                        Projects = $plist
                        LastModifiedOn = $ProjectGroup.LastModifiedOn
                        LastModifiedBy = $ProjectGroup.LastModifiedBy
                        resource = $ProjectGroup}

            $list += $pg
        }        
    }
    End
    {
        return $list
    }
}