#region Loading necesary assemblies
Add-Type -Path "$PSScriptRoot\bin\Newtonsoft.Json.dll"
Add-Type -Path "$PSScriptRoot\bin\Octopus.Client.dll"
#endregion

#region Dot sourcing and exporting all scripts inside \Scripts.
#I keep reading everywhere that dot sourcing is not a recommended practice, but having all the functions in a single PSM1 file is way too uncomfortable for me.
#If there's a way to export functions from a separate file without dot-sourcing, I'll be MORE than glad to hear about it/accept a PR.
$scripts = Get-ChildItem $PSScriptRoot\scripts -Filter "*.ps1"

foreach ($script in $scripts){
. $script.FullName
}

Export-ModuleMember $scripts.BaseName
#endregion

#region Helper Functions. These wont get exported by the module, but will be available to be used by the exported cmdlets
function Put-OctopusResource([string]$uri, [object]$resource) {
    Invoke-RestMethod -Method Put -Uri "$env:OctopusURL/$uri" -Body $($resource | ConvertTo-Json -Depth 10) -Headers $c.header -Verbose:$false
}

function Post-OctopusResource([string]$uri, [object]$resource) {
    Invoke-RestMethod -Method Post -Uri "$env:OctopusURL/$uri" -Body $($resource | ConvertTo-Json -Depth 10) -Headers $c.header -Verbose:$false
}

function Get-OctopusResource([string]$uri) {
    return Invoke-RestMethod -Method Get -Uri "$env:OctopusURL/$uri" -Headers $c.header -Verbose:$false
}

function Get-UserConfirmation {
	#Credits to http://www.peetersonline.nl/2009/07/user-confirmation-in-powershell/
	param (
		[string]$title="Confirm`n",
		[string]$message
	)
	$choiceYes = New-Object System.Management.Automation.Host.ChoiceDescription "&Yes", "Answer Yes."
	$choiceNo = New-Object System.Management.Automation.Host.ChoiceDescription "&No", "Answer No."
	$options = [System.Management.Automation.Host.ChoiceDescription[]]($choiceYes, $choiceNo)
	$result = $host.ui.PromptForChoice($title, $message, $options, 1)
	Switch ($result) {
		0{Return $true}
		1{Return $false}
	}
}

# Replaces Get-OctopusVariableScopeValue
function Resolve-OctopusVariableScope {
	[CmdletBinding()]
	Param (
		[Parameter(Mandatory=$true)]
		[Octopus.Client.Model.ScopeSpecification]$Scope,
		
		[Parameter(Mandatory=$true)]
		[Octopus.Client.Model.VariableScopeValues]$ScopeValues
	)
	foreach ( $key in $Scope.Keys ) {
		$scopeType = $key.ToString()
		$values = @()
		foreach ( $item in $Scope.Item($scopeType) ) {
			$values += ( $ScopeValues."$($scopeType)s" | Where-Object { $_.Id -eq $item } ).Name
		}
		Write-Output ([pscustomobject]@{ Scope = $scopeType; value = $values})
	}
}

#endregion