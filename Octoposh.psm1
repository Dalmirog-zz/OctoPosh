$scripts = Get-ChildItem $PSScriptRoot\scripts -Filter "*.ps1"

Add-Type -Path "$PSScriptRoot\bin\Newtonsoft.Json.dll"
Add-Type -Path "$PSScriptRoot\bin\Octopus.Client.dll"
Add-Type -Path "$PSScriptRoot\bin\Octopus.Platform.dll"

foreach ($script in $scripts){
. $script.FullName
}

##Helper Functions. These wont get exported by the module, but will be available to be used by the exported cmdlets
function Put-OctopusResource([string]$uri, [object]$resource) {
    Invoke-RestMethod -Method Put -Uri "$env:OctopusURL/$uri" -Body $($resource | ConvertTo-Json -Depth 10) -Headers $c.header -Verbose:$false
}

function Get-OctopusResource([string]$uri, [object]$header) {
    
    return Invoke-RestMethod -Method Get -Uri "$env:OctopusURL/$uri" -Headers $header -Verbose:$false
}

function Get-UserConfirmation{ #Credits to http://www.peetersonline.nl/2009/07/user-confirmation-in-powershell/
	
	        param([string]$title="Confirm`n",[string]$message)

	        $choiceYes = New-Object System.Management.Automation.Host.ChoiceDescription "&Yes", "Answer Yes."

	        $choiceNo = New-Object System.Management.Automation.Host.ChoiceDescription "&No", "Answer No."

	        $options = [System.Management.Automation.Host.ChoiceDescription[]]($choiceYes, $choiceNo)

	        $result = $host.ui.PromptForChoice($title, $message, $options, 1)

	        Switch ($result){
		        0{Return $true}
 
		        1{Return $false}
	        }
        }

#I'm quite sure there's an easier way to do this than using this ugly variable. Bet well...
function Get-OctopusVariableScopeValue{    
    param(
        [parameter(Mandatory=$true)]
        [Octopus.Client.Model.VariableSetResource]$Resource,

        [parameter(Mandatory=$true)]
        [String]$VariableName

    )

    $list = @()

    $varscopes = ($Resource.Variables | ?{$_.name -eq $variableName}).scope

    $scopevalues += $Resource.ScopeValues.Environments
    $scopevalues += $Resource.ScopeValues.Machines
    $scopevalues += $Resource.ScopeValues.Actions
    
    $varscopes.getenumerator() | %{        

        $value = @()
        If ($_.key -ne "Role"){
            foreach ($v in $_.value){
                $value += $scopevalues | ?{$_.Id -eq $v} | select -ExpandProperty name
            } 
        }

        else{$value = $_.value}

        $obj = [pscustomobject]@{
            Scope = $_.key
            value = $value
        }
        $list += $obj
    }
    
   return $list
}

#Only exporting cmdlets inside \Scripts
Export-ModuleMember $scripts.BaseName