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
        [object[]]$Resource,

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

                If (!(Get-UserConfirmation -message "Are you sure you want to delete this resource? `n`n$($Resource.name)`n")){
                    Throw "Canceled by user"
                }

            }

            switch ($Resource)
            {
                {$_.getType() -eq [Octopus.Client.Model.ProjectGroupResource]} {$r = $c.repository.ProjectGroups.Delete($_)}
                {$_.getType() -eq [Octopus.Client.Model.ProjectResource]} {$r = $c.repository.Projects.Delete($_)}
                {$_.getType() -eq [Octopus.Client.Model.EnvironmentResource]} {$r = $c.repository.Environments.Delete($_)}
                {$_.getType() -eq [Octopus.Client.Model.DeploymentResource]} {$r = $c.repository.Deployments.Delete($_)}          
                
                Default {throw "Invalid object type. Run 'Get-OctopusResourceModel -ListAvailable' to get a list of the object types accepted by this cmdlet "}
            }

            $counter = 0

            Do{

                $task = $c.repository.Tasks.Get($r.Links.Self)
                $counter++
                Start-Sleep -Seconds 2

              }
            Until (($task.state -notin ("Queued","executing")) -or ($counter -eq 5))

        }
    }
    End
    {
        return $task
    }
}