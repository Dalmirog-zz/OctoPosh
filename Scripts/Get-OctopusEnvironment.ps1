<#
.Synopsis
   Gets information about Octopus Environments
.DESCRIPTION
   Gets information about Octopus Environments
.EXAMPLE
   Get-OctopusEnvironment -name Production

   Get info about the environment "Production"
.EXAMPLE
   Get-OctopusEnvironment -name "Dev*"

   Get info about all the environments whose name starts with "Dev"
.LINK
   Github project: https://github.com/Dalmirog/Octoposh
   Advanced Cmdlet Usage: https://github.com/Dalmirog/OctoPosh/wiki/Advanced-Examples
   QA and Cmdlet request: https://gitter.im/Dalmirog/OctoPosh#initial
#>
function Get-OctopusEnvironment
{
    [CmdletBinding()]
    [Alias()]
    Param
    (
        # Environment name        
        [alias("Name")]
        [Parameter(ValueFromPipelineByPropertyName = $true, Position=0)]
        [string[]]$EnvironmentName,
        
        # When used the cmdlet will only return the plain Octopus resource object
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

        If(!([string]::IsNullOrEmpty($EnvironmentName))){            
            Write-Verbose "[$($MyInvocation.MyCommand)] Getting environments by name: $EnvironmentName" 
            $environments = $c.repository.Environments.FindMany({param($env) if (($env.name -in $EnvironmentName) -or ($env.name -like $EnvironmentName)) {$true}})
            
            foreach($N in $EnvironmentName){
                If(($n -notin $environments.name) -and !($environments.name -like $n)){
                
                    Write-Error "Environment not found: $n"
                    #write-host "Environment not found: $n" -ForegroundColor Red
                }
            }

        }

        else{
            Write-Verbose "[$($MyInvocation.MyCommand)] Getting all Environments" 
            $environments = $c.repository.Environments.FindAll()
        }

        Write-Verbose "[$($MyInvocation.MyCommand)] Environments found: $($environments.count)" 

        If ($ResourceOnly){
            Write-Verbose "[$($MyInvocation.MyCommand)] [ResourceOnly] switch is on. Returning raw Octopus resource objects"
            $list += $environments
        }
        Else{
            $dashboard = Get-OctopusResource "/api/dashboard/dynamic" -header $c.header

            foreach ($e in $environments){

                Write-Progress -Activity "Getting info from Environment: $($E.name)" -status "$i of $($environments.count)" -percentComplete ($i / $environments.count*100)
                Write-Verbose "[$($MyInvocation.MyCommand)] Getting info from environment $($e.name)"

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

