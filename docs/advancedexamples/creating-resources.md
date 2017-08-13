*If you are looking for a specific example, leave us a question on* [![Join the chat at https://gitter.im/Dalmirog/OctoPosh](https://badges.gitter.im/Join%20Chat.svg)](https://gitter.im/Dalmirog/OctoPosh?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge&utm_content=badge)

***

Most of the examples to create resources using Octoposh involve 
- Using the `get-OctopusResourceModel` to create a resource object.
- Adding values to mandatory properties of the object.
- Passing the resource object to `New-OctopusResource`.

###Project Groups
```Powershell
#Create a Project Group object
$ProjectGroup = Get-OctopusResourceModel -Resource ProjectGroup

#Add a name to the Project Group
$ProjectGroup.Name = "MyProjectGroup"

#Create the Project Group
New-OctopusResource -Resource $ProjectGroup
```

###Projects
```Powershell
##Create an instance of a project object
$Project = Get-OctopusResourceModel -Resource Project

#Get ProjectGroup and Lifecycle to use their IDs to create the project
$ProjectGroup = Get-OctopusProjectGroup -name "MyProjectGroupw"
$LifeCycle = Get-OctopusLifecycle -name "MyLifeCycle"

#Add mandatory properties to the object
$Project.name = "MyNewProject"
$Project.ProjectGroupID = $ProjectGroup.id
$Project.LifecycleId = $LifeCycle.id

#Create the resource
New-OctopusResource -Resource $Project
```

###Environments
```Powershell
#Create an instance of an environment Object
$Environment = Get-OctopusResourceModel -Resource Environment

#Add mandatory properties to the object
$Environment.name = "Staging"

#Create the resource
New-OctopusResource -Resource $Environment
```

###NuGet Feeds
*This example adds an external NuGet feed reference to Octopus. It doesnt create NuGet feeds*
```Powershell
#Create an instance of an Nuget Feed object
$Feed = Get-OctopusResourceModel -Resource NugetFeed

#Add mandatory properties to the object
$Feed.Name = "MyExternalFeed"
$Feed.FeedUri = "https://MyFeed.com/Nuget"

#Create the resource
$Feed = New-OctopusResource -Resource $Feed
```

###Library Variable Sets
*This snippet will create an empty Library Variable Set. To add variables to it see [using Update-OctopusVariableSet](https://github.com/Dalmirog/OctoPosh/wiki/Modifying-Variable-Sets)*
```Powershell
#Create an instance of a Library Set Object
$library = Get-OctopusResourceModel -Resource LibraryVariableSet

#Add mandatory properties to the object
$library.Name = "MyVariableSet"

#Create the Library Set
New-OctopusResource -Resource $library
```

###Machines
*This code snippet doesnt install the tentacle service on the machine. It just adds the machine to an Octopus Environment. If you are trying to [provision machines automatically](http://docs.octopusdeploy.com/display/OD/Automating+Tentacle+installation), then you are already using Tentacle.exe, so perhaps you might rather keep using that to register to machine to an environment*

**Listening Machine**
```Powershell
##CONFIG

$machineDisplayName = "MyTentacleName" #Name that the Tentacle will have in Octopus
$MachineRoles = "WebServer","SomeOtherRole"#Roles of the Tentacle
$machineHostname = "MYMachineName" #hostname of the Tentacle you want to add. This can either be the ComputerName of the IP address of the Tentacle machine.
$machineEnvironments = "Development","Staging" #Environment where this machine will be registered


##PROCESS
#Create an instance of a Machine Object
$machine = Get-OctopusResourceModel -Resource Machine

#Get Environment to use the ID to create the project
$environments = Get-OctopusEnvironment -EnvironmentName $machineEnvironments -ResourceOnly

#Add mandatory properties to the object
$machine.name = $machineDisplayName #Display name of the machine on Octopus

foreach($environment in $environments){
    $machine.EnvironmentIds.Add($environment.id)
}
foreach ($role in $MachineRoles){
    $machine.Roles.Add($role)    
}
#Use the Discover API to get the machine thumbprint.
$discover = (Invoke-WebRequest "$env:OctopusURL/api/machines/discover?host=$machineHostname&type=TentaclePassive" -Headers (New-OctopusConnection).header).content | ConvertFrom-Json

$machineEndpoint = New-Object Octopus.Client.Model.Endpoints.ListeningTentacleEndpointResource
$machine.EndPoint = $machineEndpoint
$machine.Endpoint.Uri = $discover.Endpoint.Uri
$machine.Endpoint.Thumbprint = $discover.Endpoint.Thumbprint

New-OctopusResource -Resource $machine
```

**Offline Drop**
```Powershell
$OfflineDropName = "MYOfflineDrop"
$Roles = "RoleA","RoleB"
$DropFolderPath = "C:\drop"
$ApplicationsDirectory = "C:\aplications"
$OctopusWorkingDirectory = "C:\Working"
$environmentNames = "Development","Staging"

$machine = Get-OctopusResourceModel -Resource Machine

$machine.Endpoint = New-Object Octopus.Client.Model.Endpoints.OfflineDropEndpointResource

$machine.Name = $OfflineDropName

$Roles | % {$machine.roles.Add($_)}

$machine.Endpoint.ApplicationsDirectory = $ApplicationsDirectory
$machine.Endpoint.DropFolderPath = $DropFolderPath
$machine.Endpoint.OctopusWorkingDirectory = $OctopusWorkingDirectory

$environments = Get-OctopusEnvironment $environmentNames -ResourceOnly

$environments | %{$machine.EnvironmentIds.Add($_.Id)}

New-OctopusResource -Resource $machine
```

###Users

**Create an Octopus user while the Octopus server is in Username/Password authentication mode**
```Powershell
#Create an instance of a User object
$newUser = Get-OctopusResourceModel -Resource User

#Add user properties
$newUser.Username = "" #Username
$newUser.Password = "" #Password
$newUser.DisplayName = "" #Display name. Try to make it match "Username" for consistency
$newUser.EmailAddress = "" #Email associated with user
$newUser.IsActive = $true # Set as false if you want the user to be disabled as soon as you create it 
$newUser.IsService = $false #Set $true if you are trying to create a service account

#Create the user
New-OctopusResource -Resource $newUser
```

**Create an Octopus user while the Octopus server is in Domain (Active Directory) authentication mode**

*Keep in mind that the user must exist on your domain before you do this. This won't create a user in your Active directory, but a user in Octopus mapped to your Active Directory user, which is what happens when a user logs into Octopus using AD for the first time*
```Powershell
$newUser = Get-OctopusResourceModel -Resource User

$newUser.Username = "" #Must match AD username. If your user is Domain\John.Doe, put "John.Doe" on this field.
$newUser.DisplayName = "" #Try to make it match "Username" for consistency.
$newUser.EmailAddress = "" #Email asociated with user
$newUser.IsActive = $true # Set as false if you want the user to be disabled as soon as you create it 
$newUser.IsService = $false #Set $true if you are trying to create a service account

New-OctopusResource -Resource $newUser
```

###Tenants

```Powershell
$TenantName = "MyTenant"

$ProjectName = "MyProject"

$EnvironmentNames = ("Development","staging")

$Tenant = Get-OctopusResourceModel -Resource Tenant
$Project = Get-OctopusProject -ProjectName $ProjectName -ResourceOnly
$environments = Get-OctopusEnvironment -EnvironmentName $EnvironmentNames -ResourceOnly

$Tenant.Name = $TenantName
$Tenant.ConnectToProjectAndEnvironments($Project,$environments)

New-OctopusResource $Tenant
```

###Teams

**Creating a team with 1 role and scoped to 1 Project, 1 Environment, 1 User and 1 Active Directory group**

*Teams involve LOTS of moving pieces, so make sure to take a deep breath before digging into this example*
```Powershell
#Get environments that you'll use as scope for the team
$environments = Get-OctopusEnvironment -EnvironmentName "" -ResourceOnly

#Get projects that you'll use as scope for the team
$projects = Get-OctopusProject -ProjectName "" -ResourceOnly

#Get Octopus Users that will be members of this team
$users = Get-OctopusUser -UserName ""

#Create team object
$teamObj = Get-OctopusResourceModel -Resource Team

#Get user role that will be asigned to the team
$userRoles = Get-OctopusUserRole -UserRoleName "Project Contributor","environment manager"

##Active Directory Group##
<#
On this section you'll need to use the cmdlet Get-ADGroup to get information about the AD group straight from Active Directory.
This cmdlet is part of the ActiveDirectory Powershell module. More info on how to get it on the link below:

https://4sysops.com/archives/how-to-install-the-powershell-active-directory-module/

Needles to say, the machine running the AD cmdlets needs to be on the domain.

#>

$filter = 'Name -eq "MyGroupName"' #Where "MyGroupName" is the name of the AD group. 

$groups = Get-ADGroup -Filter $filter #If you'd like to learn more about the Get-ADGroup cmdlet and how it works (for example to grab more than 1 group at a time), check the MS documentation: https://technet.microsoft.com/en-us/library/ee617196.aspx

##Adding all AD groups found on the line above to the team object.
If($groups){
    foreach ($group in $groups){
        $nri = New-Object Octopus.Platform.Model.NamedReferenceItem

        $nri.DisplayName = $group.Name
        $nri.Id = $group.SID

        $teamObj.ExternalSecurityGroups.Add($nri)
    }
}
###End Active Directory Group###

##Collection Workaround##
<#
Users,Environments, UserRoles and Projects need to be inside of a collection to be attached to the Team Object.

The below workaround makes sure that this example will work, regardless of the amount of Users(etc) that are gathered above.

Try not to touch this part unless you know what you are doing.
#>
$userRoleIds = New-Object 'System.Collections.Generic.List[System.String]'
$userRoles | %{$userRoleIds.Add($_.id)}

$environmentIds = New-Object 'System.Collections.Generic.List[System.String]'
$environments | %{$EnvironmentIds.Add($_.id)}

$projectIds = New-Object 'System.Collections.Generic.List[System.String]'
$projects | %{$projectIds.Add($_.id)}

$userIds = New-Object 'System.Collections.Generic.List[System.String]'
$users | %{$UserIds.Add($_.id)}
##End Collection Workaround

#Adding properties to team object using all the data gathered so far.
$teamObj.Name = "MyTeam" #Name of the team
$teamObj.UserRoleIds = $userRoleIds
$teamObj.EnvironmentIds = $environmentIds
$teamObj.ProjectIds = $projectIds
$teamObj.MemberUserIds = $userIds

#(finally) creating the team!
New-OctopusResource -Resource $teamObj
```
