<#
.Synopsis
   If $env:OctopusURI or $env:OctopusAPIKey are $null or empty, this function throw an error. Otherwise It'll return $true
.DESCRIPTION
   If $env:OctopusURI or $env:OctopusAPIKey are $null or empty, this function throw an error. Otherwise It'll return $true
.EXAMPLE
   Test-OctopusConnectionInfo
.LINK
   Github project: ghttps://github.com/Dalmirog/OctopusDeploy-Powershell-module.
#>
function Test-OctopusConnectionInfo
{
    Begin
    {
        
    }
    Process
    {
        If($env:OctopusURI -eq $null -or $env:OctopusURI -eq "" -or $env:OctopusAPIKey -eq $null -or $env:OctopusAPIKey -eq "")
        {
            throw "At least one of the following variables do not have a value set: `$env:OctopusURI or `$env:OctopusAPIKey. Use Set-OctopusConnectionInfo to set values on these variables"            
        }
        else{
            return $true
        }
    }
    End
    {
        
    }
}