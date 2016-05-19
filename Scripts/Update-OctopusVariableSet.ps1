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
    
            $JSON = Get-Content $file -Raw

            Try{
                Write-Verbose "[$($MyInvocation.MyCommand)] Importing variables as JSON from: $file"
                $obj = ($JSON | ConvertFrom-Json).variables                
            }

            Catch{
                throw $_
            }
    
            return $obj
        }        
        
        function Get-ScopeId([string] $name, $value) {
            $isArray = ($value -is [System.Array])
            if (-not $isArray) {
                $value = @($value)
            }
                        
            $scopeValues = $variableSet.ScopeValues.$Name
 
            foreach($v in $value) {               
                $ID = $scopeValues | ?{$_.name -eq $v} | select -ExpandProperty ID
                if ($Id) {
                    return $Id
                }
            }
            
            
        }

        function create-VariableSet([PSObject]$FileVariables, [Octopus.Client.Model.VariableSetResource]$variableSet){
            
            $NewVariableSet = New-Object Octopus.Client.Model.VariableSetResource
            
            Write-Verbose "[$($MyInvocation.MyCommand)] Creating variable set out of file"

            foreach($var in $FileVariables){                

                $variableName = $var.Name

                $newvar = New-Object Octopus.Client.Model.VariableResource        
                $newvar.Name = $var.Name
                $newvar.Value = $var.Value
                $newvar.IsEditable = $var.IsEditable
                $newvar.IsSensitive = $var.IsSensitive
        
                If(!([string]::IsNullOrEmpty($var.Scope))){

                    foreach($scope in ($var.Scope | Get-Member | ?{$_.membertype -eq "NoteProperty"} | select -ExpandProperty Name)){
                        $name = ($scope + "s")
                        $value = $var.scope.$scope
                        
                        if (-not $value -is [System.Array]) {
                            $value = @($value)
                        }
                        
                        $scopeIds = $value | foreach { Get-ScopeId $name $_ }
                        foreach($scopeId in $scopeIds) {                                   
                            If(!$scopeId){
                                Throw "Value [$scopeValue] was not found on the scope for [$name] for variable $variableName"
                            }          
                        }
                        
                        write-verbose "Adding $variableName $scope $value"
                        $newvar.Scope.Add([Octopus.Client.Model.Scopefield]::$scope, (New-Object Octopus.Client.Model.ScopeValue($scopeIds)))    
                    }

                }

                If(!([string]::IsNullOrEmpty($var.Prompt))){
                   $prompt = New-Object Octopus.Client.Model.VariablePromptOptions
   
                   $prompt.Description = $var.prompt.Description
                   $prompt.Label = $var.prompt.Label
                   $prompt.Required = $var.prompt.required
   
                   $newvar.Prompt = $prompt
                } Else {
                    $newvar.Prompt = $var.prompt
                }
        
                $NewVariableSet.Variables.Add($newvar)                
            }            
            return $NewVariableSet
        }

        function comparescope([Octopus.Client.Model.VariableResource]$var1, [Octopus.Client.Model.VariableResource]$var2){
            foreach($scopetype in $var1.Scope.Keys){
                Try{                
                    $scope1 = $var1.Scope["$scopetype"]
                    $scope2 = $var2.Scope["$scopetype"]
                    
                    If($var1.Scope.Count -eq 0){
                        $compare = $null
                        Write-Output "hello?"
                    }
                    else{
                        $compare = Compare-Object $scope1 $scope2
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
            return $Identical
        }
        
         
        function Get-MatchingVariable([Octopus.Client.Model.VariableResource]$variable ,[Octopus.Client.Model.VariableSetResource]$variableSet){
            foreach($var in $variableSet.Variables){
                
                if ($var.name -ne $variable.name) {continue;}
                if ($var.scope.keys.count -ne $variable.scope.keys.count) {continue;}
                if ($var.scope.keys.count -eq 0) {return $var}

                If((comparescope -var1 $var -var2 $variable) -eq $true){
                    return $var
                }
            }
            return $null
        }
         
        function Get-MergedVariableSet([Octopus.Client.Model.VariableSetResource]$mergedVariableSet, [Octopus.Client.Model.VariableSetResource]$New) {
            $variableSetName = $mergedVariableSet.Name
            write-verbose "Merging changes to variable set $variableSetName"

            foreach ($Variable in $new.Variables){
                $matchingVariable = Get-MatchingVariable $variable $mergedVariableSet
                
                $variableName = $variable.Name
                
                if ($matchingVariable) {
                    write-verbose "Updating matching variable $variableName on $mergedVariableSetName"
                    $matchingVariable.value = $Variable.value 
                    $matchingVariable.IsEditable = $Variable.IsEditable
                    $matchingVariable.IsSensitive = $Variable.IsSensitive 
                    $matchingVariable.Prompt = $prompt
                }
                else {
                    write-verbose "Adding new variable $variableName to $mergedVariableSetName"
                    $mergedVariableSet.Variables.add($variable)
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
                $finalVarSet = Get-MergedVariableSet -mergedVariableSet $BaseVariableset -New $newVariableSet
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
