### Summary
This cmdlet returns information about Octopus Lifecycles
### Parameters
| Name | DataType          | Description |
| ------------- | ----------- | ----------- |
| LifecycleName | String[] |  Lifecycle name     |
| ResourceOnly | Switch |  If set to TRUE the cmdlet will return the basic Octopur resource. If not set or set to FALSE, the cmdlet will return a human friendly Octoposh  output object     |

### Syntax
``` powershell

Get-OctopusLifecycle [[-LifecycleName] <string[]>] [-ResourceOnly <SwitchParameter>] [<CommonParameters>]




``` 

### Examples
Get all the Lifecycles of the current Instance

``` powershell 
 Get-OctopusLifecycle
``` 

Get the Lifecycle named "MyLifecycle"

``` powershell 
 Get-OctopusLifecycle -name MyLifecycle
``` 

Gets the teams with the names "ProjectA_Lifecycle" and "ProjectB_Lifecycle"

``` powershell 
 Get-OctopusLifecycle -name "ProjectA_Lifecycle","ProjectB_Lifecycle"
``` 

Gets all the lifecycles whose name starts with "ProjectA"

``` powershell 
 Get-OctopusLifecycle -name "ProjectA*"
``` 

