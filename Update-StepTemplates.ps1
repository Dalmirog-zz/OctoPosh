#Make sure you have the correcct Template ID before running this script.
#To get the Id, open the template on the Octopus web UI
$templateID = "Actiontemplates-3"

#Loading Octopus.client assemblies
Add-Type -Path "C:\Program Files\Octopus Deploy\Tentacle\Newtonsoft.Json.dll"
Add-Type -Path "C:\Program Files\Octopus Deploy\Tentacle\Octopus.Client.dll"
Add-Type -Path "C:\Program Files\Octopus Deploy\Tentacle\Octopus.Platform.dll" 

#Connection Data
$apikey = "API-RLMWLZBPMX5DRPLCRNZEOT24HA"
$OctopusURI = "http://localhost"
$headers = @{"X-Octopus-ApiKey"="$($apikey)";}

$endpoint = new-object Octopus.Client.OctopusServerEndpoint "$($OctopusURI)","$($apikey)"
$repository = new-object Octopus.Client.OctopusRepository $endpoint

$template = Invoke-WebRequest -Uri $OctopusURI/api/actiontemplates/$templateID -Method Get -Headers $headers | select -ExpandProperty content| ConvertFrom-Json

$usage = Invoke-WebRequest -Uri $OctopusURI/api/actiontemplates/$templateID/usage -Method Get -Headers $headers | select -ExpandProperty content | ConvertFrom-Json

#Getting all the DeploymentProcesses that need to be updated
$deploymentprocesstoupdate = $usage | ?{$_.version -lt $template.Version}

If($deploymentprocesstoupdate -eq $null){

    Write-Output "All the deployment processes using template '$($template.name)' are using the latest version of it"

}

Else{

    Foreach($d in $deploymentprocesstoupdate){

        Write-Output "Updating deployment process of project: $($d.projectname)"

        #Getting DeploymentProcess obj
        $process = $repository.DeploymentProcesses.Get($d.DeploymentProcessId)

        #Finding the step that uses the step template
        $step = $process.Steps | ?{$_.actions.properties.values -eq $template.Id}

        foreach($prop in $step.Actions.properties.keys){

            $step.Actions.properties.$prop = $template.Properties.$prop

        }

        #Updating the Template.Version property to the latest
        $step.Actions.properties.'Octopus.Action.Template.version' = $template.Version

        #Saving the updated DeploymentProcess obj on the database
        $repository.DeploymentProcesses.Modify($process) | Out-Null
    }
           
}
