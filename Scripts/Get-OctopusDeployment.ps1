<#
.Synopsis
   Gets information about Octopus deployments
.DESCRIPTION
   Gets information about Octopus deployments
.EXAMPLE
   Get-OctopusDeployment
.EXAMPLE
   Get-OctopusDeployment -project "MyProduct.Webapp"
.EXAMPLE
   Get-OctopusDeployment -Environment "Staging","UAT"
.EXAMPLE
   Get-OctopusDeployment -project "MyProduct.Webapp","MyProduct.service" -Environment "Production"
.LINK
   Github project: https://github.com/Dalmirog/OctopusDeploy-Powershell-module
#>
function Get-OctopusDeployment
{
    [CmdletBinding()]        
    Param
    (
        # Octopus instance URL
        [Parameter(Mandatory=$false)]
        [string]$OctopusURI = $env:OctopusURI,

        # Octopus API Key. How to create an API Key = http://docs.octopusdeploy.com/display/OD/How+to+create+an+API+key
        [Parameter(Mandatory=$false)]
        [string]$APIKey = $env:OctopusAPIKey,

        # Octopus environment name
        [Parameter(Mandatory=$false)]
        [string[]]$Environment = "*",

        # Octopus project name
        [Parameter(Mandatory=$false)]
        [string[]]$Project = "*"        
          
    )

    Begin
    {

        $c = New-OctopusConnection

        if($Project -ne "*"){

            $project = ($c.repository.Projects.FindMany({param($proj) if ($proj.name -in $Project) {$true}})).id
                                    
            }

        if($Environment -ne "*"){
            
            $environment = ($c.repository.Environments.FindMany({param($env) if ($env.name -in $environment) {$true}})).id
            
            }

        $deployments = $c. repository.Deployments.FindMany({param($dep) if ((($dep.projectid -in $Project) -or ($dep.projectid -like $Project)) -and (($dep.environmentid -in $Environment) -or ($dep.environmentid -like $Environment))) {$true}})

       
        $list = @()
        

    }
    Process
    {

        foreach ($d in $deployments){

            $p = $c.repository.projects.Get($d.Links.project)
            $e = $c.repository.Environments.Get($d.Links.Environment)
            $t = $c.repository.Tasks.Get($d.Links.task)
            $r = $c.repository.Releases.Get($d.Links.Release)
            $dp = $c.repository.DeploymentProcesses.Get($r.links.ProjectDeploymentProcessSnapshot)
            
            <#
            [datetime]$time = "00:00:00"

            #$t.duration -eq "less than a second"
            
            if (($t.Duration).Split(" ")[1] -eq "seconds"){
                #[datetime]$time = "00:00:00"
                $t.Duration = ($time.AddSeconds(($t.Duration).Split(" ")[0])).TimeOfDay              
            }
            if (($t.Duration).Split(" ")[1] -eq ("minutes" -or "minute")){
                #[datetime]$time = "00:00:00"
                
                $t.Duration = ($time.AddMinutes(($t.Duration).Split(" ")[0])).TimeOfDay
            }
            if ($t.duration -eq "less than a second"){
                #[datetime]$time = "00:00:00"                
                $t.Duration = $time.TimeOfDay
            }
            if (($t.Duration).Split(" ")[1] -eq ("days" -or "day")){
                #$t.Duration = ($time.Addhours((($t.Duration).Split(" ")[0]) * 24)).TimeOfDay
            
            }

            #>
            $packages = @()
            
            foreach ($s in $r.SelectedPackages){

                $ds = $dp.steps | ? {$_.name -eq "$($s.stepname)"} 

                $properties = [ordered]@{
                    Name = $ds.Actions.properties.'Octopus.Action.Package.NuGetPackageId'
                    Version = $s.version
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
                           Package = $Packages
                       }

            $list += $obj = new-object psobject -Property $property
        }

    }
    End
    {
        return $list
    }
}