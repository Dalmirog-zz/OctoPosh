*If you are looking for a specific example, leave us a request on* [![Join the chat at https://gitter.im/Dalmirog/OctoPosh](https://badges.gitter.im/Join%20Chat.svg)](https://gitter.im/Dalmirog/OctoPosh?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge&utm_content=badge)
***

Most of the examples to modify resources using Octoposh involve 
- Using the `get-Octopus[resource]` cmdlet to retrieve the resource you want to modify.
- Modifying the properties of the resource object.
- Passing the resource object to `Update-OctopusResource`.

None of the examples below include the `-force` switch to avoid any accidents. If you want to avoid getting prompted every time you update a resource, pass the switch `-Force` to `Update-OctopusResource`

## Machines

### Enable/Disable a machine
```Powershell
$machine = Get-OctopusMachine -MachineName "SQL_Production" 
$machine.resource.isdisabled = $true #Set to $False to enable the machine/s
$machine | Update-OctopusResource
```

### Enable/Disable all the machines in an environment
```Powershell
$machines = Get-OctopusMachine -Environment UAT
$machines | %{$_.isdisabled = $true} #Set to $False to enable the machine/s
$machines | update-OctopusResource
```

### Add machine to an environment
```Powershell
#Getting Source Environment
$Environment = Get-OctopusEnvironment -EnvironmentName "MYEnvironment"

#Getting the machine
$machine = Get-OctopusMachine -Machinename "MyMachine"

#Adding environment to machine
$machine.resource.EnvironmentIds.Add($Environment.id)

#Saving changes on the database
Update-OctopusResource -Resource $machine.Resource -Force
```

### Move the machines from one environment to another
```Powershell
$Source = "" #Name of the source environment
$Destination = "" #Name of the Destination environment

#Getting Source Environment
$sourceEnv = Get-OctopusEnvironment -EnvironmentName $Source

#Getting Destination Environment
$destinationEnv = Get-OctopusEnvironment -EnvironmentName $Destination

#Getting ONLY 1 machine to migrate. If you want to migrate all the machines on the environment, comment this line and uncomment the one below
$machines = Get-OctopusMachine -Machinename "MyMachine"

#Getting all machines from Source environment
#$machines = Get-OctopusMachine -EnvironmentName $sourceEnv.EnvironmentName



foreach ($machine in $machines){
    
    #Removing source environment from Environment IDs of the machine
    $machine.resource.EnvironmentIds.remove($sourceEnv.id)

    #Adding Destination environment from EnvironmentIds of the machine
    $machine.resource.EnvironmentIds.Add($destinationEnv.id)

    #Saving changes on the database
    Update-OctopusResource -Resource $machine.Resource -Force
}
```

### Update machine's roles
```Powershell
#getting the machine
$machine = Get-OctopusMachine -MachineName "MyMachine" 

#updating the machine's roles
$machine.resource.roles.add("NewRoleToAdd")
$machine.resource.roles.Remove("OldRoleToRemove")

#Saving the machine resource on the database
Update-OctopusResource -Resource $machine.resource

## 2) Updating the roles of a group of machines

#Getting all the machines inside of an Environment
$EnvMachines = Get-OctopusMachine -EnvironmentName Development

#Looping through all the machines and changing their roles
foreach ($machine in $EnvMachines){
    
    $machine.resource.roles.add("NewRoleToAdd") #adding a role
    $machine.resource.roles.Remove("OldRoleToRemove") #removing a role
}

#updating all machines at once
Update-OctopusResource -Resource $EnvMachines.resource -Force
```

## Variable Sets

### Put all of a (single) Project's variables into a New Library Variable Set
```Powershell
$NewLibraryVariableSetName = "MyNewVariableSet" #Name of the new Library Variable Set
$SourceProject = "MyProject" #Name of the project where you want to take the variables from

#Get the Project Variable Set
$ProjectVariableSet = Get-OctopusVariableSet -Projectname $SourceProject

#Create an instance of a LibraryVariableSet Object
$NewLibraryVariableSetObject = Get-OctopusResourceModel -Resource LibraryVariableSet

#Name the set object
$NewLibraryVariableSetObject.Name = $NewLibraryVariableSetName

#Save the set to the Octopus Database
New-OctopusResource -Resource $NewVariableSet

#Get the new variable set from the Octopus Database
$NewLibraryVariableSet = Get-OctopusVariableSet -LibrarySetName $NewLibraryVariableSetName

#Set the variables of the Variable Set to be == to the variables of the Project Variable Set.
$NewLibraryVariableSet.Resource.Variables = $ProjectVariableSet.Resource.Variables

#Save the changes to the new variable set on the Database
Update-OctopusResource -Resource $NewLibraryVariableSet.Resource -Force
```

### Append all of a Project's variables into an already existing Library Variable Set
```Powershell
#Name of the libraty variable set where you want to put the variables on
$LibraryVariableSetName = "MyAlreadyExistingVariable" 

#Name of the project where you want to take the variables from
$SourceProjectName = "MyProject" 

#Get the Project Variable Set
$ProjectVariableSet = Get-OctopusVariableSet -Projectname $SourceProjectName

#Get the new variable set from the Octopus Database
$LibraryVariableSet = Get-OctopusVariableSet -LibrarySetName $LibraryVariableSetName

#Set the variables of the Variable Set to be == to the variables of the Project Variable Set.

foreach($variable in $ProjectVariableSet.Resource.Variables){
    $LibraryVariableSet.Resource.Variables.Add($variable)
}
#Save the changes to the new variable set on the Database
Update-OctopusResource -Resource $LibraryVariableSet.Resource -Force
```

## Teams

### Add a user to a team
```Powershell
$user = Get-OctopusUser -UserName "MyUserToAdd"
$team = Get-OctopusTeam -TeamName "MyTeam"

$team.MemberUserIds.Add($user.id)

Update-OctopusResource -Resource $team
```

## Users

### Change a user display name
```Powershell
$user = Get-OctopusUser -UserName "MyUsername"
$user.DisplayName = "NewUserDisplayName"

Update-OctopusResource -Resource $user
```
