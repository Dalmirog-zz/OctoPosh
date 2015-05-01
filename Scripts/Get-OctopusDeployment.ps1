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
   Get-OctopusDeployment -project "MyProduct.*"
.EXAMPLE
   Get-OctopusDeployment -Environment "Staging","UAT"
.EXAMPLE
   Get-OctopusDeployment -Environment "Staging-*","Production"
.EXAMPLE
   Get-OctopusDeployment -project "MyProduct.Webapp","MyProduct.service" -Environment "Production"
.EXAMPLE
   Get-OctopusDeployment -project "MyProduct.Webapp" -Environment "Production" -After 2/20/2015 -Before 2/21/2015
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
        [string[]]$EnvironmentName = "*",

        # Octopus project name        
        [Parameter(ValueFromPipelineByPropertyName = $true)]
        [string[]]$ProjectName = "*",

        #Before date
        [System.DateTimeOffset]$Before = [System.DateTimeOffset]::MaxValue,
        
        #After date
        [System.DateTimeOffset]$After = [System.DateTimeOffset]::MinValue

          
    )

    Begin
    {
        $c = New-OctopusConnection
        $list = @()
    }
    Process
    {
        #Getting EnvironmentIDs and ProjectIDs based on values set on parameters
        if($ProjectName -ne "*"){

            $projectid = ($c.repository.Projects.FindMany({param($proj) if ($proj.name -in $ProjectName) {$true}})).id
                                    
            }
        else {$projectid = "*"}

        if($EnvironmentName -ne "*"){
            
            $environmentid = ($c.repository.Environments.FindMany({param($env) if ($env.name -in $environmentName) {$true}})).id
            
            }

        else {$Environmentid = "*"}

        #Getting deployments based on EnvironmentIds, ProjectIds, created $Before and $After
        $deployments = $c. repository.Deployments.FindMany(`
            
            {param($dep) if (`
                (($dep.projectid -in $projectid) -or ($dep.projectid -like $projectid))`
                 -and (($dep.environmentid -in $environmentid) -or ($dep.environmentid -like $environmentid))`
                 -and (($dep.created -ge $After) -and ($dep.created -le $Before)))`
            {$true}})

        foreach ($d in $deployments){

            $p = $c.repository.projects.Get($d.Links.project)
            $e = $c.repository.Environments.Get($d.Links.Environment)
            $t = $c.repository.Tasks.Get($d.Links.task)
            $r = $c.repository.Releases.Get($d.Links.Release)
            $dp = $c.repository.DeploymentProcesses.Get($r.links.ProjectDeploymentProcessSnapshot)
            $dev = (Invoke-WebRequest -Uri "$env:OctopusURL/api/events?regarding=$($d.Id)" -Method Get -Headers $c.header | ConvertFrom-Json).items | ? {$_.category -eq "DeploymentQueued"}
            $rev = (Invoke-WebRequest -Uri "$env:OctopusURL/api/events?regarding=$($r.Id)" -Method Get -Headers $c.header | ConvertFrom-Json).items | ? {$_.category -eq "Created"}

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
                            ReleaseCreatedBy = $rev.Username
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