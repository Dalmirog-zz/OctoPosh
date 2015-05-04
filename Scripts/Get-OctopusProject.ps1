<#
.Synopsis
   Gets information about Octopus Projects
.DESCRIPTION
   Gets information about Octopus Projects
.EXAMPLE
   Get-OctopusProject

   This command gets all the projects of the current Instance
.EXAMPLE
   Get-OctopusProject -name MyProject

   This command gets the project named "MyProject"
.EXAMPLE
   Get-OctopusProject -name MyApp*

   This command gets all the projects whose name start with the string MyApp
.EXAMPLE
   Get-OctopusEnvironment -Name "Production" | Select -ExpandProperty LatestDeployments | Get-OctopusProject

   This command gets all the projects that deployed to the environment "Production" at least once
.EXAMPLE
   Get-OctopusProjectGroup -name MyProjects | Get-OctopusProject | Remove-OctopusResource

   This command gets all the projects inside of the Project Group "MyProjects" and then deletes them from the database
.LINK
   Github project: https://github.com/Dalmirog/Octoposh
#>
function Get-OctopusProject
{
    [CmdletBinding()]    
    Param
    (
        #Project name
        [alias("ProjectName")]
        [Parameter(ValueFromPipelineByPropertyName = $true)]
        [string[]]$Name
    )

    Begin
    {
        $c = New-OctopusConnection
        $list = @()
    }
    Process
    {

        #Getting Projects        
        If(!([string]::IsNullOrEmpty($Name))){
            
            $Projects = $c.repository.Projects.FindMany({param($Proj) if (($Proj.name -in $name) -or ($Proj.name -like $name)) {$true}})
        }

        else{
        
            $Projects = $c.repository.projects.FindAll()
        }        

        #Getting Dashboard info
        $dashboard = Get-OctopusResource "/api/dashboard/dynamic" -header $c.header

        #Getting info by project
        foreach ($p in $Projects){

            $deployments = @()

            $dashboardItem = $dashboard.Items | ?{$p.Id -eq $_.projectid}

            foreach($d in $dashboardItem){
                
                $t = $c.repository.Tasks.Get($d.links.task)

                $dev = (Invoke-WebRequest -Uri "$env:OctopusURL/api/events?regarding=$($d.Id)" -Method Get -Headers $c.header | ConvertFrom-Json).items | ? {$_.category -eq "DeploymentQueued"}

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
                ProjectName = $p.name
                ID = $p.Id
                ProjectGroupName = $pg.name                
                LifecycleName = $l.name
                LatestDeployments = $deployments
                AutoCreateRelease = $p.AutoCreateRelease
                Resource = $p                
            }
            
            $list += $obj

        }       


    }
    End
    {
        $list
    }
}