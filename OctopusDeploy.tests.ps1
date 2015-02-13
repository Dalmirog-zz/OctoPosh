import-module "$PSScriptRoot\OctopusDeploy.psm1" -force

Describe "Octopus Module Tests" {

        Context "Lovely Tests"{

            $TestName = new-testname

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

                $api = New-OctopusAPIKey -Purpose "$TestName" -Username Ian.Paullin -password "Michael2"

                $api.purpose | should be $TestName

                $api.APIKey | should not be $null

            }
        }
}