#Generates a random test name that'll be used to name everything on the tests
Function New-TestName {    
    
    $length = 10 #length of random chars
    $characters = 'abcdefghkmnprstuvwxyzABCDEFGHKLMNPRSTUVWXYZ1234567890' #characters to use
    
    # select random characters
    $random = 1..$length | ForEach-Object { Get-Random -Maximum $characters.length }
        
    #Set ofs to "" to avoid having spaces between each char
    $private:ofs=""

    #output prefix (max 10 chars) + 5 random chars
    Return [String]($prefix + $characters[$random])

}

Describe "Octopus Module Tests" {

        $TestName = new-testname

        Write-Output "Test name: $TestName"

        $c = New-OctopusConnection

        It "New-OctopusResource creates environments"{               

            $env = Get-OctopusResourceModel -Resource Environment                

            #Creating environment correctly
            $env.Name = $testname
                
            $envobj = New-OctopusResource -Resource $env

            $envobj.name | should be $testname

        }

        It "Get-OctopusEnvironment gets environments"{
                
            Get-OctopusEnvironment -Name $TestName | should not be $null
        }

        It "Remove-OctopusResource deletes environments"{
                
            {Get-OctopusEnvironment -Name $testname | Remove-OctopusResource -Force} | should not Throw               

            (Get-OctopusEnvironment -Name $TestName) | should be $null
        }
            
        It "New-OctopusResource creates Project Groups"{

            $Pg = Get-OctopusResourceModel -Resource ProjectGroup
                                                
            $Pg.Name = $testname

            $Pgobj = New-OctopusResource -Resource $Pg

            #Should change this for Get-OctopusProjectGroup
            $Pgobj.name | should be $testname

        }

        It "New-OctopusResource creates Projects"{

            $Proj = Get-OctopusResourceModel -Resource Project
                
            $Proj.Name = $testname
            $Proj.ProjectGroupId = ($c.repository.ProjectGroups.FindByName($testname)).id
            $Proj.LifecycleId = "lifecycle-ProjectGroups-1"

            $Projobj = New-OctopusResource -Resource $Proj

            $Projobj.Name | should be $testname

        }

        It "UGLY PLACEHOLDER FOR GET-OCTOPUSPROJECT"{

            Get-OctopusProject -Name $TestName | should not be $null
                
        }

        It "UGLY PLACEHOLDER FOR GET-OCTOPUSPROJECTGROUP"{
                
        }

        It "Remove-OctopusResource deletes Projects"{

            {Get-OctopusProject -Name $TestName | Remove-OctopusResource -Force} | should not throw

            Get-OctopusProject -Name $TestName | should be $null

        }

        It "Remove-OctopusResource deletes Project Groups"{

            #should change this for Get-OctopusProjectGroup
            $pgobj = $c.repository.ProjectGroups.FindByName($TestName)

            {Remove-OctopusResource -Resource $pgobj -Force} | should not throw

            ($c.repository.ProjectGroups.FindByName($TestName)) | should be $null

        } 

        It "UGLY PLACEHOLDER FOR NEW-OCTOPUSDEPLOYMENT"{

        }

        It "Get-OctopusDeployment gets deployments" {

            #I should be creating a deployment or something like that here            

            (Get-OctopusDeployment -ProjectName TestProject1) | should not be $null
                
        }

        It "UGLY PLACEHOLDER FOR REMOVE-OCTOPUSDEPLOYMENT"{

        }
                    
        It "Get/Set-OctopusConnectionInfo do their thing" {
            
            $originalURL = $env:OctopusURL
            $originalAPIKey = $env:OctopusAPIKey

            Set-OctopusConnectionInfo -URL "SomethingURL" -APIKey "SomethingAPIKey"

            $ci = Get-OctopusConnectionInfo
            $ci.OctopusURL | should be "SomethingURL"
            $ci.OctopusAPIKey | should be "SomethingAPIKey"                

            Set-OctopusConnectionInfo -URL $originalURL -APIKey $originalAPIKey

            $ci = Get-OctopusConnectionInfo
            $ci.OctopusURL | should be $originalURL
            $ci.OctopusAPIKey | should be $originalAPIKey
            
        }

        It "Get/Set-OctopusSMTPConfig do their thing"{
            
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

        It "Get/Set-OctopusMaintenanceMode do their thing" {

            Set-OctopusMaintenanceMode -On | should be $true

            (Get-OctopusMaintenanceMode).IsInMaintenanceMode | should be $true

            Set-OctopusMaintenanceMode -OFF | should be $true

            (Get-OctopusMaintenanceMode).IsInMaintenanceMode | should be $False

        }

        <#
        It "Set-OctopusUserAccountStatus Enabled/Disabled" { https://github.com/Dalmirog/OctopusDeploy-Powershell-module/issues/53

            $d = Set-OctopusUserAccountStatus -Username Ian.Paullin -status Disabled
            $d.IsActive | should be "False"

            $e = Set-OctopusUserAccountStatus -Username Ian.Paullin -status Enabled
            $e.IsActive | should be "True"
        }
        #>
        <# https://github.com/Dalmirog/OctopusDeploy-Powershell-module/issues/52z=
        It "New-OctopusAPIKey creates an API Key"{

            $api = New-OctopusAPIKey -Purpose "$TestName" -Username Tester -password "Michael3" -NoWarning
                
            $api.purpose | should be $TestName

            $api.APIKey | should not be $null

            {$c.repository.Users.RevokeApiKey($api)} | should not throw

        }
        #>
            
        <# https://github.com/Dalmirog/OctopusDeploy-Powershell-module/issues/49
        It "Block/Unblock Release"{

            Block-OctopusRelease -ProjectName Powershell -Version 1.1.1 -Description $TestName | should be $true

            UnBlock-OctopusRelease -ProjectName Powershell -Version 1.1.1 | should be $true
        }
            
        
    #>
        
}