<#
.Synopsis
   Gets information about Octopus Lifecycles
.DESCRIPTION
   Gets information about Octopus Lifecycles
.EXAMPLE
   Get-OctopusLifecycle

   This command gets all the Lifecycles of the current Instance
.EXAMPLE
   Get-OctopusLifecycle -name MyLifecycle

   This command gets the Lifecycle named "MyLifecycle"
.EXAMPLE
   Get-OctopusProject -name MyProject | Get-OctopusLifecycle

   This command gets the Lifecycle of the project called "MyProject"
.EXAMPLE
   Get-OctopusProjectGroup -name "MyProjectGroup" | Get-OctopusProject | Get-OctopusLifecycle

   This command gets the Lifecycles of all the projects inside the project group called "MyProjectGroup"
.LINK
   Github project: https://github.com/Dalmirog/Octoposh
#>
function Get-OctopusLifeCycle
{
    [CmdletBinding()]    
    Param
    (
        #Lifecycle Name
        [alias("LifecycleName")]
        [Parameter(ValueFromPipelineByPropertyName = $true)]
        [string[]]$Name
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
        If(!([string]::IsNullOrEmpty($Name))){            
            $Lifecycles = $c.repository.Lifecycles.FindMany({param($lc) if (($lc.name -in $name)) {$true}})
        }

        else{
        
            $Lifecycles = $c.repository.Lifecycles.FindAll()
        }        

        #Getting info by Lifecycle
        foreach ($l in $Lifecycles){            
            
            Write-Progress -Activity "Getting info from lifecycle: $($l.name)" -status "$i of $($Lifecycles.count)" -percentComplete ($i / $Lifecycles.count*100)

            $obj = [PSCustomObject]@{
                LifecycleName = $l.name
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
    End
    {
        If($list.count -eq 0){
            $list = $null
        }
        return $List
    }
}