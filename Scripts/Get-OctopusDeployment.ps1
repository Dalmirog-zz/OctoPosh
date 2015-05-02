<#
.Synopsis
   Gets information about Octopus deployments
.DESCRIPTION
   Gets information about Octopus deployments
.EXAMPLE
   Get-OctopusDeployment

   Gets all the deployments that were done on the Octopus Instance. You might wanna go grab a coffee after hitting [enter] on this one, its gonna take a while.
.EXAMPLE
   Get-OctopusDeployment -ProjectName "MyProduct.*"

   Gets all the deployments from all the projects which name starts with "MyProduct.*"
.EXAMPLE
   Get-OctopusDeployment -EnvironmentName "Staging","UAT" -ProjectName "MyService"

   Gets all the deployents that were done to the environments Staging and UAT on the project "MyService"
.EXAMPLE
   Get-OctopusDeployment -project "MyProduct.Webapp","MyProduct.service" -Environment "Production"

   Gets all the deployments that were done to the environment "Production"  on the projects "MyProduct.webapp" and "MyProduct.service"
.EXAMPLE
   Get-OctopusDeployment -project "MyProduct.Webapp" -Environment "Production" -After 2/20/2015 -Before 2/21/2015

   Gets all the deployments that where done to the environment "Production" on the projects "MyProduct.Webapp" between 2/20/2015 and 2/21/2015
.LINK
   Github project: https://github.com/Dalmirog/OctopusDeploy-Powershell-module
#>
function Get-OctopusDeployment
{
    [CmdletBinding()]        
    Param
    (
        # Octopus environment name        
        [Parameter(ValueFromPipelineByPropertyName = $true)]
        [alias("Environment")]
        [string[]]$EnvironmentName = $null,

        # Octopus project name        
        [Parameter(ValueFromPipelineByPropertyName = $true)]
        [alias("Project")]
        [string[]]$ProjectName = $null,

        # Release version
        [Parameter(ValueFromPipelineByPropertyName = $true)]        
        [string[]]$ReleaseVersion = $null,

        #Before date
        [System.DateTimeOffset]$Before = [System.DateTimeOffset]::MaxValue,
        
        #After date
        [System.DateTimeOffset]$After = [System.DateTimeOffset]::MinValue

          
    )

    Begin
    {
        $c = New-OctopusConnection
        $list = @()
        $releases = @()
        $deployments = @()
    }
    Process
    {
        If($ProjectName -ne $null){
            
            $Projects = $c.repository.Projects.FindMany({param($Proj) if (($Proj.name -in $ProjectName)) {$true}})
        }

        else{
        
            $Projects = $c.repository.Projects.FindAll()
        }        

        If($EnvironmentName -ne $null){
            
            $environments = $c.repository.Environments.FindMany({param($env) if ($env.name -in $environmentName) {$true}})                        
                        
        }

        Else {$environments = $c.repository.Environments.FindAll()}

        foreach ($Project in $Projects){

            If($ReleaseVersion-ne $null){

                foreach ($V in $ReleaseVersion){
                    Try{       
                        $rel = $c.repository.Projects.GetReleaseByVersion($Project,$v)
                    }

                    Catch [Octopus.Client.Exceptions.OctopusResourceNotFoundException]{
                        write-host "No releases found for project $($Project.name) with the ID $v"
                        $rel = $null
                    }                
                 }
             }       
            
            Else{
                $rel = ($c.repository.Projects.GetReleases($Project)).items
            }
                            
            If ($rel -ne $null){
                foreach ($r in $rel){
                    $deployments += ($c.repository.Releases.GetDeployments($r)).items | ?{$_.environmentID -in $environments.id} |?{($_.created -gt $after) -and ($_.created -lt $Before)}
                    $releases += $r
                }
            }
        }                  
        
        foreach ($d in $deployments){

            $p = $Projects | ? {$_.id -eq $d.projectid}
            $e = $environments | ? {$_.id -eq $d.environmentID}
            $t = $c.repository.Tasks.Get($d.Links.task)
            $r = $releases | ? {$_.id -eq $d.ReleaseID}
            $dp = $c.repository.DeploymentProcesses.Get($r.links.ProjectDeploymentProcessSnapshot)
            $dev = (Invoke-WebRequest -Uri "$env:OctopusURL/api/events?regarding=$($d.Id)" -Method Get -Headers $c.header | ConvertFrom-Json).items | ? {$_.category -eq "DeploymentQueued"}
            
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
            $obj = [PSCustomObject]@{
                            ProjectName = $p.name
                            EnvironmentName = $e.name
                            DeploymentstartTime = ($t.Starttime).DateTime
                            DeploymentEndTime = ($t.Completedtime).DateTime
                            DeploymentStartedBy = $dev.Username
                            Duration = [math]::Round($duration,2)
                            Status = $t.state                           
                            ReleaseVersion = $r.version
                            ReleaseCreationDate = ($r.assembled).DateTime
                            ReleaseNotes = $r.ReleaseNotes                            
                            Package = $Packages
                            Resource = $d
                        }                                    
            $list += $obj
        }
        
    }
    End
    {
        return $list
    }
}