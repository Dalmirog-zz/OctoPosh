[CmdletBinding()]
param (
    [string]$OctopusInstance
    )

#Copying custom OctopusDSC build to modulePath
<#
if((Get-DscResource -Module OctopusDSC) -eq $null){
        
    $modulepath = $env:PSModulePath.Split(';')[1]

    Write-Output "DSC Resource [OctopusDSC] not installed. Copying custom fork from repo dir to [$modulepath]"

    if(!(Test-Path $modulepath)){
        New-Item $modulepath -ItemType Directory
    }

    Copy-Item -Path $PSScriptRoot\OctopusDSC -Destination $modulepath -Recurse -Force -Verbose
}
#>
Configuration DSC_OctopusServerUsernamePasswordAuth
{   

    #Importing custom OctopusDSC resource
    Import-DscResource -ModuleName OctopusDSC

    Node "localhost"
    {
        cOctopusServerUsernamePasswordAuthentication "Enable Username/Password Auth"
        {
            InstanceName = $OctopusInstance
            Enabled = $true
        }
    }
}

DSC_OctopusServerUsernamePasswordAuth -OutputPath $PSScriptRoot\DSC_OctopusServerUsernamePasswordAuth

Start-DscConfiguration $PSScriptRoot\DSC_OctopusServerUsernamePasswordAuth -wait -Force -Verbose