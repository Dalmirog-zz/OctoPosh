<#
.Synopsis
   Updates the variable set of a release. Only one project an one release can be updated at a time.
.DESCRIPTION
   Updates the variable set of a release. Only one project an one release can be updated at a time.
.EXAMPLE
   Update-OctopusReleaseVariableSet -ProjectName MyProject -ReleaseVersion 1.0.0 

   Update the variable set of a Project's Release
.LINK
   Github project: https://github.com/Dalmirog/Octoposh
   Advanced Cmdlet Usage: https://github.com/Dalmirog/OctoPosh/wiki/Advanced-Examples
   QA and Cmdlet request: https://gitter.im/Dalmirog/OctoPosh#initial
#>
function Update-OctopusReleaseVariableSet
{
    [CmdletBinding()]
    Param
    (
        # Project Name
        [Parameter(Mandatory=$true,
                   ValueFromPipelineByPropertyName=$true,
                   Position=0)]
        $ProjectName,

        # Release Version
        [Alias("Version")]
        [Parameter(Mandatory=$true,
                   ValueFromPipelineByPropertyName=$true,
                   Position=0)]
        [string]$ReleaseVersion
    )

    Begin
    {
        $c = New-OctopusConnection
    }
    Process
    {
        Write-Verbose "[$($MyInvocation.MyCommand)] Getting Release [$ReleaseVersion] of Project [$ProjectName]" 

        $release = Get-OctopusRelease -ProjectName $ProjectName -ReleaseVersion $ReleaseVersion -ResourceOnly

        If($release){
           Try{    
                Write-Verbose "[$($MyInvocation.MyCommand)] Updating variable set of Release $ReleaseVersion of Project $ProjectName" 
        
                $r = Invoke-WebRequest -Uri ("$env:OctopusURL" + "$($release.Links.self)/snapshot-variables") -Method Post -Headers $c.header
            }
            Catch{       
                write-error $_        
            }
        }        
    }
    End
    {
        Write-Verbose "[$($MyInvocation.MyCommand)] HTTP request to update variables for release $($Release.version) of project $($p.name) returned code $($r.statuscode)"
        if($r.statuscode -eq 200){
            Return $True
        }
        Else{
            Return $false
        }
    }
}