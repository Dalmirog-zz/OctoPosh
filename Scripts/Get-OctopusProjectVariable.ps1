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
   $VariableName = "ConnectionString"
    
   Get-OctopusProjectVariable | where {$_.variables.name -eq $VariableName}

   This command gets all the variable sets that include a variable with the name "ConnectionString"
.EXAMPLE
   $VariableValue = "MySiteName"
    
    Get-OctopusProjectVariable | where {$_.variables.value -eq $VariableValue}

   This command gets all the variable sets that include a variable with the value "MySiteName"
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
        $i = 1        
    }
    Process
    {
        #Getting Projects        
        $Projects = Get-OctopusProject -Name $Projectname -ResourceOnly       

        #Getting info by project
        foreach ($p in $Projects){

            Write-Progress -Activity "Getting info from variable set of project: $($p.name)" -status "$i of $($Projects.count)" -percentComplete ($i / $Projects.count*100)                

            $vars = @()

            $projVar = $C.repository.VariableSets.Get($p.links.variables)

            foreach ($var in $projVar.variables){

                $scope = Get-OctopusVariableScopeValue -Resource $projVar -VariableName $var.name
                
                $obj = [PSCustomObject]@{
                    Name = $var.name
                    Value = $var.value
                    Scope = $scope
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
            
            $i++
        }

    }
    End
    {
        If($list.count -eq 0){
            $list = $null
        }
        return $List
    }
}