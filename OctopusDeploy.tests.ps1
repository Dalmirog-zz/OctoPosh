import-module "$PSScriptRoot\OctopusDeploy.psm1" -force

Describe "Octopus Module Tests" {

        $TestName = new-testname

        Context "Lovely Tests"{            

            It "Get/Set-OctopusConnectionInfo" {
            
                Set-OctopusConnectionInfo -URL "SomethingURL" -APIKey "SomethingAPIKey"

                $ci = Get-OctopusConnectionInfo
                $ci.OctopusURL | should be "SomethingURL"
                $ci.OctopusAPIKey | should be "SomethingAPIKey"                

                Set-OctopusConnectionInfo -URL "http://localhost" -APIKey "API-7CH6XN0HHOU7DDEEUGKUFUR1K"

                $ci = Get-OctopusConnectionInfo
                $ci.OctopusURL | should be "http://localhost"
                $ci.OctopusAPIKey | should be "API-7CH6XN0HHOU7DDEEUGKUFUR1K"
            
            }

            It "Set-OctopusMaintenanceMode ON/OFF" {

                Set-OctopusMaintenanceMode -On | should be $true
                Set-OctopusMaintenanceMode -OFF | should be $true

            }

            It "Set-OctopusUserAccountStatus Enabled/Disabled" {

                $d = Set-OctopusUserAccountStatus -Username Ian.Paullin -status Disabled
                $d.IsActive | should be "False"

                $e = Set-OctopusUserAccountStatus -Username Ian.Paullin -status Enabled
                $e.IsActive | should be "True"
            }

            It "New-OctopusAPIKey creates an API Key"{

                #changing WarningPreference to avoid getting the warning of
                #new-octopusAPIKey so tests look nicer
                $WarningPreference = "silentlycontinue"

                $api = New-OctopusAPIKey -Purpose "$TestName" -Username Ian.Paullin -password "Michael2"

                #Setting it back to normal
                $WarningPreference = "continue"

                $api.purpose | should be $TestName

                $api.APIKey | should not be $null

                {$c.repository.Users.RevokeApiKey($api)} | should not throw

            }
        }

        Context "Create/Delete resources"{

            $c = New-OctopusConnection

            It "Environment"{                

                $env = Get-OctopusResourceModel -Resource Environment

                $env.Name = $testname

                $envobj = New-OctopusResource -Resource $env

                $envobj.Name | should be $testname
                $envobj.Id | should not be $null 

                Remove-OctopusResource -Resource $envobj

                $c.repository.Environments.FindByName($testname) | should be $null

                }

            It "Project & Project Group"{

                $Pg = Get-OctopusResourceModel -Resource ProjectGroup
                $Proj = Get-OctopusResourceModel -Resource Project

                $Pg.Name = $testname

                $Pgobj = New-OctopusResource -Resource $Pg

                $Pgobj.Name | should be $testname
                $Pgobj.Id | should not be $null

                $Proj.Name = $testname
                $Proj.ProjectGroupId = $Pgobj.Id
                $Proj.LifecycleId = "lifecycle-ProjectGroups-1"

                $Projobj = New-OctopusResource -Resource $Proj

                $Projobj.Name | should be $testname
                $Projobj.Id | should not be $null
                
                Remove-OctopusResource -Resource $Projobj
                Remove-OctopusResource -Resource $Pgobj
               

                $c.repository.ProjectGroups.FindByName($testname) | should be $null
                $c.repository.Projects.FindByName($testname) | should be $null

            }
        
        }
}