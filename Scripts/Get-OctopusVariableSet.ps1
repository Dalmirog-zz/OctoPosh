<#
.Synopsis
   Gets Octopus Variable sets. These can belong to a specific Project or to a Library Variable set
.DESCRIPTION
   Gets Octopus Variable sets. These can belong to a specific Project or to a Library Variable set
.EXAMPLE
   Get-OctopusVariableSet

   Get the Variable Sets of all the Projects and all the Library Variable Sets
.EXAMPLE
   Get-OctopusVariableSet -ProjectName MyProject

   Get the Variable Set of the Project "MyProject"
.EXAMPLE
   Get-OctopusVariableSet -LibrarySetName "MyLibrarySet"

   Get the library variable set "MyLibrarySet" and the variable set of the project "MyProject"
.EXAMPLE
   $VariableName = "ConnectionString"
    
   Get-OctopusVariableSet | where {$_.variables.name -eq $VariableName}

   Get all the variable sets that include a variable with the name "ConnectionString"
.EXAMPLE
   $VariableValue = "MySiteName"
    
   Get-OctopusVariableSet | where {$_.variables.value -eq $VariableValue}

   Get all the variable sets that include a variable with the value "MySiteName"
.EXAMPLE
   Get-OctopusProjectGroup -name "MyImportantProjects"| Get-OctopusProject | Get-OctopusVariableSet

   Get the Variable Sets of all the Projects inside of a Project Group named "MyImportantProjects"
.LINK
   Github project: https://github.com/Dalmirog/Octoposh
   Wiki: https://github.com/Dalmirog/OctoPosh/wiki
   QA and Cmdlet request: https://gitter.im/Dalmirog/OctoPosh#initial
#>
function Get-OctopusVariableSet
{
    [CmdletBinding()]    
    Param
    (
        #Project name
        [Parameter(ValueFromPipelineByPropertyName = $true)]
        [string[]]$Projectname,
        
        #Library variable set name
        [Parameter(ValueFromPipelineByPropertyName = $true)]
        [string[]]$LibrarySetName,

        #When used the cmdlet will only return the plain Octopus resource object
        [switch]$ResourceOnly
    )

    Begin
    {
        $c = New-OctopusConnection
        $list = @()
        $variablesetids = @()
        $variablesets = @()        
        $i = 1        
    }
    Process
    {
        #Getting Project's variable set IDs
        If(!$Projectname -and !$LibrarySetName){
            Write-Verbose "[$($MyInvocation.MyCommand)] Getting all variable sets"
            $Projects = Get-OctopusProject -ResourceOnly
            $LibrarySets = $c.repository.LibraryVariableSets.FindAll()
            
            If($LibrarySets.count -gt 0){
                $variablesetids += $LibrarySets.links.variables
            }
            $variablesetids += $Projects.links.variables
        }
        else{
            If(![string]::IsNullOrEmpty($Projectname)){
                Write-Verbose "[$($MyInvocation.MyCommand)] Getting variable sets from projects: $projectname"
                $Projects = Get-OctopusProject -ProjectName $Projectname -ResourceOnly
                $variablesetids += $Projects.links.variables
            }
            If(![string]::IsNullOrEmpty($LibrarySetName)){
                Write-Verbose "[$($MyInvocation.MyCommand)] Getting library variable sets: $LibrarySetName"    
                $LibrarySets = $c.repository.LibraryVariableSets.FindMany({param($lib) if (($lib.name -in $LibrarySetName) -or ($lib.name -like $LibrarySetName)) {$true}})
                $variablesetids += $LibrarySets.links.variables                
            }
        }

        If($ResourceOnly -and $variablesetids){
            foreach ($id in $variablesetids){
                Write-Verbose "[$($MyInvocation.MyCommand)] [ResourceOnly] switch is on. Returning raw Octopus resource objects"
                $list += $c.repository.VariableSets.Get($id)
            }
        }

        Else{
            foreach ($id in $variablesetids){

            $vs = $c.repository.VariableSets.Get($id)

            Write-Progress -Activity "Getting info from variable set : $($vs.id)" -status "$i of $($variablesetids.count)" -percentComplete ($i / $variablesetids.count*100)
            Write-Verbose "[$($MyInvocation.MyCommand)] Getting info from variable set : $($vs.id)"                

            $vars = @()           

            foreach ($var in $vs.variables){

                $scope = Get-OctopusVariableScopeValue -Resource $vs -VariableName $var.name
                
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

            If($vs.ownerID -in $Projects.id){
                $Pname = $Projects | ?{$_.id -eq $vs.ownerid} | select -ExpandProperty name
                $LibraryVariableSetName = $null
            }
            else{
                $Pname = $null
                $LibraryVariableSetName = $LibrarySets | ?{$_.id -eq $vs.ownerid} | select -ExpandProperty name
            }
            
            $obj = [PSCustomObject]@{
                ProjectName = $Pname
                LibraryVariableSetName = $LibraryVariableSetName
                Variables = $vars
                LastModifiedOn = $vs.LastModifiedOn
                LastModifiedBy = $vs.LastModifiedBy
                Resource = $vs
                    
                } 
            
            $list += $obj
            
            $i++
        }#>        
        }

    }
    End
    {
        If($List.count -eq 0){
            $List = $null
        }
        return $List
    }
}