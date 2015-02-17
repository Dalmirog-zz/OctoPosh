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
        [Parameter(Mandatory=$true,ParameterSetName ='Project/Version')]
        $ProjectName,

        # Release Version. You can only Unblock one release at a time using thie parameter
        [Parameter(Mandatory=$true,ParameterSetName ='Project/Version')]
        $Version,

        # Accepts [Octopus.Client.Module.ReleaseResource] objects. Accepts pipeline value from Get-OctopusRelease
        [Parameter(Mandatory=$true,ParameterSetName ='Resource',ValueFromPipelineByPropertyName=$true)]
        [Octopus.Client.Model.ReleaseResource[]]$Resource
    )

    Begin
    {
        $c = New-OctopusConnection
    }
    Process
    {
        Try{
            if($Resource){

                $response = Invoke-WebRequest $env:OctopusURL/$($Resource.links.ResolveDefect) -Method Post -Headers $c.header

            }
        
            If($Version){
        
                $p = $c.repository.Projects.FindOne({param($proj) if($proj.name -eq $ProjectName ){$true}})

                $r = $c.repository.Releases.FindOne({param($Rel) if(($Rel.version -eq $Version) -and ($Rel.projectID -eq $p.ID)) {$true}})

                $response = Invoke-WebRequest $env:OctopusURL/$($r.links.ResolveDefect) -Method Post -Headers $c.header
        
            }
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