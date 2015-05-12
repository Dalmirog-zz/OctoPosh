<#
.Synopsis
   Starts a Health Check task on a specific set of machines, or on all the machines on an environment
.DESCRIPTION
   Starts a Health Check task on a specific set of machines, or on all the machines on an environment
.EXAMPLE
   Start-OctopusHealthCheck -Machinename "MyProject_Database"

   Starts a health check on the machine "MyProject_Database"
.EXAMPLE
   Start-OctopusHealthCheck -Environment "Staging"

   Starts a health check on all the machines inside of the environment "Staging"
.EXAMPLE
   Get-OctopusEnvironment -Name "Production" | Start-OctopusHealthCheck

   Starts a health check on all the machines inside of the environment "Production"
.EXAMPLE
   Get-OctopusMachine -Name "MyDb_Prod" | Start-OctopusHealthCheck

   Starts a health check on the machine "MyDb_Prod"
.LINK
   Github project: https://github.com/Dalmirog/Octoposh
#>
function Start-OctopusHealthCheck
{
    [CmdletBinding()]
    Param
    (
        # The name of the Machine on which you wanna start a health check. A task will start for each Environment listed on this parameter.        
        [Parameter(Mandatory=$true,
                   ValueFromPipelineByPropertyName=$true,
                   ParameterSetNAme="Machine")]
        [string[]]$MachineName,

        # The name of the environment/s on which you wanna start a health check. A task will start for each Environment listed on this parameter
        [Parameter(Mandatory=$true,
                   ValueFromPipelineByPropertyName=$true,
                   ParameterSetNAme="Environment")]
        [string[]]$EnvironmentName,

        #The message that will show up on the Octopus task
        [string]$Message,

        # Forces resource delete.
        [switch]$Force
    )

    Begin
    {
        $c = New-OctopusConnection
    }
    Process
    {
        If($PSCmdlet.ParameterSetName -eq "Environment"){            
            $targets = Get-OctopusMachine -EnvironmentName $EnvironmentName -ResourceOnly
        }
        
        else{$targets = Get-OctopusMachine -Name $machinename -ResourceOnly}
        
        If($targets -eq $null){
            Throw "No machines where found "
        }      

        If(!($Force)){
            If (!(Get-UserConfirmation -message "Are you sure you want to start a health check on the $($PSCmdlet.ParameterSetName): $($target.name)")){
                Throw "Canceled by user"
            }
        }

        foreach ($target in $targets){

            If(!$message){
                $message = "[API Generated] Check Health on $($PSCmdlet.ParameterSetName): $($target.name)"
            }

            Write-Verbose "Starting Health check on $($PSCmdlet.ParameterSetName): $($target.name)"

            If($PSCmdlet.ParameterSetName -eq "Environment"){
               $task = $c.repository.Tasks.ExecuteHealthCheck($Message,5,$target.id)
            }

            Else{
               #INVESTIGAR $Target.Links.connection
               $task = $c.repository.Tasks.ExecuteHealthCheck($Message,5,($target | select -ExpandProperty Environmentids | select -First 1),$target.Id)
            }
        }
    }
    End
    {
        return $task
    }
}