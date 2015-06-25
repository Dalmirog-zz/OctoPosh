<#
.Synopsis
   Unblocks an Octopus Release from being deployed to another environment
.Synopsis
   Unblocks an Octopus Release from being deployed to another environment
.EXAMPLE
   Unblock-OctopusRelease -Projectname "MyProduct.WebApp" -Version 1.0.0

   UnBlocks release 1.0.0 of the project "MyProduct.WebApp"
.EXAMPLE
   Get-OctopusRelease -Project "MyProduct.Webapp" -version 1.0.1 | UnBlock-OctopusRelease
   
   Unblocks the release 1.0.1 of the project "MyProduct.Webapp"
.LINK
   Github project: https://github.com/Dalmirog/Octoposh
   Wiki: https://github.com/Dalmirog/OctoPosh/wiki
   QA and Cmdlet request: https://gitter.im/Dalmirog/OctoPosh#initial
#>
function Unblock-OctopusRelease
{
    [CmdletBinding()]
    Param
    (
        # Project Name
        [Parameter(Mandatory=$true, ValueFromPipelineByPropertyName = $true)]
        [string]$ProjectName,

        # Release Version number
        [Parameter(Mandatory=$true, ValueFromPipelineByPropertyName = $true)]
        [string]$ReleaseVersion,

        # Forces cmdlet to continue without prompting
        [switch]$Force
    )

    Begin
    {
        $c = New-OctopusConnection
    }
    Process
    {

        If(!($Force)){
            If (!(Get-UserConfirmation -message "Are you sure you want to unblock release $ReleaseVersion on project $ProjectName ?")){
                Throw "Canceled by user"
            }
        }
    
		Write-Verbose "[$($MyInvocation.MyCommand)] Looking for project: $ProjectName"		
		$p = Get-OctopusProject -ProjectName $ProjectName -ErrorAction Stop -ResourceOnly		
		Write-Verbose "[$($MyInvocation.MyCommand)] Project found: $ProjectName"
		
		Write-Verbose "[$($MyInvocation.MyCommand)] Looking for release: $ReleaseVersion"

        $release = $c.repository.Projects.GetReleaseByVersion($p, $ReleaseVersion)		

        If($release -eq $null){
            Throw "Release $ReleaseVersion not found for project $ProjectName"
        }
		
		Write-Verbose "[$($MyInvocation.MyCommand)] Release found: $($Release.version)"

        Try{
            Write-Verbose "[$($MyInvocation.MyCommand)] Blocking release $($Release.version)"            
            $r = Invoke-WebRequest $env:OctopusURL/$($release.links.ResolveDefect) -Method Post -Headers $c.header -UseBasicParsing -Verbose:$false
        }

        Catch{
            write-error $_        
        }

    }
    End
    {
        Write-Verbose "[$($MyInvocation.MyCommand)] HTTP request to block release $($Release.version) of project $($p.name) returned code $($r.statuscode)"
        if($r.statuscode -eq 200){        
            Return $True
        }
        Else{
            Return $false
        }

    }
}