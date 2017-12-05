*If you are looking for a specific example, leave us a question on* [![Join the chat at https://gitter.im/Dalmirog/OctoPosh](https://badges.gitter.im/Join%20Chat.svg)](https://gitter.im/Dalmirog/OctoPosh?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge&utm_content=badge)
***

Most of the examples to delete resources on using Octoposh involve 
- Using the `get-Octopus**` to get the resource you want to delete
- Piping the resource object to `Remove-OctopusResource`

None of the examples below include the `-force` switch to avoid any accidents. If you want to avoid getting prompted every time you delete a resource, pass the switch `-Force` to `Remove-OctopusResource`

###Project Groups
```Powershell
Get-OctopusProjectGroup -name "MyProjectGroup" | Remove-OctopusResource
```

###Projects
Delete one project
```Powershell
Get-OctopusProject -name "MyProject" | Remove-OctopusResource
```
Delete all the projects inside a ProjectGroup
```Powershell
Get-OctopusProjectGroup -name "MyProjectGroup" | Get-OctopusProject | Remove-OctopusResource
```

###Environments

Delete single environment by name
```Powershell
Get-OctopusEnvironment -name "MyEnvironment" | Remove-OctopusResource
```

Delete all environments that have no machines in them
```Powershell
Get-OctopusEnvironment | ? {$_.Machines.count -eq 0} |Remove-OctopusResource
```


###NuGet Feeds
*This example removes the reference of an external NuGet feed from Octopus. It doesnt delete any packages from the NuGet feed itself*
```Powershell
Get-OctopusFeed -Name "MyFeed" | Remove-OctopusResource
```

###Library variable sets
```Powershell
Get-OctopusVariableSet -LibrarySetName "MyLibrarySet" | Remove-OctopusResource
```

###Machines
```Powershell
Get-OctopusMachine -Name "MySQLDatabase" | Remove-OctopusResource
```

###Lifecycles
```Powershell
Get-OctopusLifecycle -name "MyLifecycle" | Remove-OctopusResource
```

###Users
```#Powershell
$user = Get-OctopusUser -name "MyUser" 
Remove-OctopusResource -resource $user
```

###Teams
```Powershell
$team = Get-OctopusTeam -name "MyTeam" 
Remove-OctopusResource - resource $team
```
