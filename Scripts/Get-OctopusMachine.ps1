<#
.Synopsis
   This cmdlet returns info about Octopus machines (tentacles)
.DESCRIPTION
   This cmdlet returns info about Octopus machines (tentacles)
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

    Gets all the machines registered in "Listening" mode. "Polling" is also a valid value
.LINK
   Github project: https://github.com/Dalmirog/Octoposh
   Advanced Cmdlet Usage: https://github.com/Dalmirog/OctoPosh/wiki/Advanced-Examples
   QA and Cmdlet request: https://gitter.im/Dalmirog/OctoPosh#initial
#>
function Get-OctopusMachine
{
    [CmdletBinding(DefaultParameterSetName='Name')]
    Param
    (
        # Machine name
        [Alias('Name')]
        [Parameter(ValueFromPipelineByPropertyName=$true,
                   ParameterSetName = 'Name')]
        [string[]]$MachineName,

        # Environment name. Use to get all machines inside of an environment
        [Alias('Environment')]
        [Parameter(ValueFromPipelineByPropertyName=$true,
                   ParameterSetName = 'Environment')]
        [string[]]$EnvironmentName,

        # URL of the machine
        [Alias('URI')]
        [Parameter(ParameterSetName = 'URL')]
        [string[]]$URL,

        # Communication style of the machine. Only values accepted are "Listening" and "Polling"
        [Alias('Mode','TentacleMode')]
        [ValidateSet('Listening','Polling')]         
        [Parameter(ParameterSetName = 'CommunicationStyle')]
        [string]$CommunicationStyle,

        # When used the cmdlet will only return the plain Octopus resource object
        [switch]$ResourceOnly
    )

    Begin
    {
        $c = New-OctopusConnection
        $List = @()
        $i = 1
    }
    Process
    {
        If(($PSCmdlet.ParameterSetName -eq 'Name') -and !([string]::IsNullOrEmpty($MachineName))) {
            Write-Verbose "[$($MyInvocation.MyCommand)] Getting Machines by $($PSCmdlet.ParameterSetName): $Machinename "
            $Machines = $c.repository.Machines.FindMany({param($Mach) if ((($Mach.name -in $MachineName) -or ($Mach.name -like $MachineName))) {$true}})

            foreach($n in $MachineName){
                        If(($n -notin $Machines.name) -and !($Machines.name -like $n)){
                            write-error "No Machines found with the name: $n"
                            #write-host "No Machines found with the name: $n" -ForegroundColor Red
                        }
            }
        }

        elseIf($PSCmdlet.ParameterSetName -eq 'CommunicationStyle') {
            Write-Verbose "[$($MyInvocation.MyCommand)] Getting Machines by $($PSCmdlet.ParameterSetName): $CommunicationStyle"
            If($CommunicationStyle -eq 'Polling'){$Style = 'TentacleActive'}            
            elseIf($CommunicationStyle -eq 'Listening'){$Style = 'TentaclePassive'}

            $Machines = $c.repository.Machines.FindMany({param($Mach) if ($Mach.CommunicationStyle -eq $Style){$true}})

            If($Machines -eq $null){
                Write-Error "No Machines found with CommunicationStyle: $($Style)"
                #write-host "No Machines found with CommunicationStyle: $($Style)" -ForegroundColor Red
            }
        }

        elseIf(($PSCmdlet.ParameterSetName -eq 'URL') -and (!([string]::IsNullOrEmpty($URL)))) {
            Write-Verbose "[$($MyInvocation.MyCommand)] Getting Machines by $($PSCmdlet.ParameterSetName): $URL"
            $Machines = $c.repository.Machines.FindMany({param($Mach) if ((($Mach.URI -in $URL) -or ($Mach.URI -like $URL))) {$true}})

            foreach($U in $URL){
                If(($U -notin $Machines.URI) -and !($Machines.URI -like $U)){
                    write-error "No Machines found with the URL: $U"
                    #write-host "No Machines found with the URL: $U" -ForegroundColor Red
                }
            }
        }

        elseIf(($PSCmdlet.ParameterSetName -eq 'Environment') -and !([string]::IsNullOrEmpty($EnvironmentName))) {
            
            Write-Verbose "[$($MyInvocation.MyCommand)] Getting Machines by $($PSCmdlet.ParameterSetName): $EnvironmentName"
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
            Write-Verbose "[$($MyInvocation.MyCommand)] Getting all the Machines"
            $Machines = $c.repository.Machines.FindAll()
        }

        Write-Verbose "[$($MyInvocation.MyCommand)] Machines found: $($Machines.count)"

        If($ResourceOnly){
            $list += $Machines
        }

        else{            
            foreach ($machine in $Machines){                
                Write-Progress -Activity "Getting info from machine: $($machine.name)" -status "$i of $($machines.count)" -percentComplete ($i / $machines.count*100)
                Write-Verbose "[$($MyInvocation.MyCommand)] Getting info of Machine: $($Machine.name)"

                $e = @() 

                If($environments){
                    $e = $environments | ?{$_.id -eq $machine.EnvironmentIds}
                }

                Else{
                    $e = Get-OctopusResource "api/environments/$($machine.EnvironmentIds)" -header $c.header
                }
                
                If($Machine.CommunicationStyle -eq 'TentacleActive'){$Style = 'Polling'}            
                If($Machine.CommunicationStyle -eq 'TentaclePassive'){$Style = 'Listening'}               

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
                
                $i++
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