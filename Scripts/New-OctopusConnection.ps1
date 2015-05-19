<#
.Synopsis
   Either Creates a connection with an Octopus server or it returns a hashtable that can be used along invoke-webrequest to hit the REST API (see examples)
.DESCRIPTION
   Either Creates a connection with an Octopus server or it returns a hashtable that can be used along invoke-webrequest to hit the REST API (see examples)
.EXAMPLE
   $c = New-octopusconnection ; $c.repository.environments.findall()
.EXAMPLE
   $c = New-OctopusConnection ; invoke-webrequest -header $c.header -uri http://Octopus.company.com/api/environments/all -method Get
.LINK
   Github project: https://github.com/Dalmirog/Octoposh
   Advanced Cmdlet Usage: https://github.com/Dalmirog/OctoPosh/wiki/Advanced-Examples
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