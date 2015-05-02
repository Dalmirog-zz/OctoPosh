<#
.Synopsis
   Unblocks an Octopus Release from being deployed to another environment
.Synopsis
   Unblocks an Octopus Release from being deployed to another environment
.EXAMPLE
   Unblock-OctopusRelease -Projectname "MyProduct.WebApp" -Version 1.0.0
.EXAMPLE
   Get-OctopusRelease -Project "MyProduct.Webapp" -version 1.0.1 | UnBlock-OctopusRelease
.LINK
   Github project: https://github.com/Dalmirog/OctopusDeploy-Powershell-module
#>
function Unblock-OctopusRelease
{
    [CmdletBinding()]
    Param
    (

        # Project Name of the release. You can only Unblock one release at a time using thie parameter
        [Parameter(Mandatory=$true)]
        $ProjectName,

        # Release Version. You can only Unblock one release at a time using thie parameter
        [Parameter(Mandatory=$true)]
        $ReleaseVersion,

        # Forces action.
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
    
        $p = $c.repository.Projects.FindOne({param($proj) if($proj.name -eq $ProjectName ){$true}})

        If($p -eq $null){
            Throw "Project not found: $projectname"
        }

        $r = $c.repository.Projects.GetReleaseByVersion($p,$ReleaseVersion)

        If($r -eq $null){
            Throw "Release $ReleaseVersion not found for project $ProjectName"
        }

        Try{
            $response = Invoke-WebRequest $env:OctopusURL/$($r.links.ResolveDefect) -Method Post -Headers $c.header -UseBasicParsing    
        }

        Catch{
            write-error $_        
        }

    }
    End
    {

        if($response.statuscode -eq 200){        
            Return $True
        }
        Else{
            Return $false
        }

    }
}