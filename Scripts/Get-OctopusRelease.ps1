<#
.Synopsis
   Gets information about Octopus Releases
.DESCRIPTION
   Gets information about Octopus Releases
.EXAMPLE
   Get-OctopusRelease
.EXAMPLE
   Get-OctopusRelease -Project "MyApp.Webapp","MyClientApp.Webapp" -version 1.0.1
.EXAMPLE
   Get-OctopusRelease -Project "*WebApp" -version 1.0.*
.LINK
   Github project: https://github.com/Dalmirog/OctopusDeploy-Powershell-module
#>
function Get-OctopusRelease{
    [CmdletBinding()]    
    Param
    (
        # Environment name
        [Parameter(Position=0)]
        [string[]]$Version = $null,

        # Environment ID
        [String[]]$ProjectName = $null
    )

    Begin
    {
        $c = New-OctopusConnection
        
        #These If statements were written on a saturday at 5am after 5hs of programming. I'm pretty sure they could be better, but right now they work and that's more than enough for tonight
        If($ProjectName -ne $null){
            
            $Projects = $c.repository.Projects.FindMany({param($Proj) if (($Proj.name -in $ProjectName) -or ($Proj.name -like $Projectname)) {$true}})
        }

        else{
        
            $Projects = $c.repository.Projects.FindAll()
        }
        
        If($Version -ne $null){
        
            $releases = $c.repository.Releases.FindMany({param($rel) if ((($rel.Version -eq $Version) -or ($rel.Version -like $Version)) -and ($rel.ProjectID -in $Projects.id)) {$true}})
        
        }

        Else{

            $releases = $c.repository.Releases.FindMany({param($rel) if ($rel.Projectid -in $Projects.id) {$true}})
        }

        $list = @()
        
    }
    Process
    {

        foreach($release in $releases){
        
        $p = $Projects | ?{$_.id -eq $Release.projectID} #to use the name
        $d = $c.repository.Deployments.FindOne({param($dep) if($dep.releaseid -eq $release.Id){$true}})
        $e = $c.repository.Environments.Get($d.Links.Environment)
        $t = $c.repository.tasks.Get($d.Links.task)
        

        $properties = [ordered]@{
                ProjectName = $p.name
                ReleaseVersion = $release.version
                ReleaseCreationDate = $release.assembled
                LastDeployDate = $t.starttime
                LastDeployEnvironment = $e.name
                LastDeployState = $t.state
                LastDeployStartedBy = "TBD"
                Resource = $release                
            }
            
            $list += $obj = New-Object psobject -Property $properties       

        }


    }
    End
    {
        return $list | sort projectname
    }
}