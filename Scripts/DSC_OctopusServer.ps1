[CmdletBinding()]
param (
    [string]$ConnectionString = "",
    [ValidateSet("Present","Absent")]
    [string]$Ensure,
    [ValidateSet("Started","Stopped")]
    [string]$ServiceState,
    [int]$Port,
    [string]$OctopusAdmin,
    [string]$OctopusPassword,
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
Configuration DSC_OctopusServer
{   

    #Importing custom OctopusDSC resource
    Import-DscResource -ModuleName OctopusDSC

    Node "localhost"
    {
        cOctopusServer "Octopus Server"
        {
            Ensure = $Ensure
            State = $ServiceState

            # Server instance name. Leave it as 'OctopusServer' unless you have more
            # than one instance
            Name = $OctopusInstance

            # The url that Octopus will listen on
            WebListenPrefix = "http://localhost:$port"

            #SqlDbConnectionString = "Server=NY-DB1\NYDB1;Database=OctopusDSC;Integrated Security=SSPI;"
            SqlDbConnectionString = $ConnectionString

            # The admin user to create
            OctopusAdminUsername = $OctopusAdmin
            OctopusAdminPassword = $OctopusPassword

            # optional parameters
            AllowUpgradeCheck = $false
            AllowCollectionOfAnonymousUsageStatistics = $false
            ForceSSL = $false
            ListenPort = 10943            

            # for pre 3.5, valid values are "UsernamePassword"
            # for 3.5 and above, only "Ignore" is valid (this is the default value)
            LegacyWebAuthenticationMode = "Ignore"
        }
    }
}

DSC_Octopusserver -OutputPath $PSScriptRoot\DSC_OctopusServer

Start-DscConfiguration $PSScriptRoot\DSC_OctopusServer -wait -Force