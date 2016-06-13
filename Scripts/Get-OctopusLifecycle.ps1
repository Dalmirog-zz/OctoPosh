<#
.Synopsis
   Gets information about Octopus Lifecycles
.DESCRIPTION
   Gets information about Octopus Lifecycles
.EXAMPLE
   Get-OctopusLifecycle

   Get all the Lifecycles of the current Instance
.EXAMPLE
   Get-OctopusLifecycle -name MyLifecycle

   Get the Lifecycle named "MyLifecycle"
.EXAMPLE
   Get-OctopusProject -name MyProject | Get-OctopusLifecycle

   Get the Lifecycle of the project called "MyProject"
.EXAMPLE
   Get-OctopusProjectGroup -name "MyProjectGroup" | Get-OctopusProject | Get-OctopusLifecycle

   This command gets the Lifecycles of all the projects inside the project group called "MyProjectGroup"
.LINK
   Github project: https://github.com/Dalmirog/Octoposh
   Wiki: https://github.com/Dalmirog/OctoPosh/wiki
   QA and Cmdlet request: https://gitter.im/Dalmirog/OctoPosh#initial
#>
function Get-OctopusLifeCycle
{
    [CmdletBinding()]    
    Param
    (
        # Lifecycle Name
        [alias("Name")]
        [Parameter(ValueFromPipelineByPropertyName = $true)]
        [string[]]$LifeCycleName,

        # When used the cmdlet will only return the plain Octopus resource object
        [switch]$ResourceOnly
    )

    Begin
    {
        $c = New-OctopusConnection
        $list = @()
        $i = 1
    }
    Process
    {
        #Getting Lifecycles        
        If(!([string]::IsNullOrEmpty($LifeCycleName))){            
            Write-Verbose "[$($MyInvocation.MyCommand)] Getting Lifecycles with name/s: [$LifeCycleName]"
            $Lifecycles = $c.repository.Lifecycles.FindMany({param($lc) if (($lc.name -in $LifeCycleName)) {$true}})
        }

        else{
            Write-Verbose "[$($MyInvocation.MyCommand)] Getting all the Lifecycles"
            $Lifecycles = $c.repository.Lifecycles.FindAll()
        }        

        Write-Verbose "[$($MyInvocation.MyCommand)] Lifecycles found: $($Lifecycles.count)"

        If($ResourceOnly){
            $list += $Lifecycles
        }
        else{
            #Getting info by Lifecycle
            foreach ($l in $Lifecycles){            
            
                Write-Progress -Activity "Getting info from lifecycle: $($l.name)" -status "$i of $($Lifecycles.count)" -percentComplete ($i / $Lifecycles.count*100)
                Write-Verbose "[$($MyInvocation.MyCommand)] Getting info from Lifecycle: $($l.name)"

                $obj = [PSCustomObject]@{
                    Name = $l.name
                    Id = $l.Id
                    ReleaseRetentionPolicy = $l.ReleaseRetentionPolicy
                    TentacleRetentionPolicy = $l.TentacleRetentionPolicy
                    Phases = $l.Phases
                    LastModifiedOn = $l.LastModifiedOn
                    LastModifiedBy = $l.LastModifiedBy
                    Resource = $l           
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