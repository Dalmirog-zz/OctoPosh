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
        [Parameter(Mandatory=$true,ParameterSetName ='Project/Version')]
        $ProjectName,

        # Release Version number. You can only block one release at a time using thie parameter
        [Parameter(Mandatory=$true,ParameterSetName ='Project/Version')]
        $Version,

        # Accepts [Octopus.Client.Module.ReleaseResource] objects. Accepts pipeline value from Get-OctopusRelease
        [Parameter(Mandatory=$true,ParameterSetName ='Resource',ValueFromPipelineByPropertyName=$true)]
        [Octopus.Client.Model.ReleaseResource[]]$Resource,

        # Description of the blocking
        [ValidateNotNullOrEmpty()]
        [string]$Description=$(throw "[Description] is mandatory, please provide a value to this parameter.")

    )

    Begin
    {
        $c = New-OctopusConnection

        $Defect = @{Description = $Description} | ConvertTo-Json
    }
    Process
    {

        Try{

            If($Resource){

                Invoke-WebRequest $env:OctopusURL/$($Resource.links.ReportDefect) -Method Post -Headers $c.header -Body $Defect

            }
        
            If($Version){
        
                $p = $c.repository.Projects.FindOne({param($proj) if($proj.name -eq $ProjectName ){$true}})

                $r = $c.repository.Releases.FindMany({param($Rel) if (($Rel.version -eq $Version) -and ($Rel.projectID -eq $p.ID)) {$true}})
            
                Invoke-WebRequest $env:OctopusURL/$($r.links.ReportDefect) -Method Post -Headers $c.header -Body $Defect
        
            }
        }
        Catch{
        
            $_
        
        }

        

    }
    End
    {
    }
}