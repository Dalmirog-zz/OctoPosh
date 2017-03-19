function New-OctopusVariable {
	[CmdletBinding()]
	Param (
		
		[Parameter(Mandatory=$true,ParameterSetName="resource")]
		[Octopus.Client.Model.VariableResource[]]$Resource,
		
		[Parameter(Mandatory=$true,ParameterSetName="resource")]
		[Octopus.Client.Model.VariableScopeValues]$ScopeValues,
		
		[Parameter(Mandatory=$true,ParameterSetName="raw")]
		[ValidateNotNullOrEmpty()]
		[string]$Name,
		
		[Parameter(ParameterSetName="raw")]
		[string]$Value,
		
		[Parameter(ParameterSetName="raw")]
		$Scope,
		
		[Parameter(ParameterSetName="raw")]
		[switch]$IsSensitive,
		
		[Parameter(ParameterSetName="raw")]
		[switch]$IsEditable,
		
		[Parameter(ParameterSetName="raw")]
		$Prompt

	)
	
	Begin {
		if ( $PsCmdlet.ParameterSetName -eq "raw" ) {
			Write-Verbose "Generating resource object from parameters..."
			$Resource = @{
				name = $Name
				value = $Value
				IsSensitive = ($false,$true)[,$IsSensitive]
				IsEditable = ($false,$true)[,$IsEditable]
				Prompt = $Prompt
			}
		}
	}
	
	Process {
		foreach ( $var in $Resource ) {
			if ( $PsCmdlet.ParameterSetName -eq "resource" ) {
				$Scope = Resolve-OctopusVariableScope -Scope $var.Scope -ScopeValues $ScopeValues
			}
			
			$result = [PSCustomObject]@{
				Name = $var.name
				Value = $var.value
				Scope = [PSCustomObject]$Scope
				IsSensitive = $var.IsSensitive
				IsEditable = $var.IsEditable
				Prompt = $var.Prompt
			}
			Write-Output $result
		}
	}
}