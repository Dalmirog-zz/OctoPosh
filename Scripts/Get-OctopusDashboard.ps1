<#
.Synopsis
   This cmdlet gets the Octopus dashboard by hitting the `/dashboard/dynamic` Octopus API endpoint. It also gives the user a few filtering options like being able to only see deployments for a certain project, certain environment or on a certain status.
.DESCRIPTION
   This cmdlet gets the Octopus dashboard by hitting the `/dashboard/dynamic` Octopus API endpoint. It also gives the user a few filtering options like being able to only see deployments for a certain project, certain environment or on a certain status.
.EXAMPLE
   Get-OctopusDashboard

   Gets the entire Octopus dashboard
.EXAMPLE
   Get-OctopusDashboard -ProjectName MyWebApp

   Gets the dashboard info for the project MyWebApp
.EXAMPLE
   Get-OctopusDashboard -EnvironmentName Production

   Gets the dashboard info for all the projects that have a release deployed to the "Production" environment.
.EXAMPLE
   Get-OctopusDashboard -DeploymentStatus Success

   Gets all the deployments in "Success" status on the dashboard, regardless the project/environment they belong to.
.EXAMPLE
   Get-OctopusDashboard -EnvironmentName Production -DeploymentStatus Failed

   Gets all the deployments of the dashboard on the "Production" Environment that are in "Failed" status.
.EXAMPLE
   Get-OctopusDashboard -EnvironmentName Production -DeploymentStatus Failed

   Gets all the deployments of the dashboard on the "Production" Environment that are in "Failed" status.
.LINK
   WebSite: http://Octoposh.net
   Github project: https://github.com/Dalmirog/Octoposh
   Wiki: https://github.com/Dalmirog/OctoPosh/wiki
   QA and Cmdlet request: https://gitter.im/Dalmirog/OctoPosh#initial
#>
function Get-OctopusDashboard
{
    [CmdletBinding()]
    [Alias("Get-Dashboard")]    
    Param
    (
        # Name of the Project to filter for.        
        [string]$ProjectName,

        # Name of the Environment to filter for.        
        [string]$EnvironmentName,

        # Deploymeny status to filter for. Accepted values are "Success","Failed","Executing","Canceled"        
        [ValidateSet('Success','Failed','Executing','Canceled')] 
        [string]$DeploymentStatus
    )

    Begin
    {
        $c = New-OctopusConnection
        $i = 1
        $rawDashboard = Get-Octopusresource -uri "/api/dashboard/dynamic"
    }
    Process
    {

        If($ProjectName){
            $Project = $rawDashboard.projects | ?{$_.name -eq $ProjectName}
            If(!$Project){
                Throw "Project not found: $ProjectName"
            }    
        }
        
        If($EnvironmentName){
            $Environment = $rawDashboard.Environments| ?{$_.name -eq $EnvironmentNAme}
            If(!$Environment){
                Throw "Environment not found: $EnvironmentName"
            }    
        }

        $List = @()
        foreach($deployment in $rawDashboard.items){
            
            If(($Project) -and ($deployment.projectID -ne $Project.id)){
                #A project was passed as parameter, but this deployment doesn't belong to it
                Continue
            }
            
            If(($Environment) -and ($deployment.EnvironmentID -ne $Environment.id)){
                #An environment was passed as parameter, but this deployment doesn't belong to it
                Continue
            }

            If(($DeploymentStatus) -and ($deployment.state -ne $DeploymentStatus)){
                #A deployment status was passed as parameter, but this deployment isn't in that state
                Continue
            }

            #Write-Progress -Activity "Massaging dashboard info for deployment: $($deployment.id)" -status "$i of $($rawDashboard.items.count)" -percentComplete ([Double]$i / $rawDashboard.items.count*100)            
            [datetime]$queuetime = $deployment.queueTime
            $startdate = '{0:yyyy/mm/dd HH:mm:ss}' -f $queuetime

            If($deployment.CompletedTime){
                [datetime]$CompletedTime = $deployment.CompletedTime
                $duration = New-TimeSpan –Start $queuetime –End $CompletedTime
                $EndDate = '{0:yyyy/mm/dd HH:mm:ss}' -f $CompletedTime
            }
            else{
                #If the deployment is running or waiting for manual intervention, the "CompletedTime" property wont have a value
                #In that case still calculate the duration against the current datetime, and set "EndDate" to $null.
                $duration = New-TimeSpan –Start $queuetime –End (get-date)
                $EndDate = $null
            }            
            
            $obj = [PSCustomObject]@{
                ProjectName = If($Project){$Project.name} else{($rawDashboard.projects | ?{$_.Id -eq $deployment.projectID}).name}   
                EnvironmentName = If($Environment){$Environment.name} else{($rawDashboard.Environments| ?{$_.Id -eq $deployment.EnvironmentID}).name}
                ReleaseVersion = [version]$deployment.ReleaseVersion
                DeploymentStatus = $deployment.state
                StartDate = $startdate
                EndDate = $EndDate
                IsCompleted = $deployment.IsCompleted                
                HasPendingInterruptions = $deployment.HasPendingInterruptions
                HasWarningsOrErrors = $deployment.HasWarningsOrErrors
                Duration = [string]::Format("{0:D2}:{1:D2}:{2:D2}", $duration.Hours,$duration.Minutes,$duration.Seconds)
            }

            $list += $obj

            $i++
        }
    }
    End
    {
        return $List | Sort-Object -Property ProjectName,ReleaseVersion
    }
}