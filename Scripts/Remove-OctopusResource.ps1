<#
.Synopsis
   Deletes Octopus Deploy resources such as Projects, Releases, Environments, etc
.DESCRIPTION
   Deletes Octopus Deploy resources such as Projects, Releases, Environments, etc
.EXAMPLE
   $ProjectResource = Get-OctopusProject -name "MyApp" ;
   Remove-OctopusResource -resource $ProjectResource
.EXAMPLE
   Get-OctopusEnvironment -name "Development" | Remove-OctopusResource -force
.LINK
   Github project: https://github.com/Dalmirog/OctopusDeploy-Powershell-module
#>
function Remove-OctopusResource
{
    [CmdletBinding()]
    Param
    (
        # Octopus resource object
        [Parameter(Mandatory=$true,
                   ValueFromPipelineByPropertyName=$true,
                   Position=0)]
        $Resource,

        # Forces resource delete.
        [switch]$Force

    )

    Begin
    {
        $c = New-OctopusConnection

        Function Get-UserConfirmation{ #Credits to http://www.peetersonline.nl/2009/07/user-confirmation-in-powershell/
	
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
    }
    Process
    {
        Foreach ($r in $Resource){

            if(!($Force)){

                If (!(Get-UserConfirmation -message "Are you sure you want to delete this resource? `n`n$resource`n")){
                    break
                }

            }

            switch ($Resource)
            {
                {$_.getType() -eq [Octopus.Client.Model.ProjectGroupResource]} {$c.repository.ProjectGroups.Delete($_)}
                {$_.getType() -eq [Octopus.Client.Model.ProjectResource]} {$c.repository.Projects.Delete($_)}
                {$_.getType() -eq [Octopus.Client.Model.EnvironmentResource]} {$c.repository.Environments.Delete($_)}
                {$_.getType() -eq [Octopus.Client.Model.DeploymentResource]} {$c.repository.Deployments.Delete($_)}          
                
                Default {throw "Invalid object type. Run 'Get-OctopusResourceModel -ListAvailable' to get a list of the object types accepted by this cmdlet "}
            }

        }
    }
    End
    {
        
    }
}