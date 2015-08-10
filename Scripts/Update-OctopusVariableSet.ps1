<#
.Synopsis
   This cmdlet adds/Updates variables on a variable set (Project's or Library's)  from a JSON file. It returns the updated Variable Set for it to be saved on the database using Update-OctopusResource.

   This is an advanced cmdlet. For info about its usage go to: https://github.com/Dalmirog/OctoPosh/wiki/Modifying-Variable-Sets   
.DESCRIPTION
   This cmdlet adds/Updates Octopus variables from a JSON file to a Variable Set. Then it returns the updated Variable Set for it to be saved on the database using Update-OctopusResource.

   This is an advanced cmdlet. For info about its usage go to: https://github.com/Dalmirog/OctoPosh/wiki/Modifying-Variable-Sets
.EXAMPLE
   Update-OctopusVariableSet -ProjectName "MyProject" -file "C:\Files\Variables.json" -mode Overwrite

   Overwrites the variable set of the project "MyProject" with the contents of C:\Files\Variables.json
.EXAMPLE
   Update-OctopusVariableSet -Library "MyLibrarySet" -file "C:\Files\Variables.json" -mode Merge

   Merges the variables inside of "C:\Files\Variables.json" with the variable set "MyLibrarySet"
.LINK
   Github project: https://github.com/Dalmirog/Octoposh
   Advanced Cmdlet Usage: https://github.com/Dalmirog/OctoPosh/wiki/Advanced-Examples
   QA and Cmdlet request: https://gitter.im/Dalmirog/OctoPosh#initial
#>
function Update-OctopusVariableSet
{
    [OutputType([Octopus.Client.Model.VariableSetResource])]
    Param
    (
        # Path of the JSON file that contains the variables info.
        [Parameter(Mandatory=$true)]        
        [string]$File,

        # Library variable Set
        [Parameter(Mandatory=$true,ParameterSetName = 'Library')]        
        [string]$LibraryVariableSet,

        # Project name        
        [Parameter(Mandatory=$true,ParameterSetName = 'Project')]        
        [string]$ProjectName,

        # This parameter sets the way the variables will be added to the Variable Set. Accepted values are "Merge" and "Overwrite"
        [Parameter(Mandatory=$true)]        
        [ValidateSet('Overwrite','Merge')]
        [string]$Mode

    )

    Begin
    {
        function Import-JsonFile ([string]$file){
            If(!(Test-path $file)){
                Throw "File not found: $file"
            }
    
            $JSON = Get-Content $file

            Try{
                Write-Verbose "[$($MyInvocation.MyCommand)] Importing variables as JSON from: $file"
                $obj = ($JSON | ConvertFrom-Json).variables                
            }

            Catch{
                throw $_
            }
    
            return $obj
        }        

        function Validate-Scope([string]$Name,[string]$value,[Octopus.Client.Model.VariableScopeValues]$scopes){
    
            $ID = $variableSet.ScopeValues.$Name | ?{$_.name -eq $value} | select -ExpandProperty ID

            If(!$ID){
                Throw "Value [$value] was not found on the scope for [$name]"
            }
            else{
                return $ID
            }
        }

        function create-VariableSet([PSObject]$FileVariables, [Octopus.Client.Model.VariableSetResource]$variableSet){
            
            $NewVariableSet = New-Object Octopus.Client.Model.VariableSetResource
            
            Write-Verbose "[$($MyInvocation.MyCommand)] Creating variable set out of file"

            foreach($var in $FileVariables){                

                $newvar = New-Object Octopus.Client.Model.VariableResource        
                $newvar.Name = $var.Name
                $newvar.Value = $var.Value
                $newvar.IsEditable = $var.IsEditable
                $newvar.IsSensitive = $var.IsSensitive
        
                If(!([string]::IsNullOrEmpty($var.Scope))){
                    foreach($scope in ($var.Scope | Get-Member | ?{$_.membertype -eq "NoteProperty"} | select -ExpandProperty Name)){
                        $scopeID = Validate-Scope -Name ($scope + "s") -value $var.scope.$scope -scopes $variableSet.ScopeValues
                        $newvar.Scope.Add([Octopus.platform.Model.Scopefield]::$scope, (New-Object Octopus.Platform.Model.ScopeValue($scopeID)))
                    }
                }

                $prompt = New-Object Octopus.Platform.Model.VariablePromptOptions

                $prompt.Description = $var.prompt.Description
                $prompt.Label = $var.prompt.Label
                $prompt.Required = $var.prompt.required

                $newvar.Prompt = $prompt
        
                $NewVariableSet.Variables.Add($newvar)                
            }            
            return $NewVariableSet
        }

        function comparescope([Octopus.Client.Model.VariableResource]$var1, [Octopus.Client.Model.VariableResource]$var2){
            Write-Verbose "comparing $($var1.name) and $($var2.name)"

            foreach($scopetype in $var1.Scope.Keys){
                Write-Output "Hello?"
                Try{                
                    If($var1.Scope.Count -eq 0){
                        $compare = $null
                        Write-Output "hello?"
                    }
                    else{
                        $compare = Compare-Object $var1.Scope["$scopetype"] $var2.Scope["$scopetype"]
                    }
                }
                Catch{                
                    $Identical = $false
                    break
                }
            
                If($compare -ne $null){
                    $Identical = $false
                    break
                }
                else{
                    $Identical = $true
                }            
        
            }
            Write-Verbose "Identical: $Identical"
            return $Identical
        }
        
        function VariableExists([Octopus.Client.Model.VariableResource]$variable ,[Octopus.Client.Model.VariableSetResource]$variableSet){
            foreach($var in $variableSet.Variables){
                If(($var.name -eq $variable.name) -and ($var.scope.keys.count -eq $variable.scope.Keys.count) -and ($var.scope.keys.count -ne 0)){
                    If((comparescope -var1 $var -var2 $variable) -eq $true){
                        $varExists = $true
                        break
                    }
                    Else{
                        $varExists = $false
                    }

                }

                If(($var.name -eq $variable.name) -and ($var.scope.keys.count -eq $variable.scope.Keys.count) -and ($var.scope.keys.count -eq 0)){
                    $varExists = $true
                    break
                }

                else{
                    $varExists = $false
                }
            }
            return $varExists
        }
        
        function Get-MergedVariableSet([Octopus.Client.Model.VariableSetResource]$base,[Octopus.Client.Model.VariableSetResource]$New) {

            $mergedVariableSet = $base    

            foreach ($Variable in $new.Variables){
                If(VariableExists -variable $Variable -variableSet $base){
                    $MergedvariableSet.Variables | ?{$_.name -eq $Variable.name} | %{
                        $_.value = $Variable.value 
                        $_.IsEditable = $Variable.IsEditable
                        $_.IsSensitive = $Variable.IsSensitive 
                        $_.Prompt = $prompt
                        
                        }
                }
                else{
                    $mergedVariableSet.variables.add($variable)
                }
            }
    
            return $mergedVariableSet          
        }
        
    }
    Process
    {
        If($PSCmdlet.ParameterSetName -eq 'Project'){
            $BaseVariableset = Get-OctopusVariableSet -Projectname $ProjectName -ResourceOnly
            }
        Else{
            $BaseVariableset = Get-OctopusVariableSet -LibrarySetName $LibraryVariableSet -ResourceOnly
        }

        If($Basevariableset){
            Write-Verbose "[$($MyInvocation.MyCommand)] Working with variable set of project: $ProjectName"
            Write-Verbose "[$($MyInvocation.MyCommand)] Variables on set before starting: $($Basevariableset.variables.count)"

            $fileVariables = Import-JsonFile -file $File

            $newVariableSet = create-VariableSet -FileVariables $fileVariables -variableSet $BaseVariableset

            If($Mode -eq "Overwrite"){
                $BaseVariableset.variables = $newVariableSet.variables
                $finalVarSet = $BaseVariableset
            }
            else{
                $finalVarSet = Get-MergedVariableSet -Base $BaseVariableset -New $newVariableSet
            }
        }
        else{
            Write-Verbose "[$($MyInvocation.MyCommand)] No variable set found for project: $name"
            break
        }        
    }
    End
    {
        Write-Verbose "[$($MyInvocation.MyCommand)] Returning Variable set: $($newVariableSet.id)"
        Write-Verbose "[$($MyInvocation.MyCommand)] Variables on set after script: $($Newvariableset.variables.count)"
        return $finalVarSet
    }
}