<#
.Synopsis
   Either Creates a connection with an Octopus server or it returns a hashtable that can be used along invoke-webrequest to hit the REST API (see examples)
.DESCRIPTION
   Either Creates a connection with an Octopus server or it returns a hashtable that can be used along invoke-webrequest to hit the REST API (see examples)
.EXAMPLE
   $c = New-octopusconnection ; $c.repository.environments.findall()
.EXAMPLE
   $header = New-OctopusConnection -RestAPI ; invoke-webrequest -header $header -uri http://Octopus.company.com/api/environments/all -method Get
.LINK
   Github project: https://github.com/Dalmirog/OctopusDeploy-Powershell-module
#>
function New-OctopusConnection
{

    [CmdletBinding()]
    Param
    (
        # When used, the function will return the hashtable that can be passed to the [header] parameter of Invoke-Webrequest
        [switch]$RestAPI
    )


    Begin
    {
        If(($env:OctopusURL -eq $null) -or ($env:OctopusURL -eq "") -or ($env:OctopusAPIKey -eq $null) -or ($env:OctopusAPIKey -eq ""))
        {
            throw "At least one of the following variables does not have a value set: `$env:OctopusURL or `$env:OctopusAPIKey. Use Set-OctopusConnectionInfo to set these values"            
        }

    }
    Process
    {

        If(!($RestAPI)){

            $endpoint = new-object Octopus.Client.OctopusServerEndpoint "$($Env:OctopusURL)","$($env:OctopusAPIKey)"    
            $repository = new-object Octopus.Client.OctopusRepository $endpoint                     

            $properties = [ordered]@{
                endpoint = $endpoint
                repository = $repository
            }

            $output = New-Object psobject -Property $properties

        }

        else{

            $output =  @{ "X-Octopus-ApiKey" = $env:OctopusAPIKey }

        }

    }
    End
    {
        return $output
    }
}