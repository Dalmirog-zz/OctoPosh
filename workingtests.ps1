#Generates a random test name that'll be used to name everything on the tests
Function New-TestName {    
    
    $length = 10 #length of random chars
    $characters = 'abcdefghkmnprstuvwxyzABCDEFGHKLMNPRSTUVWXYZ1234567890' #characters to use
    
    # select random characters
    $random = 1..$length | ForEach-Object { Get-Random -Maximum $characters.length }
        
    #Set ofs to "" to avoid having spaces between each char
    $private:ofs=''

    #output prefix (max 10 chars) + 5 random chars
    Return [String]($prefix + $characters[$random])

}

Describe 'Octopus Module Tests' {

    $TestName = new-testname

    $c = New-OctopusConnection
    
It '[New-OctopusResource] creates environments'{               

        $env = Get-OctopusResourceModel -Resource Environment                

        $env.Name = $testname
                
        $envobj = New-OctopusResource -Resource $env

        $envobj.name | should be $testname

    }
    It '[New-OctopusResource] creates Project Groups'{
        $Pg = Get-OctopusResourceModel -Resource ProjectGroup
                                                
        $Pg.Name = $testname

        $Pgobj = New-OctopusResource -Resource $Pg

        $Pgobj.name | should be $testname
    }
    It '[New-OctopusResource] creates Projects'{
        $Proj = Get-OctopusResourceModel -Resource Project
                
        $Proj.Name = $testname
        $Proj.ProjectGroupId = (Get-OctopusProjectGroup -Name $TestName).id
        $Proj.LifecycleId = (Get-OctopusLifeCycle)[0].id

        $Projobj = New-OctopusResource -Resource $Proj

        $Projobj.Name | should be $testname
    }
    It '[New-OctopusResource] adds NuGet feeds'{
        $Feedname = $testname
        $feedURL = "https://$testname.com"

        $feed = Get-OctopusResourceModel -Resource NugetFeed

        $feed.Name = $Feedname
        $feed.FeedUri = $feedURL

        $newfeed = New-OctopusResource -Resource $feed

        $newfeed.name | should be $testname 
        $newfeed.feeduri | should be $feedURL
    }
    It '[New-OctopusResource] creates Library Variable Sets'{
        $libraryName = $testname
        $library = Get-OctopusResourceModel -Resource LibraryVariableSet

        $library.Name = $libraryName

        $NewLibrary = New-OctopusResource -Resource $library

        $NewLibrary.name | should be $testname         
    }

    It '[Remove-OctopusResource] deletes environments'{                
        {Get-OctopusEnvironment -Name $testname | Remove-OctopusResource -Force} | should not Throw               

        Get-OctopusEnvironment -Name $TestName -ErrorAction SilentlyContinue | should be $null
    }        
    It '[Remove-OctopusResource] deletes Projects'{
        {Get-OctopusProject -Name $TestName | Remove-OctopusResource -Force} | should not throw

        Get-OctopusProject -Name $TestName -ErrorAction SilentlyContinue| should be $null
    }
    It '[Remove-OctopusResource] deletes Project Groups'{
        {Get-OctopusProjectGroup -Name $TestName |Remove-OctopusResource -Force} | should not throw

        Get-OctopusProjectGroup -Name $TestName -ErrorAction SilentlyContinue | should be $null
    }
    It '[Remove-OctopusResource] deletes NuGet feeds'{
        (Get-OctopusFeed -FeedName $TestName | Remove-OctopusResource -Force) | should be $true
    }
    It '[Remove-OctopusResource] deletes Library Variable Sets'{
        (Get-OctopusVariableSet -LibrarySetName $TestName | Remove-OctopusResource -Force) | should be $true        
    }
    It '[Get/Set-OctopusConnectionInfo] do their thing' {            
        $originalURL = $env:OctopusURL
        $originalAPIKey = $env:OctopusAPIKey

        Set-OctopusConnectionInfo -URL 'SomethingURL' -APIKey 'SomethingAPIKey'

        $ci = Get-OctopusConnectionInfo
        $ci.OctopusURL | should be 'SomethingURL'
        $ci.OctopusAPIKey | should be 'SomethingAPIKey'                

        Set-OctopusConnectionInfo -URL $originalURL -APIKey $originalAPIKey

        $ci = Get-OctopusConnectionInfo
        $ci.OctopusURL | should be $originalURL
        $ci.OctopusAPIKey | should be $originalAPIKey            
    }
    It '[Get/Set-OctopusSMTPConfig] do their thing'{            
        $port = Get-Random
                
        Set-OctopusSMTPConfig -SMTPHost "$TestName" `
        -Port $port -SendEmailFrom 'dalmiro@company.com' | should be $true

        $SMTPConfig = Get-OctopusSMTPConfig

        $SMTPConfig.SMTPHost | Should be $TestName
        $SMTPConfig.SMTPPort | should be $port

        Set-OctopusSMTPConfig -SMTPHost 'Localhost' `
        -Port 25 -SendEmailFrom 'Octopus@company.com' | should be $true

        $SMTPConfig = Get-OctopusSMTPConfig

        $SMTPConfig.SMTPHost | Should be 'Localhost'
        $SMTPConfig.SMTPPort | should be 25
    }
    It '[Get/Set-OctopusMaintenanceMode] do their thing' {
        Set-OctopusMaintenanceMode -Mode ON -Force | should be $true

        (Get-OctopusMaintenanceMode).IsInMaintenanceMode | should be $true

        Set-OctopusMaintenanceMode -Mode OFF -Force | should be $true

        (Get-OctopusMaintenanceMode).IsInMaintenanceMode | should be $False
    }
        It '[Set-OctopusUserAccountStatus] Enables\Disables a user account by name' {        
        $User = Set-OctopusUserAccountStatus -Username 'OT\Tester@OT' -status Disabled
        $User.IsActive | should be 'False'

        $User = Set-OctopusUserAccountStatus -Username 'OT\Tester@OT' -status Enabled
        $User.IsActive | should be 'True'
    }
    It '[Set-OctopusUserAccountStatus] Enables\Disables multiple user accounts by name' {        
        $User = Set-OctopusUserAccountStatus -Username 'OT\Tester@OT','Ian.Paullin@OT' -status Disabled
        $User.IsActive | select -Unique | should be 'False'
        
        $User = Set-OctopusUserAccountStatus -Username 'OT\Tester@OT','Ian.Paullin@OT' -status Enabled
        $User.IsActive | select -Unique | should be 'True'    
    }
    It '[Set-OctopusUserAccountStatus] Doesnt Enable/Disable a non-existent user account by name'{
        $username = "DoesntExist"

        (Set-OctopusUserAccountStatus -Username $username -status Disabled) | should be $null
        (Set-OctopusUserAccountStatus -Username $username -status Enabled) | should be $null

    }
    It '[Set-OctopusUserAccountStatus] Only Enables/Disables users that exist from a list with existent and non-existent usernames'{
        $username = "DoesntExist","OT\Tester@OT"

        $users = Set-OctopusUserAccountStatus -Username $username -status Disabled

        $users.count | should be 1

        $users = Set-OctopusUserAccountStatus -Username $username -status Enabled

        $users.count | should be 1

    }
    It '[Set-OctopusUserAccountStatus] Enables\Disables a user account by single resource' {        
        $List = @()
        $username = "OT\Tester@OT"
        $list += $c.repository.Users.FindMany({param($u) if (($u.username -in $Username) -or ($u.username -like $Username)) {$true}})

        $user = Set-OctopusUserAccountStatus -status Disabled -Resource $List

        $user.isactive | should be 'false'

        $user = Set-OctopusUserAccountStatus -status Enabled -Resource $List

        $user.isactive | should be 'true'
    }
    It '[Set-OctopusUserAccountStatus] Enables\Disables a user account by multiple resources' {        
        $List = @()
        $username = "OT\Tester@OT","Ian.Paullin@OT"
        $list += $c.repository.Users.FindMany({param($u) if (($u.username -in $Username) -or ($u.username -like $Username)) {$true}})

        $users = Set-OctopusUserAccountStatus -status Disabled -Resource $List

        $users.isactive | select -Unique | should be 'false'

        $users = Set-OctopusUserAccountStatus -status Enabled -Resource $List

        $users.isactive | select -Unique | should be 'true'
    }    



}
