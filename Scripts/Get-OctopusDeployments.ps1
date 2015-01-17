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

    $p = $repository.projects.Get($d.Links.project)
    $e = $repository.Environments.Get($d.Links.Environment)
    $t = $repository.Tasks.Get($d.Links.task)
    $r = $repository.Releases.Get($d.Links.Release)
       
    if (($t.Duration).Split(" ")[1] -eq "seconds"){
        [datetime]$time = "00:00:00"
        $t.Duration = ($time.AddSeconds(($t.Duration).Split(" ")[0])).TimeOfDay              
    }
    elseif (($t.Duration).Split(" ")[1] -eq "minutes" -or "minute"){
        [datetime]$time = "00:00:00"
        $t.Duration = ($time.AddMinutes(($t.Duration).Split(" ")[0])).TimeOfDay
    }
    

    $property = [ordered]@{
                   Project = $p.name
                   Environment = $e.name
                   Date = ($t.queuetime).DateTime
                   Duration = ($t.Duration)
                   Status = $t.state
                   ReleaseVersion = $r.version
                   Assembled = ($r.assembled).DateTime
               }

    $list += $obj = new-object psobject -Property $property
    
     #$obj
}

#$list | select duration |Format-Table

$list |Format-Table