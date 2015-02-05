<#
.Synopsis
   Short description
.DESCRIPTION
   Long description
.EXAMPLE
   Example of how to use this cmdlet
.EXAMPLE
   Another example of how to use this cmdlet
#>
function New-OctopusConnection
{
    Begin
    {
        If(($env:OctopusURI -eq $null) -or ($env:OctopusURI -eq "") -or ($env:OctopusAPIKey -eq $null) -or ($env:OctopusAPIKey -eq ""))
        {
            throw "At least one of the following variables does not have a value set: `$env:OctopusURI or `$env:OctopusAPIKey. Use Set-OctopusConnectionInfo to set these values"            
        }

    }
    Process
    {
        $endpoint = new-object Octopus.Client.OctopusServerEndpoint "$($Env:OctopusURI)","$($env:OctopusAPIKey)"    
        $repository = new-object Octopus.Client.OctopusRepository $endpoint                     

        $properties = [ordered]@{
            endpoint = $endpoint
            repository = $repository
        }

        $output = New-Object psobject -Property $properties

    }
    End
    {
        return $output
        $output.repository

    }
}