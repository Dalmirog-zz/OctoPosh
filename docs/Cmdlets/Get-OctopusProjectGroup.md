### Summary
Gets information about Octopus Project Groups
### Parameters
| Name | DataType          | Description |
| ------------- | ----------- | ----------- |
| ProjectGroupName | String[] |  Project Group name     |
| ResourceOnly | Switch |  If set to TRUE the cmdlet will return the basic Octopur resource. If not set or set to FALSE, the cmdlet will return a human friendly Octoposh  output object     |

### Syntax
``` powershell

Get-OctopusProjectGroup [[-ProjectGroupName] <string[]>] [-ResourceOnly <SwitchParameter>] [<CommonParameters>]




``` 

### Examples
**EXAMPLE 5**

Gets all the Project Groups on the Octopus instance

 ``` powershell 
 PS C:\> Get-OctopusProjectGroup
 ``` 

**EXAMPLE 5**

Gets a Project Group named "MyProjects"

 ``` powershell 
 PS C:\> Get-OctopusProjectGroup -name "MyProjects"
 ``` 

**EXAMPLE 5**

Get all the projects whose name matches the pattern "*web*"

 ``` powershell 
 PS C:\> Get-OctopusProjectGroup -name "*web*"
 ``` 

