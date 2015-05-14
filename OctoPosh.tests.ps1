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
        "$testname"
        It "[New-OctopusResource] creates environments"{               

            $env = Get-OctopusResourceModel -Resource Environment                

            $env.Name = $testname
                
            $envobj = New-OctopusResource -Resource $env

            $envobj.name | should be $testname

        }
        It "[New-OctopusResource] creates Project Groups"{

            $Pg = Get-OctopusResourceModel -Resource ProjectGroup
                                                
            $Pg.Name = $testname

            $Pgobj = New-OctopusResource -Resource $Pg

            $Pgobj.name | should be $testname

        }
        It "[New-OctopusResource] creates Projects"{

            $Proj = Get-OctopusResourceModel -Resource Project
                
            $Proj.Name = $testname
            $Proj.ProjectGroupId = (Get-OctopusProjectGroup -Name $TestName).id
            $Proj.LifecycleId = (Get-OctopusLifeCycle)[0].id

            $Projobj = New-OctopusResource -Resource $Proj

            $Projobj.Name | should be $testname
        }
        It "[NEW-OCTOPUSRESOURCE] CREATES LIFECYCLES. UGLY PLACEHOLDER"{

        }
        It "[NEW-OCTOPUSRESOURCE] CREATES MACHINES. UGLY PLACEHOLDER"{

        }
        It "[NEW-OCTOPUSRELEASE] CREATES RELEASES. UGLY PLACEHOLDER"{

        }
        It "[NEW-OCTOPUSDEPLOYMENT] CREATES DEPLOMENTS. UGLY PLACEHOLDER"{

        }
        It "[Get-OctopusEnvironment] gets environments"{
           
            Get-OctopusEnvironment -Name $TestName | select -ExpandProperty EnvironmentNAme | should be $TestName
        }
        It "[Get-OctopusProject] gets projects"{

            Get-OctopusProject -Name $TestName | select -ExpandProperty ProjectName | should be $TestName
                
        }
        It "[Get-OctopusProjectGroup] gets Project Groups"{
            Get-OctopusProjectGroup -Name $TestName | select -ExpandProperty ProjectGroupName | should be $TestName
        }
        It "[Get-OctopusLifecycle] gets Lifecycles"{
                
            Get-OctopusLifeCycle | should not be $null
        }                
        It "[Get-OctopusMachine] gets machines by single name"{
            $Machinename = "OctopusTest02 - TestMachine1"
            Get-OctopusMachine -MachineName $Machinename | select -ExpandProperty Machinename | should be $Machinename
        }
        It "[Get-OctopusMachine] gets machines by name using wildcards"{
            $Machinename = "*OctopusTest*"
            $regex = $Machinename.Replace('*','')
            Get-OctopusMachine -MachineName $Machinename | select -ExpandProperty Machinename | should match ([regex]::Escape($regex))
        }
        It "[Get-OctopusMachine] gets machines by multiple names"{
            $Machinename = "OctopusTest02 - TestMachine1","OctopusTest02 - TestMachine2"            
            Get-OctopusMachine -MachineName $Machinename | select -ExpandProperty Machinename | should be $Machinename
        }        
        It "[Get-OctopusMachine] doesnt get machines by non-existent names "{
            $Machinename = "Charizard"
            Get-OctopusMachine -MachineName $Machinename -ErrorAction SilentlyContinue | should be $null
        }        
        It "[Get-OctopusMachine] gets machines by single environment "{
            $environmentName = "staging"
            Get-OctopusMachine -EnvironmentName $environmentName | select -ExpandProperty EnvironmentName -unique | should be $environmentName
        }        
        It "[Get-OctopusMachine] gets machines by multiple environments "{
            $environmentName = "staging","Production"
            Get-OctopusMachine -EnvironmentName $environmentName | select -ExpandProperty EnvironmentName -Unique | should be $environmentName
        }
        It "[Get-OctopusMachine] gets machines by Environment using wildcards"{
            $EnvironmentName = "Prod*"
            $regex = $EnvironmentName.Replace('*','')
            Get-OctopusMachine -EnvironmentName $EnvironmentName| select -ExpandProperty EnvironmentName | should match ([regex]::Escape($regex))
        }
        It "[Get-OctopusMachine] gets doesnt get machines from non-existent environments"{
            $environmentName = "Lugia","Articuno"
            Get-OctopusMachine -EnvironmentName $environmentName -ErrorAction SilentlyContinue| should be $null
        }
        It "[Get-OctopusMachine] gets machines by single URL"{
            $URL = "https://octopustest02:10937/"
            Get-OctopusMachine -URL $URL | select -ExpandProperty URI -unique | should be $URL
        }
        It "[Get-OctopusMachine] gets machines by multiple URLs"{
            $URL = "https://octopustest02:10936/"
            Get-OctopusMachine -URL $URL | select -ExpandProperty URI -unique | should be $URL
        }
        It "[Get-OctopusMachine] gets machines by URL using wildcards"{
            $URL = "*109*"
            $regex = $URL.Replace('*','')
            Get-OctopusMachine -URL $URL| select -ExpandProperty URI | should match ([regex]::Escape($regex))
        }
        It "[Get-OctopusMachine] gets doesnt get machines from non-existent URL"{
            $URL = "Umbreon"
            Get-OctopusMachine -URL $URL -ErrorAction SilentlyContinue| should be $null
        }
        It "[Get-OctopusMachine] gets machines by communication style"{
            $CommunicationStyle = "Listening"
            Get-OctopusMachine -CommunicationStyle $CommunicationStyle | select -ExpandProperty communicationstyle -unique | should be $CommunicationStyle
        }
        It "[Get-OctopusRelease] gets AN UGLY HARDCODED release"{
            #Get-OctopusRelease -ProjectName TestProject1 | should not be $null
        }
        It "[Get-OctopusDeployment] gets deployments. IT SHOULD GET A SINGLE DEPLOYMENT" {

            #(Get-OctopusDeployment -ProjectName TestProject1) | should not be $null                
        }
        It "[Remove-OctopusResource] deletes environments"{
                
            {Get-OctopusEnvironment -Name $testname | Remove-OctopusResource -Force} | should not Throw               

            Get-OctopusEnvironment -Name $TestName -ErrorAction SilentlyContinue | should be $null
        }        
        It "[Remove-OctopusResource] deletes Projects"{

            {Get-OctopusProject -Name $TestName | Remove-OctopusResource -Force} | should not throw

            Get-OctopusProject -Name $TestName -ErrorAction SilentlyContinue| should be $null

        }
        It "[Remove-OctopusResource] deletes Project Groups"{

            {Get-OctopusProjectGroup -Name $TestName |Remove-OctopusResource -Force} | should not throw

            Get-OctopusProjectGroup -Name $TestName -ErrorAction SilentlyContinue | should be $null

        }
        It "[REMOVE-OCTOPUSRESOURCE] DELETES LIFECYCLES. UGLY PLACEHOLDER"{

        }
        It "[REMOVE-OCTOPUSRESOURCE] DELETES RELEASES. UGLY PLACEHOLDER"{

        }
        It "[REMOVE-OCTOPUSRESOURCE] DELETES DEPLOYMENT. UGLY PLACEHOLDER"{

        }                    
        It "[Get/Set-OctopusConnectionInfo] do their thing" {
            
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
        It "[Get/Set-OctopusSMTPConfig] do their thing"{
            
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
        It "[Get/Set-OctopusMaintenanceMode] do their thing" {

            Set-OctopusMaintenanceMode -On -Force | should be $true

            (Get-OctopusMaintenanceMode).IsInMaintenanceMode | should be $true

            Set-OctopusMaintenanceMode -OFF -Force | should be $true

            (Get-OctopusMaintenanceMode).IsInMaintenanceMode | should be $False

        }        
        It "[Set-OctopusUserAccountStatus] Enables and Disables a user account" {

            $d = Set-OctopusUserAccountStatus -Username "OT\Tester@OT" -status Disabled
            $d.IsActive | should be "False"

            $e = Set-OctopusUserAccountStatus -Username "OT\Tester@OT" -status Enabled
            $e.IsActive | should be "True"
        }                        
        It "[New-OctopusAPIKey] creates an API Key"{

            $api = New-OctopusAPIKey -Purpose "$TestName" -Username 'Ian.Paullin' -password "Michael2" -NoWarning
                
            $api.purpose | should be $TestName

            $api.APIKey | should not be $null

            {$c.repository.Users.RevokeApiKey($api)} | should not throw

        }                    
        It "[Block/Unblock-OctopusRelease] blocks/unblocks AN UGLY HARDCODED release"{

            $release = Get-OctopusRelease -ProjectName TestProject1 | Select -First 1
            
            $release | Block-OctopusRelease -Description $TestName -Force | should be $true

            $release | UnBlock-OctopusRelease -Force | should be $true
        }
        #>
        
}