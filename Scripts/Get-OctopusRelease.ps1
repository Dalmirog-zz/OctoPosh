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
    [CmdletBinding(DefaultParameterSetName='none')]    
    Param
    (
        # Release version number
        [Parameter(ValueFromPipelineByPropertyName = $true,ParameterSetName ="ByVersion")]
        [alias("Version")]
        [string[]]$ReleaseVersion = $null,

        # Project Name. Only one Project can be passed to this parameter at a time
        [Parameter(ValueFromPipelineByPropertyName = $true)]
        [alias("Project")]
        [String]$ProjectName,

        # Get latest X releases. The highest number allowed by this parameter is 30
        [Parameter(ParameterSetName ="Latest")]        
        [int]$Latest,
        
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
        
        $Project = Get-OctopusProject -Name $ProjectName -ResourceOnly

        If($ReleaseVersion -ne $null){                
            foreach ($V in $ReleaseVersion){                
                    
                Write-Verbose "[$($MyInvocation.MyCommand)] Getting release: $V"

                Try{       
                    $r = $c.repository.Projects.GetReleaseByVersion($Project,$v)
                }

                Catch [Octopus.Client.Exceptions.OctopusResourceNotFoundException]{
                    Write-Error "No releases found for project $($Project.name) with the version number $v"
                    $r = $null
                }
                
                If ($r -ne $null){
                    $releases += $r
                }                                
            }
        }

        Else{
            $releases += ($c.repository.Projects.GetReleases($Project)).items
            If($Latest){
                Write-Verbose "[$($MyInvocation.MyCommand)] Getting latest $Latest releases of project $($Project.name)"
                $releases = $releases[0..($Latest -1)]
            }
        }
            
        Write-Verbose "[$($MyInvocation.MyCommand)] Releases found for project $($Project.name): $($releases.count)"     

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
                        ProjectName = $Project.name
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