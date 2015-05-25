<#
.Synopsis
   Gets information about Octopus deployments
.DESCRIPTION
   Gets information about Octopus deployments
.EXAMPLE
   Get-OctopusDeployment

   Get all the deployments that were done on the Octopus Instance. You might wanna go grab a coffee after hitting [enter] on this one, its gonna take a while.
.EXAMPLE
   Get-OctopusDeployment -ProjectName "MyProduct.*"

   Get all the deployments from all the projects which name starts with "MyProduct.*"
.EXAMPLE
   Get-OctopusDeployment -EnvironmentName "Staging","UAT" -ProjectName "MyService"

   Get all the deployents that were done to the environments Staging and UAT on the project "MyService"
.EXAMPLE
   Get-OctopusDeployment -project "MyProduct.Webapp","MyProduct.service" -Environment "Production"

   Get all the deployments that were done to the environment "Production"  on the projects "MyProduct.webapp" and "MyProduct.service"
.EXAMPLE
   Get-OctopusDeployment -project "MyProduct.Webapp" -Environment "Production" -After 2/20/2015 -Before 2/21/2015

   Get all the deployments that were done to the environment "Production" on the projects "MyProduct.Webapp" between 2/20/2015 and 2/21/2015
.LINK
   Github project: https://github.com/Dalmirog/Octoposh
   Advanced Cmdlet Usage: https://github.com/Dalmirog/OctoPosh/wiki/Advanced-Examples
   QA and Cmdlet request: https://gitter.im/Dalmirog/OctoPosh#initial
#>
function Get-OctopusDeployment
{
    [CmdletBinding()]        
    Param
    (
        # Environment Name
        [Parameter(ValueFromPipelineByPropertyName = $true)]
        [alias("Environment")]
        [string[]]$EnvironmentName,

        # Project Name        
        [Parameter(ValueFromPipelineByPropertyName = $true)]
        [alias("Project")]
        [string[]]$ProjectName,

        # Get deployment created before this date
        [System.DateTimeOffset]$Before = [System.DateTimeOffset]::MaxValue,
        
        # Get deployment created after this date
        [System.DateTimeOffset]$After = [System.DateTimeOffset]::MinValue          
    )

    Begin
    {
        $c = New-OctopusConnection
        $list = @()
        $i = 1
    }
    Process
    {
        #Getting EnvironmentIDs and ProjectIDs based on values set on parameters

        Write-Verbose "[$($MyInvocation.MyCommand)] Getting deployments for Projects [$ProjectName] on Environments [$EnvironmentName]"

        $projects = Get-OctopusProject -Name $ProjectName -ResourceOnly

        $environments = Get-OctopusEnvironment -Name $EnvironmentName -ResourceOnly
        
        $deployments = $c. repository.Deployments.FindMany(`            
            {param($dep) if (`
                (($dep.projectid -in $projects.id))`
                 -and (($dep.environmentid -in $environments.id))`
                 -and (($dep.created -ge $After) -and ($dep.created -le $Before)))`
            {$true}})

        Write-Verbose "[$($MyInvocation.MyCommand)] Deployments found: $($deployments.count)"

        foreach ($d in $deployments){

            Write-Progress -Activity "Getting info from deloyment: $($d.id)" -status "$i of $($deployments.count)" -percentComplete ($i / $deployments.count*100)
            Write-Verbose "[$($MyInvocation.MyCommand)] Getting info of deployment: $($d.id)"

            $p = $projects | ?{$_.id -eq $d.projectid}
            $e = $environments | ? {$_.id -eq $d.environmentid}
            $t = $c.repository.Tasks.Get($d.Links.task)
            $r = $c.repository.Releases.Get($d.Links.Release)
            $dp = $c.repository.DeploymentProcesses.Get($r.links.ProjectDeploymentProcessSnapshot)
            $dev = (Invoke-WebRequest -Uri "$env:OctopusURL/api/events?regarding=$($d.Id)" -Method Get -Headers $c.header -Verbose:$false | ConvertFrom-Json).items | ? {$_.category -eq "DeploymentQueued"}
            $rev = (Invoke-WebRequest -Uri "$env:OctopusURL/api/events?regarding=$($r.Id)" -Method Get -Headers $c.header -Verbose:$false | ConvertFrom-Json).items | ? {$_.category -eq "Created"}

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
                            ID = $d.Id
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

            $i++
        }

    }
    End
    {
        If($list.count -eq 0){
            $list = $null
        }
        return $List
    }
}