<#
.Synopsis
   Sets the current Octopus connection info (URL and API Key). 
   
   Highly recommended to call this function from $profile to avoid having to re-configure this on every session.
.DESCRIPTION
   Sets the current Octopus connection info (URL and API Key). 
   
   Highly recommended to call this function from $profile to avoid having to re-configure this on every session.
.EXAMPLE
   Set-OctopusConnectionInfo -URL "http://MyOctopus.AwesomeCompany.com" -API "API-7CH6XN0HHOU7DDEEUGKUFUR1K"

   Set connection info with a specific API Key for an Octopus instance
.LINK
   Github project: ghttps://github.com/Dalmirog/Octoposh
#>
function Set-OctopusConnectionInfo
{
    [CmdletBinding()]
    Param
    (
        # Octopus URL
        [Parameter(Mandatory=$true)]
        [string]$URL,

        # Octopus API Key
        [Parameter(Mandatory=$true)]
        [string]$APIKey
    )
    Begin
    {
        
    }
    Process
    {
        $env:OctopusURL = $URL
        $env:OctopusAPIKey = $APIKey        
    }
    End
    {
        Get-octopusConnectionInfo
    }
}