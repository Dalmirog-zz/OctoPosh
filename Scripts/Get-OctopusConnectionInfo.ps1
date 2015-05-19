<#
.Synopsis
   This function gets the data of the variables $env:OctopusURI and $env:OctopusAPI that are used by all the cmdlets of the Octoposh module
.DESCRIPTION
   This function gets the data of the variables $env:OctopusURI and $env:OctopusAPI that are used by all the cmdlets of the Octoposh module
.EXAMPLE
   Get-OctopusConnectionInfo

   Get the current connection info. Its the same as getting the values of $env:OctopusURL and $Env:OctopusAPIKey
.LINK
   Github project: ghttps://github.com/Dalmirog/Octoposh
#>
function Get-OctopusConnectionInfo
{
    Begin
    {
        
    }
    Process
    {
        $properties = [ordered]@{
            OctopusURL = $env:OctopusURL
            OctopusAPIKey = $env:OctopusAPIKey
        }

        $o =  new-object psobject -Property $properties
    }
    End
    {

        return $o

    }
}