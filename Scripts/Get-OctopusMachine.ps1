<#
.Synopsis
   Get-OctopusMachine returns info about Octopus machines (tentacles)
.DESCRIPTION
   Get-OctopusMachine returns info about Octopus machines (tentacles)
.EXAMPLE
   Get-OctopusMachine -Name "Database_Prod"

   Gets the machine with the name "Database_Prod"
.EXAMPLE
    Get-OctopusMachine -Name "*_Prod"

    Gets all the machines which name is like "*_Prod"
.EXAMPLE
    Get-OctopusMachine -EnvironmentName "Staging","UAT"

    Gets all the machines on the environments "Staging","UAT"
.EXAMPLE
    Get-OctopusMachine -URL "*:10933"

    Gets all the machines with the string "*:10933" at the end of the URL
.EXAMPLE
    Get-OctopusMachine -Mode Listening

    Gets all the machines registered in "Listening" mode. "Polling" mode is also available
.LINK
   Github project: https://github.com/Dalmirog/Octoposh
#>
function Get-OctopusMachine
{
    [CmdletBinding(DefaultParameterSetName="Name")]
    Param
    (
        # ASK MONSE. Make sure to say you cant use 2 wildcarded values
        [Alias("Name")]
        [Parameter(ValueFromPipelineByPropertyName=$true,
                   ParameterSetName = "Name")]
        [string[]]$MachineName,

        # ASK MONSE. Make sure to say you cant use 2 wildcarded values
        [Alias("Environment")]
        [Parameter(ValueFromPipelineByPropertyName=$true,
                   ParameterSetName = "Environment")]
        [string[]]$EnvironmentName,

        # ASK MONSE. Make sure to say you cant use 2 wildcarded values
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
        If(($PSCmdlet.ParameterSetName -eq "Name") -and (!([string]::IsNullOrEmpty($MachineName)))) {
            $Machines = $c.repository.Machines.FindMany({param($Mach) if ((($Mach.name -in $MachineName) -or ($Mach.name -like $MachineName))) {$true}})

            foreach($n in $MachineName){
                        If(($n -notin $Machines.name) -and !($Machines.name -like $n)){
                            write-error "No Machines found with the name: $n"
                            #write-host "No Machines found with the name: $n" -ForegroundColor Red
                        }
            }
        }

        elseIf($PSCmdlet.ParameterSetName -eq "CommunicationStyle") {

            If($CommunicationStyle -eq "Polling"){$Style = "TentacleActive"}            
            elseIf($CommunicationStyle -eq "Listening"){$Style = "TentaclePassive"}

            $Machines = $c.repository.Machines.FindMany({param($Mach) if ($Mach.CommunicationStyle -eq $Style){$true}})

            If($Machines -eq $null){
                Write-Error "No Machines found with CommunicationStyle: $($Style)"
                #write-host "No Machines found with CommunicationStyle: $($Style)" -ForegroundColor Red
            }
        }

        elseIf(($PSCmdlet.ParameterSetName -eq "URL") -and (!([string]::IsNullOrEmpty($URL)))) {
            $Machines = $c.repository.Machines.FindMany({param($Mach) if ((($Mach.URI -in $URL) -or ($Mach.URI -like $URL))) {$true}})

            foreach($U in $URL){
                If(($U -notin $Machines.URI) -and !($Machines.URI -like $URL)){
                    write-error "No Machines found with the URL: $U"
                    #write-host "No Machines found with the URL: $U" -ForegroundColor Red
                }
            }
        }

        elseIf(($PSCmdlet.ParameterSetName -eq "Environment") -and !([string]::IsNullOrEmpty($EnvironmentName))) {

            $machines = @()

            $environments = Get-OctopusEnvironment $EnvironmentName -ResourceOnly

            Foreach($env in $environments){

                $envmachines = $c.repository.Environments.GetMachines($env)

                If($envmachines){
                    $machines += $envmachines            
                }

                Else{
                    Write-Error "No machines were found on Environment: $($env.name)"
                    #Write-host "No machines were found on Environment: $($env.name)" -ForegroundColor Red
                }        
            }    
        }

        else{        
            $Machines = $c.repository.Machines.FindAll()
        }

        If($ResourceOnly){
            $list += $Machines
        }

        else{            
            foreach ($machine in $Machines){        
                $e = @() 

                If($environments){
                    $e = $environments | ?{$_.id -eq $machine.EnvironmentIds}
                }

                Else{
                    $e = Get-OctopusResource "api/environments/$($machine.EnvironmentIds)" -header $c.header
                }
                
                If($Machine.CommunicationStyle -eq "TentacleActive"){$Style = "Polling"}            
                If($Machine.CommunicationStyle -eq "TentaclePassive"){$Style = "Listening"}               

                $obj = [PSCustomObject]@{
                    MachineName = $machine.Name
                    MachineID = $machine.Id
                    Thumbprint = $machine.Thumbprint
                    URI = $machine.uri
                    IsDisabled = $machine.IsDisabled
                    EnvironmentName = $e.name
                    Roles = $machine.Roles
                    Squid = $machine.Squid
                    CommunicationStyle = $Style
                    Status = $machine.Status
                    StatusSummary = $machine.StatusSummary
                    LastModifiedOn = $machine.LastModifiedOn
                    LastModifiedBy = $machine.LastModifiedBy
                    Resource = $machine
                }

                $list += $obj

            }
        }
    }
    End
    {
        If($list.count -eq 0){
            $list = $null
        }
        return $List
    }
}