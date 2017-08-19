[CmdletBinding()]
param (
    [string]$ConnectionString = "",
    [ValidateSet("Present","Absent")]
    [string]$Ensure,
    [ValidateSet("Started","Stopped")]
    [string]$ServiceState,
    [int]$Port,
    [string]$OctopusAdmin,
    [string]$OctopusPassword
    )

if((Get-DscResource -Module OctopusDSC) -eq $null){
    Write-Output "DSC Resource [OctopusDSC] not installed. Downloading from PSGallery..."
    Install-Module -Name OctopusDSC -Force
    }

Configuration DSC_OctopusServer
{   

    Import-DscResource -Module OctopusDSC

    Node "localhost"
    {
        cOctopusServer DSC_OctopusServer
        {
            Ensure = $Ensure
            State = $ServiceState

            # Server instance name. Leave it as 'OctopusServer' unless you have more
            # than one instance
            Name = "OctopusServer"

            # The url that Octopus will listen on
            WebListenPrefix = "http://localhost:$port"

            #SqlDbConnectionString = "Server=NY-DB1\NYDB1;Database=OctopusDSC;Integrated Security=SSPI;"
            SqlDbConnectionString = $ConnectionString

            # The admin user to create
            OctopusAdminUsername = $OctopusAdmin
            OctopusAdminPassword = $OctopusPassword

            # optional parameters
            AllowUpgradeCheck = $true
            AllowCollectionOfAnonymousUsageStatistics = $true
            ForceSSL = $false
            ListenPort = 10943            

            # for pre 3.5, valid values are "UsernamePassword"
            # for 3.5 and above, only "Ignore" is valid (this is the default value)
            LegacyWebAuthenticationMode = "Ignore"
        }
    }
}

DSC_Octopusserver -OutputPath $PSScriptRoot\DSC_OctopusServer

Start-DscConfiguration . $PSScriptRoot\DSC_OctopusServer -wait -Force