import-module "$PSScriptRoot\OctopusDeploy.psm1" -force

Describe "Octopus Module Tests" {

        $TestName = new-testname

        $c = New-OctopusConnection

        Context "Create/Delete resources"{

            It "Environments"{                

                $env = Get-OctopusResourceModel -Resource Environment

                #Creating environment without required properties
                {New-OctopusResource -Resource $env} | should Throw

                #Creating environment correctly
                $env.Name = $testname
                $envobj = New-OctopusResource -Resource $env

                $envobj.Name | should be $testname
                $envobj.Id | should not be $null 

                #Deleting Environment
                {Remove-OctopusResource -Resource $envobj -Force} | should not Throw

                ##should be changed for Get-OctopusEnvironments
                $c.repository.Environments.FindByName($testname) | should be $null

                }

            It "Projects & Project Groups"{

                $Pg = Get-OctopusResourceModel -Resource ProjectGroup
                $Proj = Get-OctopusResourceModel -Resource Project

                #Creating Project and ProjectGroup without required properties
                {New-OctopusResource -Resource $Pg} | should Throw
                {New-OctopusResource -Resource $Proj} | should Throw

                #Creating Project and ProjectGroup properly
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
                
                #Deleting Project and ProjectGroup
                Remove-OctopusResource -Resource $Projobj -Force
                Start-Sleep -Seconds 2
                Remove-OctopusResource -Resource $Pgobj -Force               

                $c.repository.ProjectGroups.FindByName($testname) | should be $null
                $c.repository.Projects.FindByName($testname) | should be $null

            }
        
        }

        Context "System administration Tests"{  

            It "Get/Set-OctopusConnectionInfo" {
            
                Set-OctopusConnectionInfo -URL "SomethingURL" -APIKey "SomethingAPIKey"

                $ci = Get-OctopusConnectionInfo
                $ci.OctopusURL | should be "SomethingURL"
                $ci.OctopusAPIKey | should be "SomethingAPIKey"                

                Set-OctopusConnectionInfo -URL "http://localhost:81" -APIKey "API-YHMOPNVMRLFXJBV4EQWWFKXAWLQ"

                $ci = Get-OctopusConnectionInfo
                $ci.OctopusURL | should be "http://localhost:81"
                $ci.OctopusAPIKey | should be "API-YHMOPNVMRLFXJBV4EQWWFKXAWLQ"
            
            }

            It "Get/Set-OctopusSMTPConfig"{
            
                $port = Get-Random
                
                Set-OctopusSMTPConfig -SMTPHost "$TestName" `
                -Port $port -SendEmailFrom "dalmiro@company.com" | should be $true

                $SMTPConfig = Get-OctopusSMTPConfig

                $SMTPConfig.SMTPHost | Should be $TestName
                $SMTPConfig.SMTPPort | should be $port

                Set-OctopusSMTPConfig -SMTPHost "Localhost" `
                -Port 25 -SendEmailFrom "Octopus@company.com" | should be $true

                $SMTPConfig = Get-OctopusSMTPConfig

                $SMTPConfig.SMTPHost | Should be "Localhost"
                $SMTPConfig.SMTPPort | should be 25

            
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

                $api = New-OctopusAPIKey -Purpose "$TestName" -Username Ian.Paullin -password "Michael2"
                
                $api.purpose | should be $TestName

                $api.APIKey | should not be $null

                {$c.repository.Users.RevokeApiKey($api)} | should not throw

            }

            It "Blocks/Unblocks Release"{

                Block-OctopusRelease -ProjectName Powershell -Version 1.1.1 -Description $TestName | should be $true

                UnBlock-OctopusRelease -ProjectName Powershell -Version 1.1.1 | should be $true


            }
        }

        
}