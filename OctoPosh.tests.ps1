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

Function CreateTestADUser ($TestName){
    Import-Module ActiveDirectory

    If(!(Get-Module ActiveDirectory)){
        Write-Error "ActideDirectory Powershell module not found. Expect user-related tests to fail"
    }
    
    $Name = "$TestName"    
    $Password = convertto-securestring "Michael123!" -asplaintext -force

    New-ADUser -Name $Name `
    -UserPrincipalName $Name `
    -DisplayName $Name `
    -AccountPassword $Password `
    -PasswordNeverExpires $true `
    -ChangePasswordAtLogon $false `
    -Enabled $true `
    -Verbose
}

Function DeleteTestADUser ($testname){
    Import-Module ActiveDirectory

    If(!(Get-Module ActiveDirectory)){
        Write-Error "ActideDirectory Powershell module not found. Expect user-related tests to fail"
    }

    $filter = 'Name -eq ' + "`"" +$testname + "`""

    Get-ADUser -Filter $filter | Remove-ADUser -Verbose -Confirm:$false
}

Describe 'Octopus Module Tests' {

    $TestName = new-testname

    $c = New-OctopusConnection

    CreateTestADUser -TestName $TestName
    CreateTestADUser -TestName ($TestName + "2")

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
    It '[New-OctopusResource] adds a Machine to an Environment'{
        $machine = Get-OctopusResourceModel -Resource Machine
        
        $environment = Get-OctopusEnvironment -EnvironmentName $testname
        
        $machine.name = $testname
        $machine.EnvironmentIds.Add($environment.id) | Out-Null
        $machine.Roles.Add("WebServer") | Out-Null        

        $machineEndpoint = New-Object Octopus.Client.Model.Endpoints.ListeningTentacleEndpointResource
        $machine.EndPoint = $machineEndpoint
        $machine.Endpoint.Uri = "https://localhost:10933"
        $machine.Endpoint.Thumbprint = "8A7E6157A34158EDA1B5127CB027B2A267760A4F"

        $NewMachine = New-OctopusResource -Resource $machine

        $NewMachine.name | should be $testname
    }
    It '[New-OctopusResource] Creates users (only testing this in AD mode)'{
        $TestName1 = $TestName
        $TestName2 = ($TestName + "2")

        #Creating first user
        $newUser1 = Get-OctopusResourceModel -Resource User

        $newUser1.Username = "$TestName1" #Must match AD username if you are using Active Directory Authentication. If your user is Domain\John.Doe, put "John.Doe" on this field.
        $newUser1.DisplayName = "$TestName1" #Try to make it match "Username" for consistency.
        $newUser1.EmailAddress = "$TestName1@email.com"
        $newUser1.IsActive = $true
        $newUser1.IsService = $false

        New-OctopusResource -Resource $newUser1

        Get-OctopusUser -UserName $TestName1 | select -ExpandProperty Username | should be $TestName1

        #Creating 2nd user
        $newUser2 = Get-OctopusResourceModel -Resource User
        $newUser2.Username = "$TestName2" #Must match AD username if you are using Active Directory Authentication. If your user is Domain\John.Doe, put "John.Doe" on this field.
        $newUser2.DisplayName = "$TestName2" #Try to make it match "Username" for consistency.
        $newUser2.EmailAddress = "$TestName2@email.com"
        $newUser2.IsActive = $true
        $newUser2.IsService = $false

        New-OctopusResource -Resource $newUser2
        Get-OctopusUser -UserName $TestName2 | select -ExpandProperty Username | should be $TestName2
    }
    It 'Creating releases for tests'{

        for($i = 0 ;$i -lt 31 ; $i++){
            cd $PSScriptRoot
            & ".\tools\Octo.exe" create-release --server=$env:OctopusURL --apikey=$env:OctopusAPIKey --project=$testname
        }

        1 | should not be $null

    }
    It '[Get-OctopusEnvironment] gets environments'{           
        Get-OctopusEnvironment -Name $TestName | Select-Object -ExpandProperty EnvironmentNAme | should be $TestName
    }
    It '[Get-OctopusEnvironment] returns 0 results when environment name is "" '{
        $env = Get-OctopusEnvironment -Name ""
        $env.count | should be 0
    }
    It '[Get-OctopusProject] gets projects by single name'{
        Get-OctopusProject -Name $TestName | Select-Object -ExpandProperty ProjectName | should be $TestName
    }
    It '[Get-OctopusProject] gets projects by multiple names'{
        $names = Get-OctopusProject -ResourceOnly | Select-Object -First 2 -ExpandProperty Name
        Get-OctopusProject -Name $names | Select-Object -ExpandProperty ProjectName | should be $names
    }
    It '[Get-OctopusProject] doent gets projects by non-existent names'{
        $projectname = "Gengar"
        Get-OctopusProject -ProjectName $projectname -ErrorAction SilentlyContinue| should be $null        
    }
    It '[Get-OctopusProjectGroup] gets Project Groups'{
        Get-OctopusProjectGroup -Name $TestName | Select-Object -ExpandProperty ProjectGroupName | should be $TestName
    }
    It '[Get-OctopusLifecycle] gets Lifecycles'{
        Get-OctopusLifeCycle | should not be $null
    }
    It '[Get-OctopusMachine] gets machines by single name'{
        $Machinename = $TestName
        Get-OctopusMachine -MachineName $Machinename | Select-Object -ExpandProperty Machinename | should be $Machinename
    }    
    It '[Get-OctopusMachine] doesnt get machines by non-existent names '{
        $Machinename = 'Charizard'
        Get-OctopusMachine -MachineName $Machinename -ErrorAction SilentlyContinue | should be $null
    }    
    It '[Get-OctopusMachine] gets machines by communication style'{
        $CommunicationStyle = 'Listening'
        Get-OctopusMachine -CommunicationStyle $CommunicationStyle | Select-Object -ExpandProperty communicationstyle -unique | should be $CommunicationStyle
    }
    It '[Get-OctopusUserRole] Gets a single user role by name'{
        $rolename = "System Administrator"
        Get-OctopusUserRole -UserRoleName $rolename | select -ExpandProperty name | should be $rolename
    }
    It '[Get-OctopusUserRole] Gets multiple roles by names'{
        $rolenames = "System Administrator","Project lead"
        Get-OctopusUserRole -UserRoleName $rolenames | select -ExpandProperty name | should be $rolenames
    }
    It '[Get-OctopusUserRole] Gets multiple roles by names'{
        $rolename = "*Administrator*"
        $userrole = Get-OctopusUserRole -UserRoleName $rolename | select -ExpandProperty name 
        $userrole -like $rolename
        $userrole -like $rolename | should be $true
    }
    It '[Get-OctopusTask] gets tasks by single name'{
        $name = 'Retention'
        Get-OctopusTask -Name $name | Select-Object -ExpandProperty name -Unique | should be $name
    }
    It '[Get-OctopusTask] gets tasks by multiple names'{
        $name1 = 'Retention'
        $name2 = 'Deploy'

        $n1tasks = Get-OctopusTask -Name $name1
        $n2tasks = Get-OctopusTask -Name $name2
    
        $n12tasks = Get-octopustask -Name $name1,$name2

        $n12tasks.count | should be ($n1tasks.count + $n2tasks.count) 
    }
    It '[Get-OctopusTask] gets tasks by single ID'{
        $tasks = Get-OctopusTask -After (Get-Date).AddDays(-10) | Select-Object -First 1
    
        $results = Get-OctopusTask -TaskID $tasks.id

        $results.id | should be $tasks.id            
    }
    It '[Get-OctopusTask] gets tasks by multiple IDs'{
        $i = (Get-Random -Maximum 20)

        $tasks = Get-OctopusTask -After (Get-Date).AddDays(-10) | Select-Object -First $i

        $results = Get-OctopusTask -TaskID $tasks.id

        $results.id | should be $tasks.id            
    }
    It '[Get-OctopusTask] gets tasks by single state'{
        $state = 'Failed'
            
        $tasks = Get-OctopusTask -State $state
    
        $tasks | Select-Object -ExpandProperty State -Unique | should be $state
    }
    It '[Get-OctopusTask] gets tasks by multiple states'{
        $state1 = 'Failed'
        $state2 = 'success'
            
        $s1tasks = Get-OctopusTask -State $state1
        $s2tasks = Get-OctopusTask -State $state2

        $s12tasks = Get-OctopusTask -State $state1,$state2

        $s12tasks.count | should be ($s1tasks.count + $s2tasks.count)            
    }#>
    It '[Get-OctopusTask] gets tasks by single Resource ID'{        
        $envs = Get-OctopusEnvironment | ?{$_.machines.count -gt 0}

        $i = Get-Random -Maximum ($envs.count - 1) -Minimum 0

        $null = Start-OctopusHealthCheck -EnvironmentName $envs[$i].EnvironmentName -Force -Message "[Unit Tests]Health check on environment: $($envs[$i].EnvironmentName)"

        Get-octopustask -ResourceID $envs[$i].Id | should not be $null            
    }
    It '[Get-OctopusTask] gets tasks between 2 date ranges'{        
        $After = (Get-date).Adddays(-10)
        $before = Get-Date
        
        $tasks = Get-OctopusTask -After $After -Before $before

        $tasks.count | should not be 0
        ($tasks.starttime.datetime -gt $before ).count | should be 0
        ($tasks.starttime.datetime -lt $after ).count | should be 0
    } 
    It '[Get-OctopusFeed] gets feeds by name'{
        $feed = Get-OctopusFeed -FeedName $TestName

        $feed.Name | should be $TestName
    }
    It '[Get-OctopusFeed] gets feeds by using wildcards'{
        $feed = Get-OctopusFeed -FeedName "*$($TestName.substring(5))*"

        $feed.Name | should be $TestName
    }  
    It '[Get-OctopusFeed] gets feeds by URL'{
        $feed = Get-OctopusFeed -URL "https://$testname.com"

        $feed.FeedURI| should be "https://$testname.com"
    }
    It '[Get-OctopusFeed] gets feeds by URL using wildcards'{
        $feed = Get-OctopusFeed -URL "*$($TestName.substring(5))*"

        $feed.FeedURI| should be "https://$testname.com"
    }
    It '[Get-OctopusVariableSet] gets variable sets by Project name'{        
        $vs = Get-OctopusVariableSet -Projectname $TestName
        $vs.ProjectName | should be $TestName
    }
    It '[Get-OctopusVariableSet] gets variable sets by Library Set name'{                        
        $Library = Get-OctopusVariableSet -LibrarySetName $TestName
        $Library.LibraryVariableSetName | should be $TestName
    }
    It '[Get-OctopusVariableSet] gets variable sets by Project name & Library Set name'{                        
        $vs = Get-OctopusVariableSet -LibrarySetName $TestName -Projectname $TestName        
        $vs.Count | should be 2        
        $vs.LibraryVariableSetName | select -Unique | should be $TestName
        $vs.ProjectName | select -Unique | should be $TestName
    }
    It '[Get-OctopusRelease] Gets latest X releases of a project'{
        #This uses a hardcoded project with more than 30 releases
        $latest = Get-Random -Minimum 1 -Maximum 30
        $releases = Get-OctopusRelease -ProjectName $TestName -Latest $latest -resourceonly

        $releases.count | should be $latest
    }
    It '[Get-OctopusRelease] [latest] cant get amount of release out of range'{
        {Get-OctopusRelease -ProjectName $TestName -Latest -1} | should throw

        {Get-OctopusRelease -ProjectName $TestName -Latest 31} | should throw
    }
    It '[Get-OctopusRelease] Gets a single release by release version'{
        $release = Get-OctopusRelease -ProjectName $TestName -resourceonly -Latest 1

        (Get-OctopusRelease -ProjectName $TestName -ReleaseVersion $release.version).ReleaseVersion| should be $release.Version
    }
    It '[Get-OctopusRelease] Gets a releases by multiple release versions'{
        $max = 10
        $rel1 = Get-Random -Minimum 1 -Maximum $max
        do{
            $rel2 = Get-Random -Minimum 1 -Maximum $max
        }until($rel2 -ne $rel1)

        $AllReleases = Get-OctopusRelease -ProjectName $TestName -ResourceOnly -Latest $max

        $releases = Get-OctopusRelease -ProjectName $TestName -resourceonly -ReleaseVersion $AllReleases[$rel1].version,$AllReleases[$rel2].version

        $releases.count | should be 2
        
    }
    It '[Get-OctopusUser] gets user by single name'{
        Get-OctopusUser -UserName $TestName | select -ExpandProperty Username | should be $TestName
    }
    It '[Get-OctopusUser] gets multiple users by name'{
        $Names = ($TestName,($TestName + "2"))
        $users = Get-OctopusUser -UserName $Names
        $users.Username | should be $Names
    }
    It '[Get-OctopusUser] gets multiple users by name using wildcard'{
        $Names = ($TestName,($TestName + "2"))
        $users = Get-OctopusUser -UserName "$TestName*"
        $users.Username | should be $Names
    }
    It '[Update-OctopusReleaseVariableSet] updates the variable set of a release'{
        $release = Get-OctopusRelease -ProjectName $TestName -Latest 1 -ResourceOnly
        Update-OctopusReleaseVariableSet -ProjectName $TestName -ReleaseVersion $release.version | should be $true
    }
    It '[Update-OctopusReleaseVariableSet] Doesnt update the variable set of a Release that doesnt exist'{
        Update-OctopusReleaseVariableSet -ProjectName $TestName -ReleaseVersion 100.90.34 -ErrorAction SilentlyContinue | should be $false
    }
    It '[Update-OctopusReleaseVariableSet] Doesnt update the variable set of a Release of a Project that doesnt exist'{
        Update-OctopusReleaseVariableSet -ProjectName unexistentproject -ReleaseVersion 1.0.34 -ErrorAction SilentlyContinue | should be $false
    }
    It '[Update-OctopusResource] Updates ProjectGroups'{
        $description = "New Description"
        
        $projectGroup = Get-OctopusProjectGroup -name $TestName

        $projectGroup.resource.Description = $description

        $projectGroup | Update-OctopusResource -Force | select -ExpandProperty Description -Unique | should be $description
    }
    It '[Update-OctopusResource] Updates Projects'{
        $description = "New Description"

        $project = Get-OctopusProject -name $TestName

        $project.resource.Description = $description

        $project | Update-OctopusResource -Force | select -ExpandProperty Description -Unique | should be $description
    }
    It '[Update-OctopusResource] Updates Environments'{
        $description = "New Description"

        $Environment = Get-OctopusEnvironment -name $TestName

        $Environment.resource.Description = $description

        $Environment | Update-OctopusResource -Force | select -ExpandProperty Description -Unique| should be $description
    }
    It '[Update-OctopusResource] Updates Machines'{
        $Role = "SomeRole"

        $Machine = Get-OctopusMachine -name $TestName

        $Machine.resource.roles.Add($Role)

        $role -in ($Machine | Update-OctopusResource -Force).roles | should be $true
    }
    It '[Update-OctopusResource] Updates External Feeds'{
        $URL = "https://SomeURL.com/Nuget"

        $Feed = Get-OctopusFeed -FeedName $TestName

        $feed.Resource.FeedUri = $URL

        $feed | Update-OctopusResource -Force | select -ExpandProperty FeedURI -Unique | should be $URL
    }
    It '[Update-OctopusResource] Updates users'{
        $newDisplayName = ($TestName + "Modified")

        $user = Get-OctopusUser -UserName $TestName
        
        $user.Displayname = $newDisplayName

        Update-OctopusResource -Resource $user -Force

        Get-OctopusUser -UserName $TestName | select -ExpandProperty Displayname | should be $newDisplayName
    }
    It '[Block/Unblock-OctopusRelease] blocks/unblocks a release'{
        $release = Get-OctopusRelease -ProjectName $TestName -Latest 1
            
        $release | Block-OctopusRelease -Description $TestName -Force | should be $true

        $release | UnBlock-OctopusRelease -Force | should be $true
    }        
    It '[Remove-OctopusResource] deletes Projects'{
        (Get-OctopusProject -Name $TestName | Remove-OctopusResource -Force) | should be $true

        Get-OctopusProject -Name $TestName -ErrorAction SilentlyContinue| should be $null
    }
    It '[Remove-OctopusResource] deletes Project Groups'{
        (Get-OctopusProjectGroup -Name $TestName |Remove-OctopusResource -Force) | should be $true

        Get-OctopusProjectGroup -Name $TestName -ErrorAction SilentlyContinue | should be $null
    }
    It '[Remove-OctopusResource] deletes NuGet feeds'{
        (Get-OctopusFeed -FeedName $TestName | Remove-OctopusResource -Force) | should be $true

        Get-OctopusFeed -FeedName $TestName -ErrorAction SilentlyContinue | should be $null
    }
    It '[Remove-OctopusResource] deletes Library Variable Sets'{
        (Get-OctopusVariableSet -LibrarySetName $TestName | Remove-OctopusResource -Force) | should be $true        

        Get-OctopusVariableSet -LibrarySetName $TestName -ErrorAction SilentlyContinue | should be $null
    }
    It '[Remove-OctopusResource] deletes Machines'{
        (Get-OctopusMachine -MachineName $TestName | Remove-OctopusResource -Force) | should be $true       
        
        Get-OctopusMachine -Name $TestName -ErrorAction SilentlyContinue | should be $null
    }
    It '[Remove-OctopusResource] deletes environments'{                
        (Get-OctopusEnvironment -Name $testname | Remove-OctopusResource -Force) | should be $true

        Get-OctopusEnvironment -Name $TestName -ErrorAction SilentlyContinue | should be $null
    }
    It '[Remove-OctopusResource] deletes users'{
        $user1 = Get-OctopusUser -UserName "$TestName" ; Remove-OctopusResource -Resource $user1 -Force | should be $true
        $user2 = Get-OctopusUser -UserName ("$TestName"+"2") ; Remove-OctopusResource -Resource $user2 -Force | should be $true
    }
    It '[Start-OctopusCalamariUpdate] starts a calamari update task agains a single environment'{
        $Environments = Get-OctopusEnvironment | ?{$_.Machines.count -gt 0}
        $i = Get-Random -Minimum 0 -Maximum ($Environments.count - 1)

        $task = Start-OctopusCalamariUpdate -EnvironmentName $Environments[$i].EnvironmentName -Force
        $task.gettype() | should be "Octopus.Client.Model.TaskResource"

        $Machines = Get-OctopusMachine -ResourceOnly
        $i = Get-Random -Minimum 0 -Maximum ($Machines.count - 1)

        $task = Start-OctopusCalamariUpdate -MachineName $Machines[$i].Name -Force
        $task.gettype() | should be "Octopus.Client.Model.TaskResource"
    }
    It '[Start-OctopusCalamariUpdate] doesnt start a task if at least 1 machine/environment doesnt exist'{
        $machines = Get-OctopusMachine -ResourceOnly
        $i = Get-Random -Minimum 0 -Maximum ($machines.count -1)
        
        { Start-OctopusCalamariUpdate -MachineName (Get-Random -Maximum 10000),$machines[$i].Name -Force -ErrorAction Stop }| should Throw

        $environments = Get-OctopusEnvironment -ResourceOnly
        $i = Get-Random -Minimum 0 -Maximum ($environments.count -1)
        
        { Start-OctopusCalamariUpdate -EnvironmentName (Get-Random -Maximum 10000),$environments[$i].Name -Force -ErrorAction Stop }| should Throw
       
    }
    It '[Start-OctopusRetentionPolicy] starts a "Retention" task'{
        $task = Start-OctopusRetentionPolicy -Force -Wait

        $task.GetType().fullname| should be 'Octopus.Client.Model.TaskResource'
        $task.name | should be "Retention"
    }
    It '[Start-OctopusHealthChech] doesnt start health checks on empty environments'{
        $EnvironmentName = "EmptyEnvironment"

        $env = Get-OctopusResourceModel -Resource Environment                

        $env.Name = $EnvironmentName
                
        $envobj = New-OctopusResource -Resource $env

        (Start-OctopusHealthCheck -EnvironmentName $EnvironmentName -Force -ErrorAction SilentlyContinue) | should be $null

        $delete = Remove-OctopusResource -Resource $envobj -Force
    }
    <#It '[Start-OctopusHealthChech] starts a health check on a single environment'{
        $task = Start-OctopusHealthCheck -EnvironmentName 'Staging' -Force -ErrorAction SilentlyContinue
        $task.count | should be 1
        $task.GetType().fullname| should be 'Octopus.Client.Model.TaskResource'
    }
    It '[Start-OctopusHealthChech] starts a health check on multiple environments'{
        $tasks = Start-OctopusHealthCheck -EnvironmentName 'Staging','production' -Force -ErrorAction SilentlyContinue
        $tasks.count | should be 2
        $tasks | Get-Member | Select-Object -ExpandProperty typename -Unique | should be 'Octopus.Client.Model.TaskResource'
    }#>
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
    It '[New-OctopusAPIKey] creates an API Key'{
        $api = New-OctopusAPIKey -Purpose "$TestName" -Username 'Ian.Paullin' -password 'Michael3' -NoWarning -OctopusURL $env:OctopusURL
                
        $api.purpose | should be $TestName

        $api.APIKey | should not be $null

        {$c.repository.Users.RevokeApiKey($api)} | should not throw

    }

    DeleteTestADUser -testname $TestName
    DeleteTestADUser -testname ($TestName + "2")
}
#Block to tests particular tests while debugging
Describe 'Test'{
    <#
    $TestName = new-testname
    CreateTestADUser -TestName $TestName
    CreateTestADUser -TestName ($TestName + "2")

    It '[New-OctopusResource] Creates users (only testing this in AD mode)'{
        $TestName1 = $TestName
        $TestName2 = ($TestName + "2")

        #Creating first user
        $newUser1 = Get-OctopusResourceModel -Resource User

        $newUser1.Username = "$TestName1" #Must match AD username if you are using Active Directory Authentication. If your user is Domain\John.Doe, put "John.Doe" on this field.
        $newUser1.DisplayName = "$TestName1" #Try to make it match "Username" for consistency.
        $newUser1.EmailAddress = "$TestName1@email.com"
        $newUser1.IsActive = $true
        $newUser1.IsService = $false

        New-OctopusResource -Resource $newUser1

        Get-OctopusUser -UserName $TestName1 | select -ExpandProperty Username | should be $TestName1

        #Creating 2nd user
        $newUser2 = Get-OctopusResourceModel -Resource User
        $newUser2.Username = "$TestName2" #Must match AD username if you are using Active Directory Authentication. If your user is Domain\John.Doe, put "John.Doe" on this field.
        $newUser2.DisplayName = "$TestName2" #Try to make it match "Username" for consistency.
        $newUser2.EmailAddress = "$TestName2@email.com"
        $newUser2.IsActive = $true
        $newUser2.IsService = $false

        New-OctopusResource -Resource $newUser2
        Get-OctopusUser -UserName $TestName2 | select -ExpandProperty Username | should be $TestName2
    }
    It '[Get-OctopusUser] gets user by single name'{
        Get-OctopusUser -UserName $TestName | select -ExpandProperty Username | should be $TestName
    }
    It '[Get-OctopusUser] gets multiple users by name'{
        $Names = ($TestName,($TestName + "2"))
        $users = Get-OctopusUser -UserName $Names
        $users.Username | should be $Names
    }
    It '[Get-OctopusUser] gets multiple users by name using wildcard'{
        $Names = ($TestName,($TestName + "2"))
        $users = Get-OctopusUser -UserName "$TestName*"
        $users.Username | should be $Names
    }
    It '[Update-OctopusResource] Updates users'{
        $newDisplayName = ($TestName + "Modified")

        $user = Get-OctopusUser -UserName $TestName
        
        $user.Displayname = $newDisplayName

        Update-OctopusResource -Resource $user -Force

        Get-OctopusUser -UserName $TestName | select -ExpandProperty Displayname | should be $newDisplayName
    }
    It '[Remove-OctopusResource] deletes users'{
        $user1 = Get-OctopusUser -UserName "$TestName" ; Remove-OctopusResource -Resource $user1 -Force | should be $true
        $user2 = Get-OctopusUser -UserName ("$TestName"+"2") ; Remove-OctopusResource -Resource $user2 -Force | should be $true
    }

    DeleteTestADUser -testname $TestName
    DeleteTestADUser -testname ($TestName + "2")
    #>
}