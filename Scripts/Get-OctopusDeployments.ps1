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

$list = @()

$deployments = $repository.Deployments.FindAll()

foreach ($d in $deployments){

    $property = [ordered]@{
                   Project = $repository.projects.Get($d.Links.project).name
                   Environment = $repository.Environments.Get($d.Links.Environment).name
                   Date = $repository.Tasks.Get($d.Links.task).queuetime
                   Duration = $repository.Tasks.Get($d.Links.task).Duration
                   Status = $repository.Tasks.Get($d.Links.task).state
                   Version = $repository.Releases.Get($d.Links.Release).version
                   Assembled = $repository.Releases.Get($d.Links.Release).assembled
               }

    $obj = new-object psobject -Property $property
    
    $list += $obj
}

$list | select Date |Format-Table