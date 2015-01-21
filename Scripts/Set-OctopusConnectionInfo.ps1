<#
.Synopsis
   Sets the current Octopus connection info (URL and API Key). Highly recommended to call this function on $profile to avoid having to re-configure this.
.DESCRIPTION
   Sets the current Octopus connection info (URL and API Key). Highly recommended to call this function on $profile to avoid having to re-configure this.
.EXAMPLE
   Set-OctopusConnectionInfo -URL "http://MyOctopus.AwesomeCompany.com" -API "API-7CH6XN0HHOU7DDEEUGKUFUR1K"
.LINK
   Github project: ghttps://github.com/Dalmirog/OctopusDeploy-Powershell-module    
#>
function Set-OctopusConnectionInfo
{
    [CmdletBinding()]
    Param
    (
        # Octopus URL
        [Parameter(Mandatory=$true)]
        $URL,

        # Octopus API Key
        [Parameter(Mandatory=$true)]
        $APIKey
    )
    Begin
    {
        
    }
    Process
    {
        $env:OctopusURI = $URL
        $env:OctopusAPIKey = $APIKey        
    }
    End
    {
        Get-octopusConnectionInfo
    }
}