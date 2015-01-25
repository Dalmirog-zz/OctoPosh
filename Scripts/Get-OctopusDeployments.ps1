<#
.Synopsis
   Gets information about Octopus deployments
.DESCRIPTION
   Gets information about Octopus deployments
.EXAMPLE
   Example of how to use this cmdlet
.EXAMPLE
   Another example of how to use this cmdlet
.LINK
   Github project: https://github.com/Dalmirog/OctopusDeploy-Powershell-module
#>
function Get-OctopusDeployments
{
    [CmdletBinding()]        
    Param
    (
        # Octopus instance URL
        [Parameter(Mandatory=$false)]
        [string]$OctopusURI = $env:OctopusURI,

        # Octopus API Key. How to create an API Key = http://docs.octopusdeploy.com/display/OD/How+to+create+an+API+key
        [Parameter(Mandatory=$false)]
        [string]$APIKey = $env:OctopusAPIKey        
    )

    Begin
    {

        $c = New-OctopusConnection

        $deployments = $c.repository.Deployments.FindAll()[0]
        
        $list = @()
        $packages = @()

    }
    Process
    {

        foreach ($d in $deployments){

            $p = $c.repository.projects.Get($d.Links.project)
            $e = $c.repository.Environments.Get($d.Links.Environment)
            $t = $c.repository.Tasks.Get($d.Links.task)
            $r = $c.repository.Releases.Get($d.Links.Release)
            $dp = $c.repository.DeploymentProcesses.Get($r.links.ProjectDeploymentProcessSnapshot)
            
            
            if (($t.Duration).Split(" ")[1] -eq "seconds"){
                [datetime]$time = "00:00:00"
                $t.Duration = ($time.AddSeconds(($t.Duration).Split(" ")[0])).TimeOfDay              
            }
            elseif (($t.Duration).Split(" ")[1] -eq "minutes" -or "minute"){
                [datetime]$time = "00:00:00"
                $t.Duration = ($time.AddMinutes(($t.Duration).Split(" ")[0])).TimeOfDay
            }
            
            foreach ($s in $r.SelectedPackages){

                $ds = $dp.steps | ? {$_.name -eq "$($s.stepname)"} 

                $properties = [ordered]@{
                    Packagename = $ds.Actions.properties.'Octopus.Action.Package.NuGetPackageId'
                    PackageVersion = $s.version
                }

                $Packages += $entry = New-Object psobject -Property $properties

            }

            $property = [ordered]@{
                           Project = $p.name
                           Environment = $e.name
                           Date = ($t.queuetime).DateTime
                           Duration = ($t.Duration)
                           Status = $t.state
                           ReleaseVersion = $r.version
                           Assembled = ($r.assembled).DateTime
                           Packages = $Packages
                       }

            $list += $obj = new-object psobject -Property $property
        }

    }
    End
    {
        return $list
    }
}