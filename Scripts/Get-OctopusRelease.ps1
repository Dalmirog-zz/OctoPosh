<#
.Synopsis
   Gets information about Octopus Releases
.DESCRIPTION
   Gets information about Octopus Releases
.EXAMPLE
   Get-OctopusRelease -ProjectName "MyProject"

   Gets information about all the realeases created for the project "MyProject"
.EXAMPLE
   Get-OctopusRelease -ProjectName "MyProject" -version 1.0.1

   Gets information about realease 1.0.1 of the project "MyProject"
.EXAMPLE
   Get-OctopusRelease -ProjectName "MyWebProject","MySQLProject" -version 1.0.1, 1.0.2

   Gets information about realeases 1.0.1 and 1.0.2 of the projects "MyWebProject" and "MySQLProject"
.LINK
   Github project: https://github.com/Dalmirog/OctopusDeploy-Powershell-module
#>
function Get-OctopusRelease{
    [CmdletBinding()]    
    Param
    (
        # Release version
        [Parameter(ValueFromPipelineByPropertyName = $true)]
        [alias("Version")]
        [string[]]$ReleaseVersion = $null,

        # Project Name
        [Parameter(ValueFromPipelineByPropertyName = $true)]
        [alias("Project")]
        [String[]]$ProjectName = $null
    )

    Begin
    {
        $c = New-OctopusConnection
        
        $list = @()
        $releases = @()
        
    }
    Process
    {
        If($ProjectName -ne $null){
            
            $Projects = $c.repository.Projects.FindMany({param($Proj) if (($Proj.name -in $ProjectName)) {$true}})
        }

        else{
        
            $Projects = $c.repository.Projects.FindAll()
        }

        If($Projects -eq $null){
            throw "No project/s found with the name/s: $Projectname"
        }

        foreach ($Project in $Projects){
            foreach ($V in $ReleaseVersion){
                If($ReleaseVersion-ne $null){

                    Try{       
                        $r = $c.repository.Projects.GetReleaseByVersion($Project,$v)
                    }

                    Catch [Octopus.Client.Exceptions.OctopusResourceNotFoundException]{
                        write-host "No releases found for project $($Project.name) with the ID $v"
                        $r = $null
                    }                
                }

                Else{
                    $r = ($c.repository.Projects.GetReleases($Project)).items
                }
            
                If ($r -ne $null){
                    $releases += $r
                }
            }
        }

        
        Foreach($release in $releases){
        
        $d = $c.repository.Deployments.FindOne({param($dep) if($dep.releaseid -eq $release.Id){$true}})        
        $rev = (Invoke-WebRequest -Uri "$env:OctopusURL/api/events?regarding=$($release.Id)" -Method Get -Headers $c.header | ConvertFrom-Json).items | ? {$_.category -eq "Created"}
        
        $obj = [PSCustomObject]@{
                ProjectName = ($Projects | ?{$_.id -eq $Release.projectID}).name
                ReleaseVersion = $release.version
                ReleaseCreationDate = ($release.assembled).datetime
                ReleaseCreatedBy = $rev.Username                
                Resource = $release                
            }            

        $list += $obj       
        }

    }
    End
    {
        return $list
    }
}