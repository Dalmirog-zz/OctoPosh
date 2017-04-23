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
**EXAMPLE 5**

Deletes the project called "MyApp" from the Octopus Instance

 ``` powershell 
 PS C:\> $ProjectResource = Get-OctopusProject -name "MyApp" ; Remove-OctopusResource -resource $ProjectResource
 ``` 

**EXAMPLE 5**

Deletes all the projects inside the Project Group "MyProjects"

 ``` powershell 
 PS C:\> Get-OctopusProjectGroup -name "MyProjects" | select -ExpandProperty Projects | Remove-OctopusResource
 ``` 

