<#
.Synopsis
   Gets the currently configured octopus info (Server URI and API Key)
.DESCRIPTION
   This function gets the data of the variables $env:OctopusURI and $env:OctopusAPI that are used by most of the cmdlets of the OctopusDeploy powershell module
.EXAMPLE
   Get-OctopusConnectionInfo
.LINK
   Github project: ghttps://github.com/Dalmirog/OctopusDeploy-Powershell-module
    
#>
function Get-OctopusConnectionInfo
{
    Begin
    {
        
    }
    Process
    {
        $properties = [ordered]@{
            OctopusURI = $env:OctopusURI
            OctopusAPIKey = $env:OctopusAPIKey
        }

        $o =  new-object psobject -Property $properties
    }
    End
    {

        return $o

    }
}