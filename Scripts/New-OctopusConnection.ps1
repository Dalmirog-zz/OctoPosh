<#
.Synopsis
   Creates an endpoint to connect to Octopus
.DESCRIPTION
   Creates an endpoint to connect to Octopus
.EXAMPLE
   $c = New-octopusconnection ; $c.repository.environments.findall()

   Get all the environments on the Octopus instance using New-OctopusConnection and the Octopus.client
.EXAMPLE
   $c = New-OctopusConnection ; invoke-webrequest -header $c.header -uri http://Octopus.company.com/api/environments/all -method Get

   Use the [Header] Member of the Object returned by New-OctopusConnection as a header to call the REST API using Invoke-WebRequest 
.LINK
   Github project: https://github.com/Dalmirog/Octoposh
   Wiki: https://github.com/Dalmirog/OctoPosh/wiki
   QA and Cmdlet request: https://gitter.im/Dalmirog/OctoPosh#initial
#>
function New-OctopusConnection
{

    Begin
    {
        If(([string]::IsNullOrEmpty($env:OctopusURL)) -or ([string]::IsNullOrEmpty($env:OctopusAPIKey)))
        {
            throw "At least one of the following variables does not have a value set: `$env:OctopusURL or `$env:OctopusAPIKey. Use Set-OctopusConnectionInfo to set these values"            
        }
    }
    Process
    {
        $endpoint = new-object Octopus.Client.OctopusServerEndpoint "$($Env:OctopusURL)","$($env:OctopusAPIKey)"    
        $repository = new-object Octopus.Client.OctopusRepository $endpoint                     

        $properties = [ordered]@{
            endpoint = $endpoint
            repository = $repository
            header = @{ "X-Octopus-ApiKey" = $env:OctopusAPIKey }
        }

        $output = New-Object psobject -Property $properties        
    }
    End
    {
        return $output
    }
}