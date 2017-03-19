<#
.Synopsis
   Gets information about Octopus Projects
.DESCRIPTION
   Gets information about Octopus Projects
.EXAMPLE
   Get-OctopusProject

   This command gets all the projects of the current Instance
.EXAMPLE
   Get-OctopusProject -name MyAwesomeProject

   Get the project named "MyAwesomeProject"
.EXAMPLE
   Get-OctopusProject -name MyApp*

   Get all the projects whose name starts with the string "MyApp"
.EXAMPLE
   Get-OctopusEnvironment -Name "Production" | Select -ExpandProperty LatestDeployments | Get-OctopusProject

   Gets all the projects that deployed to the environment "Production" at least once
.EXAMPLE
   Get-OctopusProjectGroup -name MyProjects | Get-OctopusProject | Remove-OctopusResource

   Get all the projects inside of the Project Group "MyProjects" and then delete them from the database
.LINK
   WebSite: http://Octoposh.net
   Github project: https://github.com/Dalmirog/Octoposh
   Wiki: https://github.com/Dalmirog/OctoPosh/wiki
   QA and Cmdlet request: https://gitter.im/Dalmirog/OctoPosh#initial
#>
function Get-OctopusProject
{
    [CmdletBinding()]    
    Param
    (
        # Project Name
        [alias("Name")]
        [Parameter(ValueFromPipelineByPropertyName = $true)]
        [ValidateNotNullOrEmpty()]
        [string[]]$ProjectName,

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
        If(!([string]::IsNullOrEmpty($ProjectName))){
            Write-Verbose "[$($MyInvocation.MyCommand)] Filtering projects by name: $ProjectName"             
            $Projects = $c.repository.Projects.FindMany({param($Proj) if (($Proj.name -in $ProjectName) -or ($Proj.name -like $ProjectName)) {$true}})

            foreach($N in $ProjectName){
                If(($n -notin $Projects.name) -and !($Projects.name -like $n)){
                    Write-Error "Project not found: $n"
                    #throw "Project not found: $n"
                }
            }
        }

        else{        
            Write-Verbose "[$($MyInvocation.MyCommand)] Getting all projects"             
            $Projects = $c.repository.projects.FindAll()
        }
        
        Write-Verbose "[$($MyInvocation.MyCommand)] Projects found: $($Projects.count)"                    
        
        If($ResourceOnly -and $Projects){
            Write-Verbose "[$($MyInvocation.MyCommand)] [ResourceOnly] switch is on. Returning raw Octopus resource objects"
            $list += $Projects
        }

        Else{
            $dashboard = Get-OctopusResource "/api/dashboard/dynamic" -header $c.header

            foreach ($p in $Projects){

                Write-Progress -Activity "Getting info from Project: $($p.name)" -status "$i of $($Projects.count)" -percentComplete ($i / $Projects.count*100)

                Write-Verbose "[$($MyInvocation.MyCommand)] Getting info from project $($p.name)"

                $deployments = @()

                $dashboardItem = $dashboard.Items | ?{$p.Id -eq $_.projectid}

                foreach($d in $dashboardItem){
                
                    $t = $c.repository.Tasks.Get($d.links.task)

                    $dev = (Invoke-WebRequest -Uri "$env:OctopusURL/api/events?regarding=$($d.Id)" -Method Get -Headers $c.header -UseBasicParsing -Verbose:$false | ConvertFrom-Json).items | ? {$_.category -eq "DeploymentQueued"}

                    $dep = [PSCustomObject]@{
                            ProjectName = ($dashboard.Projects | ?{$_.id -eq $d.projectId}).name
                            EnvironmentName = ($dashboard.Environments | ?{$_.id -eq $d.EnvironmentId}).name
                            ReleaseVersion = $d.ReleaseVersion
                            State = $d.state
                            CreatedBy = $dev.username
                            StartTime = ($t.StartTime).datetime
                            EndTime = ($t.CompletedTime).datetime
                    }

                    $deployments += $dep
                }
            
                $pg = $c.repository.ProjectGroups.Get($p.projectgroupid)

                $l = $c.repository.Lifecycles.Get($p.LifeCycleId)
            
                $obj = [PSCustomObject]@{
                    Name = $p.name
                    ID = $p.Id
                    ProjectGroupName = $pg.name                
                    LifecycleName = $l.name
                    LatestDeployments = $deployments
                    AutoCreateRelease = $p.AutoCreateRelease
                    Resource = $p                
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
