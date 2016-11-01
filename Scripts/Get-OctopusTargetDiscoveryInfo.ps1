<#
.Synopsis
   Hits the \discover API endpoint to get Tentacle info. This works just like the "Discover" button on the Web UI, which Octopus uses to automagically discover the Tentacle's Thumbprint. This cmdlet will work similarly, but instead it will just return the machine info, including its thumbprint.
.DESCRIPTION
   Hits the \discover API endpoint to get Tentacle info. This works just like the "Discover" button on the Web UI, which Octopus uses to automagically discover the Tentacle's Thumbprint. This cmdlet will work similarly, but instead it will just return the machine info, including its thumbprint.
.EXAMPLE
   Get-OctopusMachineDiscoveryInfo -ComputerName WebServer1 -Port 10933 -CommunicationStyle Listening

   Gets the discovery info of the Tentacle instance Listening in port 10933 in computer WebServer1
.EXAMPLE
   Get-OctopusMachineDiscoveryInfo -IP 10.0.0.1 -Port 10933 -CommunicationStyle Listening

   Gets the discovery info of the Tentacle instance Listening in port 10933 in the computer with the IP 10.0.0.1
#>
function Get-OctopusTargetDiscoveryInfo
{
    [CmdletBinding()]
    [Alias("Get-OctopusMachineDiscoveryInfo")]    
    Param
    (
        # Name or IP of the computer with the Tentacle instance.
        [Parameter(Mandatory=$true,Position=0)]
        [Alias("IP")]    
        $ComputerName,

        # Port where the Tentacle instance is listening on.
        [int]
        $Port,

        # Communication style of the Tentacle. Values accepted are "Listening" and "Polling"
        [Alias('Mode','TentacleMode')]
        [ValidateSet('Listening','Polling')]        
        [string]$CommunicationStyle
    )

    Begin
    {
        $c = New-OctopusConnection
        If($CommunicationStyle -eq 'Polling'){
            $Type = "TentacleActive"
        }
        else{$Type = "TentaclePassive"}
    }
    Process
    {
        $url = "$env:OctopusURL/api/machines/discover?host=$ComputerName&port=$Port&type=$Type"

        Write-Verbose "[$($MyInvocation.MyCommand)] [GET] $url"

        Try{

            $discover = ((Invoke-WebRequest $url -Headers $c.header -UseBasicParsing).content | ConvertFrom-Json).endpoint
        }
        Catch{
            Write-Error "Unable to make a connection to $url. Error was: `n $_"
            break
        }
    }
    End
    {
        return $discover
    }
}
