<#
.Synopsis
   Gets information about Octopus Releases
.DESCRIPTION
   Gets information about Octopus Releases
.EXAMPLE
   Get-OctopusRelease -ProjectName "MyProject"

   Get all the realeases of the project "MyProject"
.EXAMPLE
   Get-OctopusRelease -ProjectName "MyProject" -version 1.0.1,1.0.2

   Get the release realeases 1.0.1 & 1.0.2 of the project "MyProject"
.LINK
   Github project: https://github.com/Dalmirog/Octoposh
   Advanced Cmdlet Usage: https://github.com/Dalmirog/OctoPosh/wiki/Advanced-Examples
   QA and Cmdlet request: https://gitter.im/Dalmirog/OctoPosh#initial
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
        [String[]]$ProjectName,
        
        # When used the cmdlet will only return the plain Octopus resource object
        [switch]$ResourceOnly
    )

    Begin
    {
        $c = New-OctopusConnection        
        $list = @()
        $releases = @()
        $i++        
    }
    Process
    {

		Write-Verbose "[$($MyInvocation.MyCommand)] Getting releases [$ReleaseVersion] of project [$ProjectName]"
        $Projects = Get-OctopusProject -Name $ProjectName -ResourceOnly

        foreach ($Project in $Projects){
            
            If($ReleaseVersion -ne $null){                
                foreach ($V in $ReleaseVersion){                
                    
                    Write-Verbose "[$($MyInvocation.MyCommand)] Getting release: $V"

                    Try{       
                        $r = $c.repository.Projects.GetReleaseByVersion($Project,$v)
                    }

                    Catch [Octopus.Client.Exceptions.OctopusResourceNotFoundException]{
                        write-host "No releases found for project $($Project.name) with the version number $v"
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
            
            Write-Verbose "[$($MyInvocation.MyCommand)] Releases found: $($releases.count)"
            
        }

        If($ResourceOnly){
            $list += $releases
        }

        Else{
            Foreach($release in $releases){

                Write-Verbose "[$($MyInvocation.MyCommand)] Getting info from release: $($release.version)"

                Write-Progress -Activity "Getting info from release: $($release.id)" -status "$i of $($releases.count)" -percentComplete ($i / $releases.count*100)                
        
                #$d = $c.repository.Deployments.FindOne({param($dep) if($dep.releaseid -eq $release.Id){$true}})
                       
                $rev = (Invoke-WebRequest -Uri "$env:OctopusURL/api/events?regarding=$($release.Id)" -Method Get -Headers $c.header -Verbose:$false| ConvertFrom-Json).items | ? {$_.category -eq "Created"}
        
                $obj = [PSCustomObject]@{
                        ProjectName = ($Projects | ?{$_.id -eq $Release.projectID}).name
                        ReleaseVersion = $release.Version
                        ReleaseNotes = $release.ReleaseNotes
                        CreationDate = ($release.assembled).datetime
                        CreatedBy = $rev.Username
                        LastModifiedOn = ($release.LastModifiedOn).datetime
                        LastModifiedBy = $release.LastModifiedBy                
                        Resource = $release                
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