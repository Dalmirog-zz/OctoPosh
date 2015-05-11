<#
.Synopsis
   Get-OctopusMachine returns info about Octopus machines (tentacles)
.DESCRIPTION
   Get-OctopusMachine returns info about Octopus machines (tentacles)
.EXAMPLE
   Get-OctopusMachine -Name "Database_Prod"

   Gets the machine with the name "Database_Prod"
.EXAMPLE
   .EXAMPLE
   Get-OctopusMachine -Name "*_Prod"

   Gets all the machines which name is like "*_Prod"
.LINK
   Github project: https://github.com/Dalmirog/Octoposh
#>
function Get-OctopusMachine
{
    [CmdletBinding(DefaultParameterSetName="Name")]
    Param
    (
        # ASK MONSE
        [Alias("Name")]
        [Parameter(ValueFromPipelineByPropertyName=$true,
                   ParameterSetName = "Name")]
        [string[]]$MachineName,

        # ASK MONSE
        [Alias("Environment")]
        [Parameter(ValueFromPipelineByPropertyName=$true,
                   ParameterSetName = "Environment")]
        [string[]]$EnvironmentName,

        # ASK MONSE
        [Alias("URI")]
        [Parameter(ParameterSetName = "URL")]
        [string[]]$URL,

        # ASK MONSE
        [Alias("Mode","TentacleMode")]
        [ValidateSet("Listening","Polling")]         
        [Parameter(ParameterSetName = "CommunicationStyle")]
        [string]$CommunicationStyle,

        #When used, the cmdlet will only return the plain Octopus resource, withouth the extra info. This mode is used mostly from inside other cmdlets
        [switch]$ResourceOnly
    )

    Begin
    {
        $c = New-OctopusConnection
        $List = @()
    }
    Process
    {
        If(!([string]::IsNullOrEmpty($MachineName)) -or ([string]::IsNullOrEmpty($URL)) -or ([string]::IsNullOrEmpty($CommunicationStyle))){

            If($CommunicationStyle -eq "Polling"){$CommunicationStyle = "TentacleActive"}            
            elseIf($CommunicationStyle -eq "Listening"){$CommunicationStyle = "TentaclePassive"}
            
            $Machines = $c.repository.Machines.FindMany({param($Mach) if (`
                        (($Mach.name -in $MachineName) -or ($Mach.name -like $MachineName))`
                         -or ((($Mach.uri -in $URL) -or ($Mach.uri -like $URL)) -and !([string]::IsNullOrEmpty($mach.uri)))`
                         -or (($Mach.CommunicationStyle -eq $CommunicationStyle))`                         
                          ) {$true}})
            <#
            foreach($n in $MachineName){
                If($n -notin $Machines.name){
                    write-host "Machine not found: $n" -ForegroundColor Red
                }
            }
            #>
        }

        else{        
            $Machines = $c.repository.Machines.FindAll()
        }

        <#
        If($ResourceOnly){
            $list += $Machines
        }#>
        $list += $Machines
    }
    End
    {
        return $List
    }
}