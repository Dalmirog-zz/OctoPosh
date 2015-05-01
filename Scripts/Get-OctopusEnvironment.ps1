<#
.Synopsis
   Gets information about Octopus Environments
.DESCRIPTION
   Gets information about Octopus Environments
.EXAMPLE
   Get-OctopusEnvironment -name Production
.LINK
   Github project: https://github.com/Dalmirog/OctopusDeploy-Powershell-module
#>
function Get-OctopusEnvironment
{
    [CmdletBinding()]
    [Alias()]
    Param
    (
        # Environment name        
        [alias("EnvironmentName")]
        [Parameter(ValueFromPipelineByPropertyName = $true, Position=0)]
        [string[]]$Name
    )

    Begin
    {
        $c = New-OctopusConnection
        $list = @()
    }
    Process
    {

        If(!([string]::IsNullOrEmpty($Name))){
            
            $environments = $c.repository.Environments.FindMany({param($env) if (($env.name -in $name) -or ($env.name -like $name)) {$true}})
        }

        else{
        
            $environments = $c.repository.Environments.FindAll()
        }

        $dashboard = Get-OctopusResource "/api/dashboard/dynamic" -header $c.header
        
        foreach ($e in $environments){

            $deployments = @()

            $m = $c.repository.Machines.FindMany({param($ma) if ($e.id -in $ma.EnvironmentIds){$true}})

            $dashboardItem = $dashboard.Items | ?{$e.Id -eq $_.EnvironmentId}

            foreach($d in $dashboardItem){
                
                $t = $c.repository.Tasks.Get($d.links.task)

                $dev = (Invoke-WebRequest -Uri "$env:OctopusURL/api/events?regarding=$($d.Id)" -Method Get -Headers $c.header | ConvertFrom-Json).items | ? {$_.category -eq "DeploymentQueued"}

                $obj = [PSCustomObject]@{
                        ProjectName = ($dashboard.Projects | ?{$_.id -eq $d.projectId}).name
                        EnvironmentName = ($dashboard.Environments | ?{$_.id -eq $d.EnvironmentId}).name
                        ReleaseVersion = $d.ReleaseVersion
                        State = $d.state
                        CreatedBy = $dev.username
                        StartTime = ($t.StartTime).datetime
                        EndTime = ($t.CompletedTime).datetime

                        }

                $deployments += $obj
            }                        

            #Creating output object
            $obj = [PSCustomObject]@{
                            EnvironmentName = $e.name
                            Machines = $m
                            LatestDeployment = $deployments
                            Resource = $e
                                                        
                        }                                    
            $list += $obj

        }

    }
    End
    {
        return $list
    }
}

