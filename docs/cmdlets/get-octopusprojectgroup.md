
### Summary

Gets information about Octopus Project Groups
### Parameters
| Name | DataType          | Description |
| ------------- | ----------- | ----------- |
| ProjectGroupName | String[] |  Project Group name     |
| ResourceOnly | Switch |  If set to TRUE the cmdlet will return the basic Octopur resource. If not set or set to FALSE, the  cmdlet will return a human friendly Octoposh output object     |

### Syntax
``` powershell

Get-OctopusProjectGroup [[-ProjectGroupName] <string[]>] [-ResourceOnly <SwitchParameter>] 
[<CommonParameters>]




``` 

### Examples 

**EXAMPLE 1**

Gets all the Project Groups on the Octopus instance

``` powershell 
 Get-OctopusProjectGroup
``` 

**EXAMPLE 2**

Gets a Project Group named "MyProjects"

``` powershell 
 Get-OctopusProjectGroup -name "MyProjects"
``` 

**EXAMPLE 3**

Get all the projects whose name matches the pattern "*web*"

``` powershell 
 Get-OctopusProjectGroup -name "*web*"
``` 

