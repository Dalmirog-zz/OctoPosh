<#
.Synopsis
   Gets information about Octopus Environments
.DESCRIPTION
   Gets information about Octopus Environments
.EXAMPLE
   Get-OctopusEnvironment -name Production
.LINK
   Github project: https://github.com/Dalmirog/Octoposh
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
        [string[]]$Name,
        #When used, the cmdlet will only return the plain Octopus resource, withouth the extra info. This mode is used mostly from inside other cmdlets
        [switch]$ResourceOnly
    )

    Begin
    {
        $c = New-OctopusConnection
        $list = @()
        $i = 1
    }
    Process
    {

        If(!([string]::IsNullOrEmpty($Name))){            
            $environments = $c.repository.Environments.FindMany({param($env) if (($env.name -in $name) -or ($env.name -like $name)) {$true}})
            
            foreach($N in $Name){
                If(($n -notin $environments.name) -and !($environments.name -like $n)){
                
                    Write-Error "Environment not found: $n"
                    #write-host "Environment not found: $n" -ForegroundColor Red
                }
            }

        }

        else{
        
            $environments = $c.repository.Environments.FindAll()
        }

        If ($ResourceOnly){
            $list += $environments
        }
        Else{
            $dashboard = Get-OctopusResource "/api/dashboard/dynamic" -header $c.header

            foreach ($e in $environments){

                Write-Progress -Activity "Getting info from Environment: $($E.name)" -status "$i of $($environments.count)" -percentComplete ($i / $environments.count*100)

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

                $obj = [PSCustomObject]@{
                                EnvironmentName = $e.name
                                Id = $e.id
                                Machines = $m
                                LatestDeployment = $deployments
                                Resource = $e
                                                        
                            }                                    
                $list += $obj

                $i++
            }
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

