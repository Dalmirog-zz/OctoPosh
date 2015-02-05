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
        [string[]]$EnvironmentName = "*",

        # Octopus project name
        [Parameter(Mandatory=$false)]
        [string[]]$ProjectName = "*"
          
    )

    Begin
    {

        $c = New-OctopusConnection

        #Getting EnvironmentIDs and ProjectIDs based on values set on parameters
        if($ProjectName -ne "*"){

            $projectid = ($c.repository.Projects.FindMany({param($proj) if ($proj.name -in $ProjectName) {$true}})).id
                                    
            }
        else {$projectid = "*"}

        if($EnvironmentName -ne "*"){
            
            $environmentid = ($c.repository.Environments.FindMany({param($env) if ($env.name -in $environmentName) {$true}})).id
            
            }

        else {$Environmentid = "*"}

        #Getting deployments based on EnvironmentIds and ProjectIds
        $deployments = $c. repository.Deployments.FindMany({param($dep) if ((($dep.projectid -in $projectid) -or ($dep.projectid -like $projectid)) -and (($dep.environmentid -in $environmentid) -or ($dep.environmentid -like $environmentid))) {$true}})

       
        
        

    }
    Process
    {

        $list = @()

        foreach ($d in $deployments){

            $p = $c.repository.projects.Get($d.Links.project)
            $e = $c.repository.Environments.Get($d.Links.Environment)
            $t = $c.repository.Tasks.Get($d.Links.task)
            $r = $c.repository.Releases.Get($d.Links.Release)
            $dp = $c.repository.DeploymentProcesses.Get($r.links.ProjectDeploymentProcessSnapshot)
            
            #Getting Nuget packages and their versions
            $packages = @()
            
            foreach ($s in $r.SelectedPackages){

                $ds = $dp.steps | ? {$_.name -eq "$($s.stepname)"} 

                $properties = [ordered]@{
                    Name = $ds.Actions.properties.'Octopus.Action.Package.NuGetPackageId'
                    Version = $s.version
                }

                $Packages += $entry = New-Object psobject -Property $properties

            }

            #Duration calculation needed cause "timed out" deployments dont have a value set for "CompletedTime"
            if($t.completedtime){
                
                $duration = (New-TimeSpan –Start ($t.Starttime).DateTime –End ($t.Completedtime).DateTime).TotalMinutes

                }

            else{$duration = 0}

            #Creating output object
            $properties = [ordered]@{
                            ProjectName = $p.name
                            EnvironmentName = $e.name
                            DeploymentstartTime = ($t.Starttime).DateTime
                            DeploymentEndTime = ($t.Completedtime).DateTime
                            DeploymentStartedBy = $d.LastModifiedBy
                            Duration = [math]::Round($duration,2)
                            Status = $t.state                           
                            ReleaseVersion = $r.version
                            ReleaseCreationDate = ($r.assembled).DateTime
                            ReleaseNotes = $r.ReleaseNotes
                            ReleaseCreatedBy = $r.LastModifiedBy
                            Package = $Packages
                            Resource = $d
                        }

            #Adding object to list
            $list += $obj = new-object psobject -Property $properties
        }

    }
    End
    {
        return $list
    }
}