<#
.Synopsis
   Blocks an Octopus Release from being deployed to another environment
.DESCRIPTION
   Long description
.EXAMPLE
   Block-OctopusRelease -Projectname "MyProduct.WebApp" -Version 1.0.0
.EXAMPLE
   Get-OctopusRelease -Project "MyProduct.Webapp" -version 1.0.1 | Block-OctopusRelease -Reason "Because of reasons"
.LINK
   Github project: https://github.com/Dalmirog/OctopusDeploy-Powershell-module
#>
function Block-OctopusRelease
{
    [CmdletBinding()]
    Param
    (

        # Project Name of the release. You can only block one release at a time using thie parameter
        [Parameter(Mandatory=$true)]
        $ProjectName,

        # Release Version number. You can only block one release at a time using thie parameter
        [Parameter(Mandatory=$true)]
        $ReleaseVersion,

        # Description of the blocking
        [ValidateNotNullOrEmpty()]
        [parameter(Mandatory=$true)]
        [string]$Description, #=$(throw "[Description] is mandatory, please provide a value to this parameter.")

        # Forces action.
        [switch]$Force

    )

    Begin
    {
        $c = New-OctopusConnection

        $Defect = @{Description = $Description} | ConvertTo-Json
    }
    Process
    {

        If(!($Force)){
            If (!(Get-UserConfirmation -message "Are you sure you want to block release $ReleaseVersion on project $ProjectName ?")){
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
            $response = Invoke-WebRequest $env:OctopusURL/$($r.links.ReportDefect) -Method Post -Headers $c.header -Body $Defect -UseBasicParsing
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