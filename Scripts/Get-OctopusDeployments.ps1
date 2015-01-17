Add-Type -Path "C:\Program Files\Octopus Deploy\Tentacle\Newtonsoft.Json.dll"
Add-Type -Path "C:\Program Files\Octopus Deploy\Tentacle\Octopus.Client.dll"
Add-Type -Path "C:\Program Files\Octopus Deploy\Tentacle\Octopus.Platform.dll"

#Connection Data
$apikey = "API-7CH6XN0HHOU7DDEEUGKUFUR1K"
$OctopusURI = "http://localhost"
$headers = @{"X-Octopus-ApiKey"="$($apikey)";}

#Create endpoint connection
$endpoint = new-object Octopus.Client.OctopusServerEndpoint "$($OctopusURI)","$($apikey)"
$repository = new-object Octopus.Client.OctopusRepository $endpoint



$deployments = $repository.Deployments.FindAll()[0]

foreach ($d in $deployments){
    Write-Output "Project: $($repository.projects.Get($d.Links.project).name)"
    Write-Output "Environment: $($repository.Environments.Get($d.Links.Environment).name)"
    Write-Output "Date: $($repository.Tasks.Get($d.Links.task).queuetime)"
    Write-Output "Duration: $($repository.Tasks.Get($d.Links.task).Duration)"
    Write-Output "Status: $($repository.Tasks.Get($d.Links.task).state)"
    Write-Output "Version: $($repository.Releases.Get($d.Links.Release).version)"
    Write-Output "Assembled: $($repository.Releases.Get($d.Links.Release).assembled)"
    
}