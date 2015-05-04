$scripts = Get-ChildItem $PSScriptRoot\scripts -Filter "*.ps1"

Add-Type -Path "$PSScriptRoot\bin\Newtonsoft.Json.dll"
Add-Type -Path "$PSScriptRoot\bin\Octopus.Client.dll"
Add-Type -Path "$PSScriptRoot\bin\Octopus.Platform.dll"

foreach ($script in $scripts){
. $script.FullName
}

##Helper Functions. These wont get exported by the module, but will be available to be used by the exported cmdlets
function Put-OctopusResource([string]$uri, [object]$resource) {
    Write-Host "[PUT]: $uri"
    Invoke-RestMethod -Method Put -Uri "$env:OctopusURL/$uri" -Body $($resource | ConvertTo-Json -Depth 10) -Headers $c.header
}

function Get-OctopusResource([string]$uri, [object]$header) {
    
    return Invoke-RestMethod -Method Get -Uri "$env:OctopusURL/$uri" -Headers $header
}

function Get-UserConfirmation{ #Credits to http://www.peetersonline.nl/2009/07/user-confirmation-in-powershell/
	
	        param([string]$title="Confirm",[string]$message)

	        $choiceYes = New-Object System.Management.Automation.Host.ChoiceDescription "&Yes", "Answer Yes."

	        $choiceNo = New-Object System.Management.Automation.Host.ChoiceDescription "&No", "Answer No."

	        $options = [System.Management.Automation.Host.ChoiceDescription[]]($choiceYes, $choiceNo)

	        $result = $host.ui.PromptForChoice($title, $message, $options, 1)

	        Switch ($result){
		        0{Return $true}
 
		        1{Return $false}
	        }
        }

#Only exporting cmdlets inside \Scripts
Export-ModuleMember $scripts.BaseName