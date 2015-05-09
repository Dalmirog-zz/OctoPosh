<#
.Synopsis
   Gets Octopus Projects Variable sets
.DESCRIPTION
   Gets Octopus Projects Variable sets
.EXAMPLE
   Get-OctopusProjectVariable

   This command gets the variable sets of all the projects
.EXAMPLE
   Get-OctopusProjectVariable -ProjectName MyProject

   This command gets the Variable Set of the Project named "MyProject"
.EXAMPLE
   Get-OctopusProject -name MyProject | Get-OctopusProjectVariable

   This command gets the Variable Set of the Project named "MyProject"
.EXAMPLE
   Get-OctopusProjectGroup -name "MyImportantProjects"| Get-OctopusProject | Get-OctopusProjectVariable

   This command gets the Variable Sets of all the projects inside of a Project Group named "MyImportantProjects"
.LINK
   Github project: https://github.com/Dalmirog/Octoposh
#>
function Get-OctopusProjectVariable
{
    [CmdletBinding()]    
    Param
    (
        #Project name
        [Parameter(ValueFromPipelineByPropertyName = $true,Position=0)]
        [string[]]$Projectname
    )

    Begin
    {
        $c = New-OctopusConnection
        $list = @()        
    }
    Process
    {
        #Getting Projects        
        $Projects = Get-OctopusProject -Name $Projectname -ResourceOnly       

        #Getting info by project
        foreach ($p in $Projects){

            $vars = @()

            $projVar = $C.repository.VariableSets.Get($p.links.variables)

            foreach ($var in $projVar.variables){
                
                $obj = [PSCustomObject]@{
                    Name = $var.name
                    Value = $var.value
                    Scope = $var.scope
                    IsSensitive = $var.IsSensitive
                    IsEditable = $var.IsEditable
                    Prompt = $var.Prompt                    
                }

                $vars += $obj
            }
            
            $obj = [PSCustomObject]@{
                ProjectName = $p.name
                Variables = $vars
                LastModifiedOn = $projVar.LastModifiedOn
                LastModifiedBy = $projVar.LastModifiedBy
                Resource = $projVar
                    
                } 
            
            $list += $obj
        }

    }
    End
    {
        return $list
    }
}