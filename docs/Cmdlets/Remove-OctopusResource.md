### Summary
Deletes resources from an Octopus Instance
### Parameters
| Name | DataType          | Description |
| ------------- | ----------- | ----------- |
| Resource |  |  Resource Object to delete from the Octopus Server     |

### Syntax
``` powershell

Remove-OctopusResource [[-Resource] <Resource[]>] [<CommonParameters>]




``` 

### Examples
Deletes the project called "MyApp" from the Octopus Instance

 ``` powershell 
 PS C:\> $ProjectResource = Get-OctopusProject -name "MyApp" ; Remove-OctopusResource -resource $ProjectResource
 ``` 

Deletes all the projects inside the Project Group "MyProjects"

 ``` powershell 
 PS C:\> Get-OctopusProjectGroup -name "MyProjects" | select -ExpandProperty Projects | Remove-OctopusResource
 ``` 

