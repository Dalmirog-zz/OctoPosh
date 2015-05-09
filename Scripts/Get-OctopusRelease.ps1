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
   Github project: https://github.com/Dalmirog/OctoPosh
#>
function Get-OctopusRelease{
    [CmdletBinding()]    
    Param
    (
        ## Release version
        [Parameter(ValueFromPipelineByPropertyName = $true)]
        [alias("Version")]
        [string[]]$ReleaseVersion = $null,

        # Project Name
        [Parameter(ValueFromPipelineByPropertyName = $true)]
        [alias("Project")]
        [String[]]$ProjectName,
        
        #When used, the cmdlet will only return the plain Octopus resource, withouth the extra info. This mode is used mostly from inside other cmdlets
        [switch]$ResourceOnly
    )

    Begin
    {
        $c = New-OctopusConnection
        
        $list = @()
        $releases = @()
        
    }
    Process
    {
        $Projects = Get-OctopusProject -Name $ProjectName -ResourceOnly

        foreach ($Project in $Projects){

            If($ReleaseVersion -ne $null){                
                foreach ($V in $ReleaseVersion){                

                    Try{       
                        $r = $c.repository.Projects.GetReleaseByVersion($Project,$v)
                    }

                    Catch [Octopus.Client.Exceptions.OctopusResourceNotFoundException]{
                        write-host "No releases found for project $($Project.name) with the ID $v"
                        $r = $null
                    }                
                }
            }

            Else{
                $r = ($c.repository.Projects.GetReleases($Project)).items
            }
            
            If ($r -ne $null){
                $releases += $r
            }
            
        }

        If($ResourceOnly){
            $list += $releases
        }

        Else{
            Foreach($release in $releases){
        
        $d = $c.repository.Deployments.FindOne({param($dep) if($dep.releaseid -eq $release.Id){$true}})        
        $rev = (Invoke-WebRequest -Uri "$env:OctopusURL/api/events?regarding=$($release.Id)" -Method Get -Headers $c.header | ConvertFrom-Json).items | ? {$_.category -eq "Created"}
        
        $obj = [PSCustomObject]@{
                ProjectName = ($Projects | ?{$_.id -eq $Release.projectID}).name
                ReleaseVersion = $release.Version
                ReleaseNotes = $release.ReleaseNotes
                ReleaseCreationDate = ($release.assembled).datetime
                ReleaseCreatedBy = $rev.Username                
                Resource = $release                
            }            

        $list += $obj       
        }
        }

    }
    End
    {
        return $list
    }
}