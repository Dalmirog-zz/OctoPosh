<#
.Synopsis
   Short description
.DESCRIPTION
   Long description
.EXAMPLE
   Example of how to use this cmdlet
.EXAMPLE
   Another example of how to use this cmdlet
#>
function Get-OctopusEnvironment
{
    [CmdletBinding()]
    [Alias()]
    Param
    (
        # Environment name
        [Parameter(Position=0)]
        [string[]]$Name

        # Environment ID
        #[String]$EnvironmentId
    )

    Begin
    {
        $c = New-OctopusConnection

        if($Name -ne $null){
            
            $environments = $c.repository.Environments.FindMany({param($env) if (($env.name -in $name) -or ($env.name -like $name)) {$true}})
        }

        else{
        
            $environments = $c.repository.Environments.FindAll()
        }

        $dashboard = Get-OctopusResource "/api/dashboard/dynamic" -header $c.header
        
    }
    Process
    {
        $list = @()
        $deployments = @()

        foreach ($e in $environments){

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

