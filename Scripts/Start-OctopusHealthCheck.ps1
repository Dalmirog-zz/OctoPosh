﻿<#
.Synopsis
   Starts a Health Check task on a specific set environment
.DESCRIPTION
   Starts a Health Check task on a specific set environment
.EXAMPLE
   Start-OctopusHealthCheck -Environment "Staging"

   Starts a health on the environment "Staging"
.EXAMPLE
   Get-OctopusEnvironment -Name "Production" | Start-OctopusHealthCheck -force -Message "Health Check from powershell"

   Starts a health check on the environment "Production"
.LINK
   Github project: https://github.com/Dalmirog/Octoposh
   Wiki: https://github.com/Dalmirog/OctoPosh/wiki
   QA and Cmdlet request: https://gitter.im/Dalmirog/OctoPosh#initial
#>
function Start-OctopusHealthCheck
{
    [CmdletBinding()]
    Param
    (
        # The name of the environment/s on which you wanna start a health check. A task will start for each Environment listed on this parameter
        [Parameter(Mandatory=$true,
                   ValueFromPipelineByPropertyName=$true)]
        [string[]]$EnvironmentName,

        # The message that will show up on the Octopus task. If a value is not passed to this parameter, a default message will be used
        [string]$Message,

        # Forces cmdlet to continue without prompting
        [switch]$Force,

        # Waits until the task is not on states "Queued" or "Executing"
        [switch]$Wait,

        # Timeout for [Wait] parameter in minutes. Default timeout is 2 minutes
        [double]$Timeout = 2
    )

    Begin
    {
        $c = New-OctopusConnection
        $list = @()
    }
    Process
    {
        foreach ($environment in $EnvironmentName){
            
            Write-Verbose "[$($MyInvocation.MyCommand)] Processing environment: $environment"

            $Machines = Get-OctopusMachine -EnvironmentName $Environment -ResourceOnly

            If($Machines -ne $null){
                
                If(!$message){
                    $message = "[API Generated] Check Health on Environment: $Environment"
                }

                If(!($Force)){
                    If (!(Get-UserConfirmation -message "Are you sure you want to start a health check on the environment: $Environment")){
                        Throw 'Canceled by user'
                    }
                }
        
                Write-Verbose "Starting Health check on Environment $environment which contains machines:"

                $Machines.name | %{Write-Verbose $_}

                $environmentId = (Get-OctopusEnvironment -EnvironmentName $environment).Id

                $Task = $c.repository.Tasks.ExecuteHealthCheck($Message,5,5,$environmentId,$Machines.Id)        

                If($wait){

                    $StartTime = Get-Date

                    Do{
                        $CurrentTime = Get-date
                    
                        $task = Get-OctopusTask -ID $task.id
                    
                        Start-Sleep -Seconds 2
                    }Until (($task.state -notin ('Queued','executing')) -or ($CurrentTime -gt $StartTime.AddMinutes($Timeout)))

                    Write-Verbose "Health task on environment $environment finished with status: $($task.state.tostring().toupper())"
                }
                $list += $Task
            }

            else{
                Write-Error 'No machines were found'
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